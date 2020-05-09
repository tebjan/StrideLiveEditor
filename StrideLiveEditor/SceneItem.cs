using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Stride.Engine;

namespace StrideLiveEditor
{
    public class SceneItem
    {
        public ObservableCollection<EntityTreeItem> Entities { get; set; }
        public EntityTreeItem TreeRoot { get; set; }
        public Scene scene;

        public SceneItem(EntityTreeItem sceneRoot)
        {
            Entities = sceneRoot.Children;
            TreeRoot = sceneRoot;

            this.scene = (Scene)sceneRoot.Entity;
        }

        public void OnEntityAdded(Entity entity)
        {
            var treeItem = new EntityTreeItem(entity);

            if (entity.Transform.Parent != null)
            {
                var parentEntity = entity.GetParent();
                var parent = FindEntityInTree(Entities, parentEntity);
                if (parent == null)
                    OnEntityAdded(parentEntity); //add non existent parent(s)

                //add child to now exiting parent
                parent = FindEntityInTree(Entities, parentEntity);
                if(!parent.Children.Any(ti => ti.Entity == entity))
                    parent.Children.Add(treeItem);
            }
            else
            {

                if (!Entities.Any(ti => ti.Entity == entity))
                    Entities.Add(treeItem);
            }
        }

        public void OnEntityRemoved(Entity entity)
        {
            var result = FindEntityInTree(Entities, entity);
            var parent = FindEntityInTree(Entities, entity.GetParent());

            if (result != null && parent != null)
                parent.Children.Remove(result);
            else
            {
                Entities.Remove(FindEntityInTree(Entities, entity));
            }
        }

        private static EntityTreeItem FindEntityInTree(IEnumerable<EntityTreeItem> collection, Entity entity)
        {
            foreach (var e in collection)
            {
                if (e.Entity == entity)
                    return e;

                var result = FindEntityInTree(e.Children, entity);

                if (result != null)
                    return result;
            }

            return null;
        }

    }
}
