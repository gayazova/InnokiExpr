using System.Collections.Generic;
using Newtonsoft.Json;

namespace AccessFilter
{
    public class Parameterful
    {
        /// <summary>
        /// Параметры и их значения
        /// </summary>
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Установить значение параметра
        /// </summary>
        /// <param name="name">Наименование параметра</param>
        /// <param name="value">Значение</param>
        public void SetParameter(string name, string value)
        {
            Parameters[name] = value;
        }

        /// <summary>
        /// Получить значение параметра
        /// </summary>
        /// <param name="name">Наименование параметра</param>
        /// <returns>Значение</returns>
        public string GetParameter(string name)
        {
            if (Parameters.TryGetValue(name, out var value))
            {
                return value;
            }

            return default;
        }
    }
}
