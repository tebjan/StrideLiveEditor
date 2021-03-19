using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace StrideLiveEditor.Avalonia.DataTypeEditors
{
    public class QuaternionEditor : BaseEditor
    {
        public TextBlock PropertyName => this.FindControl<TextBlock>("PropertyName");
        public NumericUpDown X => this.FindControl<NumericUpDown>("X");
        public NumericUpDown Y => this.FindControl<NumericUpDown>("Y");
        public NumericUpDown Z => this.FindControl<NumericUpDown>("Z");
        public NumericUpDown W => this.FindControl<NumericUpDown>("W");

        public QuaternionEditor()
        {
            this.InitializeComponent();
        }

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

            if ((!editorWindowIsActive || !X.IsFocused) && GetFloat(X.Value) != value.X)
                X.Value = value.X;
            if ((!editorWindowIsActive || !Y.IsFocused) && GetFloat(Y.Value) != value.Y)
                Y.Value = value.Y;
            if ((!editorWindowIsActive || !Z.IsFocused) && GetFloat(Z.Value) != value.Z)
                Z.Value = value.Z;
            if ((!editorWindowIsActive || !W.IsFocused) && GetFloat(W.Value) != value.W)
                W.Value = value.W;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
