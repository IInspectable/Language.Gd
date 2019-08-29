using System;
using System.Collections.Generic;

namespace Pharmatechnik.Apotheke.XTplus.Framework.Tools.Generators.GuiModelGenerator {
    public struct GuiPropertyInfo {
        private bool _required;
        private string _name;

        public bool Required {
            get { return _required; }
            internal set { _required = value; }
        }

        public string Name {
            get { return _name; }
            internal set { _name = value; }
        }
    }

    /// <summary>
    /// Beginnt der Name der Eigenschaft mit einem *, gilt diese als erforderlich. (z.B. *Text).
    /// <para>
    /// Wird dem Namen ein # vorangestellt, wird die Eigenschaft ignoriert, also quasi auskommentiert.
    /// </para>
    /// <para>
    /// Wird nur der Name der Eigenschaft angegeben, gilt diese also optional.
    /// </para>
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple=true)]
    public class SupportedPropertiesAttribute : Attribute {
        protected GuiPropertyInfo[] _properties;

        public SupportedPropertiesAttribute(string nameList) {
            List<GuiPropertyInfo> properties = new List<GuiPropertyInfo>();
            string[] props = nameList.Split(',');
            foreach (string p in props) {
                string property = p.Trim();
                GuiPropertyInfo pi = new GuiPropertyInfo();
                if (property.StartsWith("*")) {
                    pi.Required = true;
                    property = property.Substring(1).Trim();
                }
                if (property.StartsWith("#")) {
                    continue;
                }
                
                pi.Name = property;
                properties.Add(pi);
            }
            _properties = properties.ToArray();
        }

        public GuiPropertyInfo[] Properties {
            get { return _properties; }
        }
    }

    [AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = true)]
    public class SupportedControlsAttribute : SupportedPropertiesAttribute
    {

        public SupportedControlsAttribute(string nameList)
            : base(nameList)
        {

        }
    }

    [AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = true)]
    public class SupportedKeyWordsAttribute : SupportedPropertiesAttribute
    {

        public SupportedKeyWordsAttribute(string nameList)
            : base(nameList)
        {

        }
    }
        
}
