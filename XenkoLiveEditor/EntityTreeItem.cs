using Stride.Core;
using Stride.Engine;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;

namespace StrideLiveEditor
{
    public class EntityTreeItem : INotifyPropertyChanged
    {

        string currentName;
        public string Name => currentName;

        public ComponentBase Entity { get; set; }

        public ObservableCollection<EntityTreeItem> Children { get; set; }

        public EntityTreeItem() { }

        public EntityTreeItem(ComponentBase entity)
        {
            Entity = entity;
            currentName = entity.Name;

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

        public void RefreshProperties()
        {
            if (Entity.Name != currentName)
            {
                currentName = Entity.Name;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Name"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
