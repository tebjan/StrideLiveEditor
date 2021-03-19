using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Stride.Core.Extensions;
using Stride.Engine;
using System.Reactive.Linq;

namespace StrideLiveEditor.Avalonia
{
    public class LiveEditorMainWindow : Window
    {
        public StackPanel ComponentGridList => this.FindControl<StackPanel>("componentGridList");
        public TextBlock TxtLog => this.FindControl<TextBlock>("txtLog");
        public ScrollViewer LogScrollViewer => this.FindControl<ScrollViewer>("logScrollViewer");
        public Grid RootGrid => this.FindControl<Grid>("RootGrid");

        public LiveEditorMainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private Game game;
        private SceneInstance sceneInstance;

        public ObservableCollection<EntityTreeItem> Entities { get; set; } = new ObservableCollection<EntityTreeItem>();

        //Scenes stored in a separate container to save tree traversal
        //TODO: Maybe factor this out
        public ObservableCollection<SceneItem> Scenes { get; set; } = new ObservableCollection<SceneItem>();

        private EntityTreeItem selectedEntity;
        private Dictionary<Type, EntityComponentInfo> componentInfos = new Dictionary<Type, EntityComponentInfo>();

        private bool componentsInitialized = false;

        public LiveEditorMainWindow(Game game) : this()
        {
            if (game == null)
                throw new ArgumentNullException("game");

            LogScrollViewer.GetObservable(ScrollViewer.ExtentProperty)
                .Subscribe(newExtent => LogScrollViewer.Offset = new Vector(0, newExtent.Height));

            RootGrid.DataContext = this;

            this.game = game;

            Task.Factory.StartNew(GetSceneInstance);
            Task.Factory.StartNew(UpdateComponentValuesTicker);
            Task.Factory.StartNew(UpdateNamesTicker);
        }

        #region Setup Stride Bindings

        private async void GetSceneInstance()
        {
            await WaitForSceneSystem();
            await WaitForSceneInstance();

            sceneInstance = game.SceneSystem.SceneInstance;

            if (sceneInstance == null)
                Log(LogLevel.Error, "No scene instance found.");
            else
                await Dispatcher.UIThread.InvokeAsync(OnSceneInstanceReady);
        }

