using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stride.Engine;

namespace StrideLiveEditor.Avalonia.DataTypeEditors
{
    public class Int32Editor : BaseEditor
    {
        public TextBlock PropertyName => this.FindControl<TextBlock>("PropertyName");
        public NumericUpDown Value => this.FindControl<NumericUpDown>("Value");

        public Int32Editor()
        {
            this.InitializeComponent();
        }

        public Int32Editor(EntityComponent component, ComponentPropertyItem property)
            : base(component, property)
        {
            InitializeComponent();

            PropertyName.Text = property.Name;
            UpdateValues(false);

            AddNumericBoxEvents(OnValueChanged, Value);
        }

        private void OnValueChanged()
        {
            ComponentProperty.SetValue(Component, GetInt(Value.Value));
        }

        public override void UpdateValues(bool editorWindowIsActive)
        {
            var value = (int)ComponentProperty.GetValue(Component);
            if ((!editorWindowIsActive || !Value.IsFocused) && GetInt(Value.Value) != value)
                Value.Value = value;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
