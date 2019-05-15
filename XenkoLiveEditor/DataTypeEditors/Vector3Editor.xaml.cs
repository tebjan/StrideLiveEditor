using Xenko.Core.Mathematics;
using Xenko.Engine;
using System.Windows.Controls;

namespace XenkoLiveEditor.DataTypeEditors
{
    public partial class Vector3Editor : BaseEditor
    {
        public Vector3Editor(EntityComponent component, ComponentPropertyItem property)
            : base(component, property)
        {
            InitializeComponent();
            
            PropertyName.Text = property.Name;
            UpdateValues(false);
            
            AddNumericBoxEvents(OnValueChanged, X, Y, Z);
        }

        private void OnValueChanged()
        {
            var v = new Vector3(GetFloat(X.Value), GetFloat(Y.Value), GetFloat(Z.Value));
            ComponentProperty.SetValue(Component, v);
        }
        
        public override void UpdateValues(bool editorWindowIsActive)
        {
            var value = (Vector3)ComponentProperty.GetValue(Component);
            
            if ((!editorWindowIsActive || !X.IsFocused) && GetFloat(X.Value) != value.X)
                X.Value = value.X;
            if ((!editorWindowIsActive || !Y.IsFocused) && GetFloat(Y.Value) != value.Y)
                Y.Value = value.Y;
            if ((!editorWindowIsActive || !Z.IsFocused) && GetFloat(Z.Value) != value.Z)
                Z.Value = value.Z;
        }
    }
}
