using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stride.Engine;

namespace StrideLiveEditor.Avalonia.DataTypeEditors
{
    public class SingleEditor : BaseEditor
    {
        public TextBlock PropertyName => this.FindControl<TextBlock>("PropertyName");
        public NumericUpDown Value => this.FindControl<NumericUpDown>("Value");

        public SingleEditor()
        {
            this.InitializeComponent();
        }

        public SingleEditor(EntityComponent component, ComponentPropertyItem property)
            : base(component, property)
        {
            InitializeComponent();

            PropertyName.Text = property.Name;
            UpdateValues(false);

            AddNumericBoxEvents(OnValueChanged, Value);
        }

        private void OnValueChanged()
        {
            ComponentProperty.SetValue(Component, GetFloat(Value.Value));
        }

        public override void UpdateValues(bool editorWindowIsActive)
        {
            var value = (float)ComponentProperty.GetValue(Component);

            if ((!editorWindowIsActive || !Value.IsFocused) && GetFloat(Value.Value) != value)
                Value.Value = value;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
