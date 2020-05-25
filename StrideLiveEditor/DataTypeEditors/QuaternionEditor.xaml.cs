using Stride.Core.Mathematics;
using Stride.Engine;
using System.Windows.Controls;

namespace StrideLiveEditor.DataTypeEditors
{
    public partial class QuaternionEditor : BaseEditor
    {
        public QuaternionEditor(EntityComponent component, ComponentPropertyItem property)
            : base(component, property)
        {
            InitializeComponent();
            
            PropertyName.Text = property.Name;
            UpdateValues(false);

            AddNumericBoxEvents(OnValueChanged, X, Y, Z, W);
        }

        private void OnValueChanged()
        {
            var q = new Quaternion(GetFloat(X.Value), GetFloat(Y.Value), GetFloat(Z.Value), GetFloat(W.Value));
            ComponentProperty.SetValue(Component, q);
        }
        
        public override void UpdateValues(bool editorWindowIsActive)
        {
            var value = (Quaternion)ComponentProperty.GetValue(Component);

            var xStr = value.X.ToString();
            var yStr = value.Y.ToString();
            var zStr = value.Z.ToString();
            var wStr = value.W.ToString();

            if ((!editorWindowIsActive || !X.IsFocused) && GetFloat(X.Value) != value.X)
                X.Value = value.X;
            if ((!editorWindowIsActive || !Y.IsFocused) && GetFloat(Y.Value) != value.Y)
                Y.Value = value.Y;
            if ((!editorWindowIsActive || !Z.IsFocused) && GetFloat(Z.Value) != value.Z)
                Z.Value = value.Z;
            if ((!editorWindowIsActive || !W.IsFocused) && GetFloat(W.Value) != value.W)
                W.Value = value.W;
        }
    }
}
