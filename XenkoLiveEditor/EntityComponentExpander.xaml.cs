using Stride.Engine;
using System.Windows.Controls;

namespace StrideLiveEditor
{
    /// <summary>
    /// Interaction logic for EntityComponentExpander.xaml
    /// </summary>
    public partial class EntityComponentExpander : UserControl
    {
        public EntityComponent Component { get; set; }

        public EntityComponentExpander() { }

        public EntityComponentExpander(EntityComponent component)
        {
            Component = component;
            InitializeComponent();
        }
    }
}
