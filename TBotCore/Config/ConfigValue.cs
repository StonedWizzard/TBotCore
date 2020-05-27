using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TBotCore.Editor;

namespace TBotCore.Config
{
    /// <summary>
    /// Converted and ready for use config variable
    /// </summary>
    public class ConfigValue : IEditable<ConfigValue.EditebleConfigValue>
    {
        string _key;
        /// <summary>
        /// Parameters key.
        /// Note: after change this property object position and key stay same.
        /// Such changes apply only after reloading configs
        /// </summary>
        public string Key
        {
            get { return _key; }
            protected set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("ConfigValue.Key", "ConfigValue key can't be empty or null!");

                _key = value;
            }
        }

        /// <summary>
        /// Parameter value
        /// </summary>
        public object Value { get; protected set; }

        /// <summary>
        /// Displayed description of property.
        /// </summary>        
        public string Description { get; protected set; }


        /// <summary>
        /// Displayed property name.
        /// </summary>
        public string Name { get; protected set; }

        Type _valueType;
        /// <summary>
        /// Type of stored variable.
        /// Altering of this property set value to default
        /// </summary>
        public Type ValueType
        {
            get { return _valueType; }
            protected set
            {
                _valueType = value;
                Value = Convert.ChangeType(Value, _valueType);
            }
        }

        public RawData.ConfigValue.ValueFlags ValueFlags { get; protected set; }

        public ConfigValue(string key, Type t)
        {
            Key = key;
            ValueType = t;
            Value = null;
        }
        public ConfigValue(string key, string name, Type t) : this(key, t)
        {
            Name = name;
        }

        public ConfigValue(RawData.ConfigValue raw)
        {
            if (raw == null) throw new ArgumentNullException();

            Key = raw.Key;
            Value = raw.Value;
            ValueType = Type.GetType(raw.ValueType);
            ValueFlags = raw.ValueFlag;

            Name = raw.Name;
            Description = raw.Description;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }


        public void SetFlag(RawData.ConfigValue.ValueFlags flag)
        {
            if (!ValueFlags.HasFlag(flag))
                ValueFlags |= flag;
        }
        public void RemoveFlag(RawData.ConfigValue.ValueFlags flag)
        {
            if (ValueFlags.HasFlag(flag))
                ValueFlags &= ~flag;
        }

        public T GetValue<T>()
        {
            return (T)Value;
        }


        #region Editors things
        public EditebleConfigValue GetEditable()
        {
            return new EditebleConfigValue(this);
        }

        public class EditebleConfigValue : IEntityEditor<ConfigValue>
        {
            public ConfigValue EditableObject { get; private set; }

            public EditebleConfigValue(ConfigValue owner) { EditableObject = owner; }

            // editable properties
            public string Key
            {
                get => EditableObject.Key;
                set => EditableObject.Key = value;
            }
            public object Value
            {
                get => EditableObject.Value;
                set => EditableObject.Value = value;
            }
            public string Description
            {
                get => EditableObject.Description;
                set => EditableObject.Description = value;
            }
            public string Name
            {
                get => EditableObject.Name;
                set => EditableObject.Name = value;
            }
            public Type ValueType
            {
                get => EditableObject.ValueType;
                set => EditableObject.ValueType = value;
            }
            public RawData.ConfigValue.ValueFlags ValueFlags 
            {
                get => EditableObject.ValueFlags;
                set => EditableObject.ValueFlags = value;
            }


            public override string ToString()
            {
                return Name;
            }
        }
        #endregion
    }
}
