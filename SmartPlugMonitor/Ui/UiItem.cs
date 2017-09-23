using System;

namespace SmartPlugMonitor.Ui
{
    public class UiItem
    {
        private object value;

        public UiItem (string name, Type type, object value)
        {
            this.Name = name;
            this.Type = type;
            this.Value = value;
        }

        public string Name { get; private set; }

        public Type Type { get; private set; }

        public object Value {
            get { return value; }
            set {
                if (!value.GetType ().Equals (Type)) {
                    throw new InvalidCastException ($"{value} is not of type {Type}");
                }
                this.value = value;
            }
        }

        public T GetValue<T> ()
        {
            if (!(Value is T)) {
                throw new InvalidCastException ($"{value} is not of type {Type}");
            }
            return (T)Value;
        }
    }
}
