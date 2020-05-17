using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stride.Engine;

namespace StrideLiveEditor.Avalonia.DataTypeEditors
{
    public class EnumEditor : BaseEditor
    {
        public TextBlock PropertyName => this.FindControl<TextBlock>("PropertyName");
        public ComboBox Value => this.FindControl<ComboBox>("Value");

        public EnumEditor()
        {
            this.InitializeComponent();
        }

        private bool pauseEvents = true;

        public EnumEditor(EntityComponent component, ComponentPropertyItem property) : base(component, property)
        {
            InitializeComponent();

            PropertyName.Text = property.Name;

            Value.Items = Enum.GetNames(property.PropertyType);

            UpdateValues(false);

            Value.SelectionChanged += Value_SelectionChanged;
        }

        public override void UpdateValues(bool editorWindowIsActive)
        {
            pauseEvents = true;

            var value = ComponentProperty.GetValue(Component);
            var selectedName = Enum.GetName(ComponentProperty.PropertyType, value);

            if ((!editorWindowIsActive || !Value.IsFocused) && (string)Value.SelectedItem != selectedName)
            {
                Value.SelectedItem = selectedName;
            }

            pauseEvents = false;
        }

        private void Value_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pauseEvents)
            {
                return;
            }

            var selectedName = (string)Value.SelectedItem;
            var value = Enum.Parse(ComponentProperty.PropertyType, selectedName);

            ComponentProperty.SetValue(Component, value);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
