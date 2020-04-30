using Stride.Engine;
using System.Windows.Controls;

namespace StrideLiveEditor.DataTypeEditors
{
    public partial class SingleEditor : BaseEditor
    {
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
    }
}
