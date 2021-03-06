using System.Collections.ObjectModel;

namespace Nevermore
{
    public class ParameterDefault
    {
        readonly Parameter parameter;
        readonly object defaultValue;

        public ParameterDefault(Parameter parameter, object defaultValue)
        {
            this.parameter = parameter;
            this.defaultValue = defaultValue;
        }

        public string ParameterName => parameter.ParameterName;
        public string GenerateSql()
        {
            switch (defaultValue)
            {
                case string s:
                    return $"'{s}'";
                case bool b:
                    return b ? "1" : "0";
                default:
                    return defaultValue.ToString();
            }
        }
    }

    public class ParameterDefaults : KeyedCollection<string, ParameterDefault>
    {
        public ParameterDefaults()
        {
        }

        public ParameterDefaults(ParameterDefaults parameterDefaults)
        {
            AddRange(parameterDefaults);
        }

        public ParameterDefaults(params ParameterDefaults[] parameterDefaults)
        {
            foreach (var defaultsCollection in parameterDefaults)
            {
                AddRange(defaultsCollection);
            }
        }

        protected override string GetKeyForItem(ParameterDefault item) => item.ParameterName;

        public void AddRange(ParameterDefaults defaults)
        {
            foreach (var def in defaults)
                Add(def);
        }
    }
}