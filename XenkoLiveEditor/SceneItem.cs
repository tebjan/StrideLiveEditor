using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xenko.Engine;

namespace XenkoLiveEditor
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
                var result = FindEntityInTree(Entities, entity.GetParent());
                if (result == null)
                    Entities.Add(treeItem);
                else
                    result.Children.Add(treeItem);
            }
            else
            {
                Entities.Add(treeItem);
            }
        }


        public void OnEntityRemoved(Entity entity)
        {

            var result = FindEntityInTree(Entities, entity.GetParent());

            if (result != null)
                result.Children.Remove(result);
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
