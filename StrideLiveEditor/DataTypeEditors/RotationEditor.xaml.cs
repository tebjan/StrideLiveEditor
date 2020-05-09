using Stride.Core.Mathematics;
using Stride.Engine;
using System;
using System.Windows.Controls;

namespace StrideLiveEditor.DataTypeEditors
{
    public partial class RotationEditor : BaseEditor
    {
        public RotationEditor(EntityComponent component, ComponentPropertyItem property)
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
            var q = GetQuaternionRotation(v);

            ComponentProperty.SetValue(Component, q);
        }
        
        public override void UpdateValues(bool editorWindowIsActive)
        {
            var quaternion = (Quaternion)ComponentProperty.GetValue(Component);
            var value = GetEulerRotation(quaternion);
            
            if ((!editorWindowIsActive || !X.IsFocused) && GetFloat(X.Value) != value.X)
                X.Value = value.X;
            if ((!editorWindowIsActive || !Y.IsFocused) && GetFloat(Y.Value) != value.Y)
                Y.Value = value.Y;
            if ((!editorWindowIsActive || !Z.IsFocused) && GetFloat(Z.Value) != value.Z)
                Z.Value = value.Z;
        }
        
        private Vector3 GetEulerRotation(Quaternion rotation)
        {
            Vector3 rotationEuler;
            
            float xx = rotation.X * rotation.X;
            float yy = rotation.Y * rotation.Y;
            float zz = rotation.Z * rotation.Z;
            float xy = rotation.X * rotation.Y;
            float zw = rotation.Z * rotation.W;
            float zx = rotation.Z * rotation.X;
            float yw = rotation.Y * rotation.W;
            float yz = rotation.Y * rotation.Z;
            float xw = rotation.X * rotation.W;

            rotationEuler.Y = (float)Math.Asin(2.0f * (yw - zx));
            double test = Math.Cos(rotationEuler.Y);
            if (test > 1e-6f)
            {
                rotationEuler.Z = (float)Math.Atan2(2.0f * (xy + zw), 1.0f - (2.0f * (yy + zz)));
                rotationEuler.X = (float)Math.Atan2(2.0f * (yz + xw), 1.0f - (2.0f * (yy + xx)));
            }
            else
            {
                rotationEuler.Z = (float)Math.Atan2(2.0f * (zw - xy), 2.0f * (zx + yw));
                rotationEuler.X = 0.0f;
            }
            return rotationEuler;
        }

        private Quaternion GetQuaternionRotation(Vector3 rotation)
        {
            // Equilvalent to:
            //  Quaternion quatX, quatY, quatZ;
            //  
            //  Quaternion.RotationX(value.X, out quatX);
            //  Quaternion.RotationY(value.Y, out quatY);
            //  Quaternion.RotationZ(value.Z, out quatZ);
            //  
            //  rotation = quatX * quatY * quatZ;

            var halfAngles = rotation * 0.5f;

            var fSinX = (float)Math.Sin(halfAngles.X);
            var fCosX = (float)Math.Cos(halfAngles.X);
            var fSinY = (float)Math.Sin(halfAngles.Y);
            var fCosY = (float)Math.Cos(halfAngles.Y);
            var fSinZ = (float)Math.Sin(halfAngles.Z);
            var fCosZ = (float)Math.Cos(halfAngles.Z);

            var fCosXY = fCosX * fCosY;
            var fSinXY = fSinX * fSinY;

            Quaternion q;

            q.X = fSinX * fCosY * fCosZ - fSinZ * fSinY * fCosX;
            q.Y = fSinY * fCosX * fCosZ + fSinZ * fSinX * fCosY;
            q.Z = fSinZ * fCosXY - fSinXY * fCosZ;
            q.W = fCosZ * fCosXY + fSinXY * fSinZ;

            return q;
        }
    }
}
