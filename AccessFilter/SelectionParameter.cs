namespace AccessFilter
{
    public class SelectionParameter
    {
        /// <summary>
        /// Название свойства
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Оператор
        /// </summary>
        public SelectionOperator Operator { get; set; }

        /// <summary>
        /// Значение
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Вложенные параметры фильтрации
        /// </summary>
        public List<SelectionParameter> Parameters { get; set; } = new List<SelectionParameter>();

        /// <summary>
        /// Режим фильтрации ссылочной коллекции
        /// </summary>
        public CollectionMode CollectionMode { get; set; }
    }
}
