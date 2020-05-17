using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Stride.Engine;

namespace StrideLiveEditor.Avalonia.DataTypeEditors
{
    public class BooleanEditor : BaseEditor
    {
        public TextBlock PropertyName => this.FindControl<TextBlock>("PropertyName");
        public CheckBox Value => this.FindControl<CheckBox>("Value");

        public BooleanEditor()
        {
            InitializeComponent();
        }

        public BooleanEditor(EntityComponent component, ComponentPropertyItem property)
            : base(component, property)
        {
            InitializeComponent();
            PropertyName.Text = property.Name;

            UpdateValues(false);

            Value.Click += OnValueChanged;
        }

        private void OnValueChanged(object sender, RoutedEventArgs e)
        {
            ComponentProperty.SetValue(Component, Value.IsChecked);
        }

        public override void UpdateValues(bool editorWindowIsActive)
        {
            var value = (bool)ComponentProperty.GetValue(Component);

            if (Value.IsChecked != value)
                Value.IsChecked = value;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
