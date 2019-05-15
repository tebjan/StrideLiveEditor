using MahApps.Metro.Controls;
using Xenko.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace XenkoLiveEditor.DataTypeEditors
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
                box.KeyDown += OnKeyDown;
            }
        }

        protected void AddNumericBoxEvents(Action valueChangedAction, params NumericUpDown[] boxes)
        {
            foreach (var box in boxes)
            {
                box.GotFocus += OnGotFocus;
                box.KeyDown += OnKeyDown;
                box.ValueChanged += (s, a) => valueChangedAction();
            }
        }
        
        protected void OnKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                var element = sender as FrameworkElement;
                if (element != null)
                    element.MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.Next));
            }
        }

        protected void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is NumericUpDown)
                ((NumericUpDown)sender).SelectAll();
            else if (sender is TextBox)
                ((TextBox)sender).SelectAll();
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
