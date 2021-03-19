using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using Stride.Engine;

namespace StrideLiveEditor.Avalonia.DataTypeEditors
{
    public abstract class BaseEditor : UserControl
    {
        public EntityComponent Component { get; private set; }
        public ComponentPropertyItem ComponentProperty { get; private set; }

        public BaseEditor() { }

        public BaseEditor(EntityComponent component, ComponentPropertyItem property)
        {
            Component = component;
            ComponentProperty = property;
        }

        protected void AddTextBoxEvents(params TextBox[] boxes)
        {
            foreach (var box in boxes)
            {
                box.GotFocus += OnGotFocus;
                box.LostFocus += OnLostFocus;
                //box.KeyDown += OnKeyDown;
            }
        }

        protected void AddNumericBoxEvents(Action valueChangedAction, params NumericUpDown[] boxes)
        {
            foreach (var box in boxes)
            {
                box.GotFocus += OnGotFocus;
                box.LostFocus += OnLostFocus;
                //box.KeyDown += OnKeyDown;
                box.ValueChanged += (s, a) => valueChangedAction();
            }
        }

        protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is Control element)
                {
                    // TODO move focus
                    //element.MoveFocus(new System.Windows.Input.TraversalRequest(FocusNavigationDirection.Next));
                }
            }
        }

        protected void OnGotFocus(object sender, GotFocusEventArgs e)
        {
            if (sender is NumericUpDown numericUpDown)
            {
                var textBox = numericUpDown.GetVisualDescendants().OfType<TextBox>().FirstOrDefault();
                textBox.SelectionStart = 0;
                textBox.SelectionEnd = textBox.Text.Length > 0 ? textBox.Text.Length : 0;
            }
            else if (sender is TextBox textBox)
            {
                textBox.SelectionStart = 0;
                textBox.SelectionEnd = textBox.Text.Length > 0 ? textBox.Text.Length : 0;
            }
        }

        private void OnLostFocus(object sender, global::Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (sender is NumericUpDown numericUpDown)
            {
                var textBox = numericUpDown.GetVisualDescendants().OfType<TextBox>().FirstOrDefault();
                textBox.SelectionStart = 0;
                textBox.SelectionEnd = 0;
            }
            else if (sender is TextBox textBox)
            {
                textBox.SelectionStart = 0;
                textBox.SelectionEnd = 0;
            }
        }

        protected float GetFloat(double? value)
        {
            return (float)value.GetValueOrDefault(0);
        }

        protected int GetInt(double? value)
        {
            return (int)value.GetValueOrDefault(0);
        }

        public abstract void UpdateValues(bool editorWindowIsActive);
    }
}
