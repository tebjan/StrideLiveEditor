using Stride.Engine;
using System.Windows.Controls;

namespace StrideLiveEditor.DataTypeEditors
{
    public partial class UnsupportedEditor : BaseEditor
    {
        public UnsupportedEditor(EntityComponent component, ComponentPropertyItem property)
            : base(component, property)
        {
            InitializeComponent();
            
            PropertyName.Text = property.Name;
            var value = property.GetValue(component);
            Value.Text = value == null ? "null" : value.ToString();
        }

        public override void UpdateValues(bool editorWindowIsActive) { }
    }
}
