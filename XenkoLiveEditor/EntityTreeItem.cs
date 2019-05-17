using Xenko.Core;
using Xenko.Engine;
using System.Collections.ObjectModel;
using System.Linq;


namespace XenkoLiveEditor
{
    public class EntityTreeItem
    {
        public string Name => Entity.Name;


        public ComponentBase Entity { get; set; }

        public ObservableCollection<EntityTreeItem> Children { get; set; }

        public EntityTreeItem() { }

        public EntityTreeItem(ComponentBase entity)
        {
            Entity = entity;

            if (entity.GetType() == typeof(Entity))
            {
                Entity castEntity = (Entity)entity;
                if (castEntity == null || castEntity.Transform == null || castEntity.Transform.Children == null)
                    Children = new ObservableCollection<EntityTreeItem>();
                else
                    Children = new ObservableCollection<EntityTreeItem>(castEntity.Transform.Children.Select(e => new EntityTreeItem(e.Entity)));
            }

            if (entity.GetType() == typeof(Scene))
            {
                Children = new ObservableCollection<EntityTreeItem>();
            }
        }
    }
}
