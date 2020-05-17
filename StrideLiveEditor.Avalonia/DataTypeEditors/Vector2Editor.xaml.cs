using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Stride.Core.Mathematics;
using Stride.Engine;

namespace StrideLiveEditor.Avalonia.DataTypeEditors
{
    public class Vector2Editor : BaseEditor
    {
        public TextBlock PropertyName => this.FindControl<TextBlock>("PropertyName");
        public NumericUpDown X => this.FindControl<NumericUpDown>("X");
        public NumericUpDown Y => this.FindControl<NumericUpDown>("Y");

        public Vector2Editor()
        {
            this.InitializeComponent();
        }

        public Vector2Editor(EntityComponent component, ComponentPropertyItem property)
            : base(component, property)
        {
            InitializeComponent();

            PropertyName.Text = property.Name;
            UpdateValues(false);

            AddNumericBoxEvents(OnValueChanged, X, Y);
        }

        private void OnValueChanged()
        {
            var v = new Vector2(GetFloat(X.Value), GetFloat(Y.Value));
            ComponentProperty.SetValue(Component, v);
        }

        public override void UpdateValues(bool editorWindowIsActive)
        {
            var value = (Vector2)ComponentProperty.GetValue(Component);

            if ((!editorWindowIsActive || !X.IsFocused) && GetFloat(X.Value) != value.X)
                X.Value = value.X;
            if ((!editorWindowIsActive || !Y.IsFocused) && GetFloat(Y.Value) != value.Y)
                Y.Value = value.Y;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
