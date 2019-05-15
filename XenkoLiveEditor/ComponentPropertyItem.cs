using System;
using System.Reflection;
using System.Text;

namespace XenkoLiveEditor
{
    public class ComponentPropertyItem
    {
        public Xenko.Core.DataMemberAttribute DataMember { get; set; }
        public Xenko.Core.DisplayAttribute Display { get; set; }
        
        public FieldInfo FieldInfo { get; set; }
        public PropertyInfo PropertyInfo { get; set; }

        public string Name
        {
            get
            {
                if (Display != null && !string.IsNullOrWhiteSpace(Display.Name))
                    return Display.Name;
                else if (FieldInfo != null)
                    return GetCleanName(FieldInfo.Name);
                else
                    return GetCleanName(PropertyInfo.Name);
            }
        }

        public Type PropertyType
        {
            get
            {
                if (FieldInfo != null)
                    return FieldInfo.FieldType;
                else
                    return PropertyInfo.PropertyType;
            }
        }

        public MemberInfo MemberInfo
        {
            get
            {
                if (FieldInfo != null)
                    return FieldInfo;
                else
                    return PropertyInfo;
            }
        }

        private string GetCleanName(string name)
        {
            // Returns camel case name with spaces.
            if (string.IsNullOrWhiteSpace(name) || name.Length < 3)
                return name;

            var sb = new StringBuilder();
            for (int i = name.Length - 1; i >= 0; i--)
            {
                sb.Insert(0, name[i]);
                if (i == 0)
                    continue;

                int c = name[i];
                bool upper = c >= 65 && c <= 90;

                if (upper)
                {
                    sb.Insert(0, ' ');
                }
            }

            return sb.ToString();
        }

        private ComponentPropertyItem(FieldInfo fieldInfo, PropertyInfo propertyInfo)
        {
            FieldInfo = fieldInfo;
            PropertyInfo = propertyInfo;

            var member = MemberInfo;
            DataMember = member.GetCustomAttribute<Xenko.Core.DataMemberAttribute>();
            Display = member.GetCustomAttribute<Xenko.Core.DisplayAttribute>();
        }

        public ComponentPropertyItem(FieldInfo fieldInfo)
            : this(fieldInfo, null)
        { }

        public ComponentPropertyItem(PropertyInfo propertyInfo)
            : this(null, propertyInfo)
        { }

        public object GetValue(object obj)
        {
            if (FieldInfo != null)
                return FieldInfo.GetValue(obj);
            else
                return PropertyInfo.GetValue(obj);
        }

        public void SetValue(object obj, object value)
        {
            if (FieldInfo != null)
                FieldInfo.SetValue(obj, value);
            else
                PropertyInfo.SetValue(obj, value);
        }
    }
}
