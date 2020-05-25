using Stride.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StrideLiveEditor
{
    public class EntityComponentInfo
    {
        public Type ComponentType { get; private set; }
        public DataMemberAttribute DataMember { get; private set; }
        public DisplayAttribute Display { get; private set; }

        public List<ComponentPropertyItem> Properties { get; private set; }

        public string Name
        {
            get
            {
                return (Display != null && !string.IsNullOrWhiteSpace(Display.Name) ? Display.Name : ComponentType.Name);
            }
        }

        public EntityComponentInfo(Type componentType)
        {
            ComponentType = componentType;

            DataMember = componentType.GetCustomAttribute<DataMemberAttribute>(false);
            Display = componentType.GetCustomAttribute<DisplayAttribute>(false);

            Properties = new List<ComponentPropertyItem>();

            Properties.AddRange(componentType.GetProperties()
                .Where(p => ShowProperty(p))
                .Select(p => new ComponentPropertyItem(p)));

            Properties.AddRange(componentType.GetFields()
                .Where(p => ShowProperty(p))
                .Select(p => new ComponentPropertyItem(p)));
        }

        private bool ShowProperty(PropertyInfo propertyInfo)
        {
            // Never show DataMemberIgnore
            if (propertyInfo.GetCustomAttribute<DataMemberIgnoreAttribute>() != null)
                return false;

            // Always show DataMember
            if (propertyInfo.GetCustomAttribute<DataMemberAttribute>() != null)
                return true;

            // No custom attributes, only show if can both get and set.
            return propertyInfo.GetSetMethod(false) != null && propertyInfo.GetGetMethod(false) != null;
        }

        private bool ShowProperty(FieldInfo fieldInfo)
        {
            // Never show DataMemberIgnore
            if (fieldInfo.GetCustomAttribute<DataMemberIgnoreAttribute>() != null)
                return false;

            // Always show DataMember
            if (fieldInfo.GetCustomAttribute<DataMemberAttribute>() != null)
                return true;

            // No custom attributes, only show if public and not static.
            return !fieldInfo.IsStatic && fieldInfo.IsPublic;
        }
    }
}
