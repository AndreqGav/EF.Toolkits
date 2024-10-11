using System;
using EF.Toolkits.CustomSql.Triggers.Enums;

namespace EF.Toolkits.CustomSql.Triggers.Models
{
    /// <summary>
    /// Объект аннотации по создании триггера.
    /// </summary>
    public class TriggerObject
    {
        /// <summary>
        /// Наименование триггера.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Наименование целевой таблицы.
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// Операция триггера.
        /// </summary>
        public TriggerOperationEnum Operation { get; set; }

        /// <summary>
        /// Время срабатывания триггера.
        /// </summary>
        public TriggerTimeEnum Time { get; set; }

        /// <summary>
        /// Тело триггера в SQL.
        /// </summary>
        public string Body { get; set; }

        // Переопределение метода Equals
        public override bool Equals(object obj)
        {
            return Equals(obj as TriggerObject);
        }

        public bool Equals(TriggerObject other)
        {
            if (other is null)
                return false;

            return string.Equals(Name, other.Name) &&
                   string.Equals(Table, other.Table) &&
                   Operation == other.Operation &&
                   Time == other.Time &&
                   string.Equals(Body, other.Body);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Table, Operation, Time, Body);
        }
    }
}