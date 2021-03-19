using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stride.Engine;

namespace StrideLiveEditor.Avalonia.DataTypeEditors
{
    public class UnsupportedEditor : BaseEditor
    {
        public TextBlock PropertyName => this.FindControl<TextBlock>("PropertyName");
        public TextBlock Value => this.FindControl<TextBlock>("Value");

        public UnsupportedEditor()
        {
            this.InitializeComponent();
        }

        public UnsupportedEditor(EntityComponent component, ComponentPropertyItem property)
    : base(component, property)
        {
            InitializeComponent();

            PropertyName.Text = property.Name;
            var value = property.GetValue(component);
            Value.Text = value == null ? "null" : value.ToString();
        }

        public override void UpdateValues(bool editorWindowIsActive) { }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
