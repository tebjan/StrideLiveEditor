using Stride.Engine;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace StrideLiveEditor.DataTypeEditors
{
    /// <summary>
    /// Interaction logic for EnumEditor.xaml
    /// </summary>
    public partial class EnumEditor : UserControl
    {
        public EntityComponent Component { get; private set; }
        public ComponentPropertyItem ComponentProperty { get; private set; }

        bool pauseEvents = true;

        public EnumEditor(EntityComponent component, ComponentPropertyItem property)
        {
            InitializeComponent();

            Component = component;
            ComponentProperty = property;

            PropertyName.Text = property.Name;
            
            Value.ItemsSource = Enum.GetNames(property.PropertyType);

            UpdateValues(false);

            Value.SelectionChanged += Value_SelectionChanged;
        }

        public void UpdateValues(bool editorWindowIsActive)
        {
            pauseEvents = true;

            var value = ComponentProperty.GetValue(Component);
            var selectedName = Enum.GetName(ComponentProperty.PropertyType, value);

            if ((!editorWindowIsActive || !Value.IsFocused) && (string)Value.SelectedValue != selectedName)
            Value.SelectedValue = selectedName;

            pauseEvents = false;
        }

        private void Value_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pauseEvents)
                return;
            
            var selectedName = (string)Value.SelectedValue;
            var value = Enum.Parse(ComponentProperty.PropertyType, selectedName);

            ComponentProperty.SetValue(Component, value);
        }
    }
}
