using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stride.Engine;

namespace StrideLiveEditor.Avalonia
{
    public class EntityComponentExpander : UserControl
    {
        public Expander Root => this.FindControl<Expander>("Root");
        public StackPanel ComponentList => this.FindControl<StackPanel>("ComponentList");

        public EntityComponent Component { get; set; }

        public EntityComponentExpander() { }

        public EntityComponentExpander(EntityComponent component)
        {
            Component = component;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
