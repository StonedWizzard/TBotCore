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
        public delegate void ConfigValueChanged(object sender, string e);
        public event ConfigValueChanged ValueChangedEvent;

        string _key;
        /// <summary>
        /// Parameters key.
        /// Note: after change this property object position and key stay same.
        /// Such changes apply only after reloading configs
        /// </summary>
        public string Key
        {
            get { return _key; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("ConfigValue.Key", "ConfigValue key can't be empty or null!");

                _key = value;
                OnConfigValueChanged("Key");
            }
        }

        object _value;
        /// <summary>
        /// Parameter value
        /// </summary>
        public object Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnConfigValueChanged("Value");
            }
        }

        string _description;
        /// <summary>
        /// Displayed description of property.
        /// </summary>        
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnConfigValueChanged("Description");
            }
        }

        string _name;
        /// <summary>
        /// Displayed property name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnConfigValueChanged("Name");
            }
        }

        Type _valueType;
        /// <summary>
        /// Type of stored variable.
        /// Altering of this property set value to default
        /// </summary>
        public Type ValueType
        {
            get { return _valueType; }
            set
            {
                _valueType = value;
                _value = Convert.ChangeType(Value, _valueType);
                OnConfigValueChanged("ValueType");
            }
        }

        public RawData.ConfigValue.ValueFlags ValueFlags { get; private set; }

        public ConfigValue(string key, Type t)
        {
            Key = key;
            ValueType = t;
            Value = null;
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


        public virtual void OnConfigValueChanged(string valueName)
        {
            ValueChangedEvent?.Invoke(this, valueName);
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
            public ConfigValue Owner { get; private set; }

            public EditebleConfigValue(ConfigValue owner) { Owner = owner; }

            // editable properties
            public string Key
            {
                get => Owner.Key;
                set => Owner.Key = value;
            }
            public object Value
            {
                get => Owner.Value;
                set => Owner.Value = value;
            }
            public string Description
            {
                get => Owner.Description;
                set => Owner.Description = value;
            }
            public string Name
            {
                get => Owner.Name;
                set => Owner.Name = value;
            }
            public Type ValueType
            {
                get => Owner.ValueType;
                set => Owner.ValueType = value;
            }
            public RawData.ConfigValue.ValueFlags ValueFlags 
            {
                get => Owner.ValueFlags;
                set => Owner.ValueFlags = value;
            }


            public override string ToString()
            {
                return Name;
            }
        }
        #endregion
    }
}