        private async Task WaitForSceneSystem()
        {
            Log("Waiting for scene system...");

            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    if (game.SceneSystem != null)
                        return;

                    await Task.Delay(100);
                }
            });
        }

        private async Task WaitForSceneInstance()
        {
            Log("Waiting for scene instance...");
            while (true)
            {
                if (game.SceneSystem != null && game.SceneSystem.SceneInstance != null)
                    return;

                await Task.Delay(100);
            }
        }

        #endregion Setup Stride Bindings

        #region Stride Event Handlers

        private void SceneInstance_EntityAdded(object sender, Entity e)
        {
            OnEntityAdded(e);
        }

        private void SceneInstance_EntityRemoved(object sender, Entity e)
        {
            OnEntityRemoved(e);
        }

        private void SceneInstance_SceneChanged(object sender, EventArgs e)
        {
            Log("Scene changed");
        }

        private void SceneInstance_ComponentChanged(object sender, Stride.Engine.Design.EntityComponentEventArgs e)
        {
            if (selectedEntity == null || e.Entity != selectedEntity.Entity)
                return;

            if (e.NewComponent != null)
                OnComponentAdded(e.NewComponent);
            if (e.PreviousComponent != null)
                OnComponentRemoved(e.NewComponent);
        }

        private void AddSceneInstanceEvents()
        {
            sceneInstance.RootSceneChanged += SceneInstance_SceneChanged;
            sceneInstance.EntityAdded += SceneInstance_EntityAdded;
            sceneInstance.EntityRemoved += SceneInstance_EntityRemoved;
            sceneInstance.ComponentChanged += SceneInstance_ComponentChanged;
            sceneInstance.RootScene.Children.CollectionChanged += Children_CollectionChanged;

        }

        private void RemoveSceneInstanceEvents()
        {
            if (sceneInstance == null)
                return;

            sceneInstance.RootSceneChanged -= SceneInstance_SceneChanged;
            sceneInstance.EntityAdded -= SceneInstance_EntityAdded;
            sceneInstance.EntityRemoved -= SceneInstance_EntityRemoved;
            sceneInstance.ComponentChanged -= SceneInstance_ComponentChanged;
            sceneInstance.RootScene.Children.CollectionChanged -= Children_CollectionChanged;

        }

        private void Children_CollectionChanged(object sender, Stride.Core.Collections.TrackingCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    Scene scene = (Scene)e.Item;
                    Scene parentScene = scene.Parent;

                    if (parentScene != null)
                    {
                        SceneItem parentInTree = FindSceneInTree(Scenes, parentScene);
                        if (parentInTree != null)
                        {
                            //Add in the scene into the tree
                            EntityTreeItem newSceneInTree = new EntityTreeItem(scene);
                            Scenes.Add(new SceneItem(newSceneInTree));
                            parentInTree.Entities.Add(newSceneInTree);

                            foreach (var entity in scene.Entities)
                            {
                                OnEntityAdded(entity);
                            }

                            //In the unlikely event that additional child scenes have been added before XLE
                            //pickup up the event, loop recursively
                            foreach (var s in scene.Children)
                            {
                                Children_CollectionChanged(null,
                                    new Stride.Core.Collections.TrackingCollectionChangedEventArgs(
                                    System.Collections.Specialized.NotifyCollectionChangedAction.Add, s, null, 0, false)
                                    );
                            }
                        }
                    }

                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    //Trigger a rebuild of the tree
                    //Bit of task trickery here as the this even fires well before actual unloading
                    var t = Task.Factory.StartNew(() =>
                    {
                        Task.Delay(500).Wait(); //TODO: 500 is totally a magic number here
                        BuildTree();
                    },
                        CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskScheduler.FromCurrentSynchronizationContext()
                    );
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                    break;
                default:
                    break;
            }
        }

        #endregion Stride Event Handlers

        #region UI Events

        private void entityTreeView_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedEntity = null;
            ClearComponentView();

            if (e == null || e.AddedItems.IsNullOrEmpty())
                return;

            var entity = e.AddedItems.OfType<EntityTreeItem>().First();
            if (entity == null)
                return;

            selectedEntity = entity;

            // Build component view and add components changed event handling.
            // Spin up watcher thread to watch for component value changes.

            if (entity.Entity.GetType() == typeof(Entity))
            {
                Entity actualEntity = (Entity)entity.Entity;
                foreach (var component in actualEntity.Components)
                {
                    OnComponentAdded(component);
                }
            }

            componentsInitialized = true;
        }

        #endregion UI Events

        #region UI Updates

        private void OnSceneInstanceReady()
        {
            AddSceneInstanceEvents();

            BuildTree();

        }

        private void OnEntityAdded(Entity entity)
        {
            Log($"Entity {entity.Name} added.");

            SceneItem sceneUpdated = FindSceneInTree(Scenes, entity.Scene);

            if (sceneUpdated != null)
            {
                sceneUpdated.OnEntityAdded(entity);
            }
        }

        private void OnEntityRemoved(Entity entity)
        {
            Log($"Entity {entity.Name} removed.");

            SceneItem sceneUpdated = FindSceneInTree(Scenes, entity.Scene);

            if (sceneUpdated != null && game.IsRunning && !game.IsExiting)
            {
                sceneUpdated.OnEntityRemoved(entity);
            }
        }

        private void OnComponentAdded(EntityComponent component)
        {
            var expander = new EntityComponentExpander(component);

            expander.Root.IsExpanded = true;

            var componentInfo = GetEntityComponentInfo(component.GetType());

            expander.Root.Header = componentInfo.Name;

            var propertyEditors = new List<UserControl>();

            foreach (var prop in componentInfo.Properties)
            {
                if (prop.Name == "Id")
                    continue;

                var elem = GetEditorForProperty(component, prop);
                expander.ComponentList.Children.Add(elem);
            }

            ComponentGridList.Children.Add(expander);
        }

        private void OnComponentRemoved(EntityComponent component)
        {
            var listItem = ComponentGridList.Children.Cast<EntityComponentExpander>().FirstOrDefault(c => c.Component == component);
            if (listItem != null)
                ComponentGridList.Children.Remove(listItem);
        }

        private void ClearComponentView()
        {
            ComponentGridList.Children.Clear();
            componentsInitialized = false;
        }

        #endregion UI Updates

        #region Methods

        private void Log(string message)
        {
            Log(LogLevel.Info, message);
        }

        private void Log(LogLevel logLevel, string message)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                TxtLog.Text += message + "\n";
            });
        }

        public void BuildTree()
        {
            Entities.Clear();
            Scenes.Clear();

            //Add in the root scene
            EntityTreeItem treeRoot = new EntityTreeItem(sceneInstance.RootScene);
            Scenes.Add(new SceneItem(treeRoot));
            Entities.Add(treeRoot);

            //Loop through any children
            foreach (var s in sceneInstance.RootScene.Children)
            {
                Children_CollectionChanged(null,
                    new Stride.Core.Collections.TrackingCollectionChangedEventArgs(
                    System.Collections.Specialized.NotifyCollectionChangedAction.Add, s, null, 0, false)
                    );
            }

            //Then add the entities of the root scene
            foreach (var entity in sceneInstance.RootScene.Entities)
            {
                OnEntityAdded(entity);
            }

            //Expand the root of the tree to open it
            //TODO: Work out how the hell to do this

        }

        private async void UpdateComponentValuesTicker()
        {
            while (true)
            {
                await Task.Delay(100);

                if (selectedEntity == null || !componentsInitialized)
                    continue;

                await Dispatcher.UIThread.InvokeAsync(UpdateComponentValues);
            }
        }

        private void UpdateComponentValues()
        {
            foreach (var item in ComponentGridList.Children.Cast<EntityComponentExpander>())
            {
                foreach (var element in item.ComponentList.Children)
                {
                    if (element is DataTypeEditors.BaseEditor)
                        ((DataTypeEditors.BaseEditor)element).UpdateValues(IsActive);
                }
            }
        }

        private async void UpdateNamesTicker()
        {
            while (true)
            {
                await Task.Delay(500);

                if (sceneInstance == null)
                    continue;

                await Dispatcher.UIThread.InvokeAsync(() => UpdateTreeNames(Entities));
            }
        }

        private void UpdateTreeNames(IEnumerable<EntityTreeItem> items)
        {
            foreach (var item in items)
            {
                item.RefreshProperties();
                UpdateTreeNames(item.Children);
            }
        }

        public SceneItem FindSceneInTree(IEnumerable<SceneItem> collection, Scene scene)
        {
            foreach (var s in collection)
            {
                if (s.scene == scene)
                    return s;
            }
            return null;
        }

        private UserControl GetEditorForProperty(EntityComponent component, ComponentPropertyItem property)
        {
            var type = property.PropertyType;

            if (property.PropertyType.IsEnum)
                return new DataTypeEditors.EnumEditor(component, property);
            else if (type == typeof(int))
                return new DataTypeEditors.Int32Editor(component, property);
            else if (type == typeof(float))
                return new DataTypeEditors.SingleEditor(component, property);
            else if (type == typeof(bool))
                return new DataTypeEditors.BooleanEditor(component, property);
            else if (type == typeof(Stride.Core.Mathematics.Vector3))
                return new DataTypeEditors.Vector3Editor(component, property);
            else if (type == typeof(Stride.Core.Mathematics.Vector2))
                return new DataTypeEditors.Vector2Editor(component, property);
            else if (type == typeof(Stride.Core.Mathematics.Quaternion))
            {
                if (component.GetType().Name == "TransformComponent" && property.Name == "Rotation")
                    return new DataTypeEditors.RotationEditor(component, property);
                else
                    return new DataTypeEditors.QuaternionEditor(component, property);
            }
            else
                return new DataTypeEditors.UnsupportedEditor(component, property);
        }

        private EntityComponentInfo GetEntityComponentInfo(Type type)
        {
            if (componentInfos.ContainsKey(type))
                return componentInfos[type];

            var info = new EntityComponentInfo(type);
            componentInfos.Add(type, info);

            return info;
        }


        #endregion Methods
    }
}
