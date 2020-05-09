using Stride.Engine;
using System.Windows.Controls;

namespace StrideLiveEditor.DataTypeEditors
{
    public partial class Int32Editor : BaseEditor
    {
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
    }
}
