using EF.Toolkits.CustomSql.Triggers.Constants;
using EF.Toolkits.CustomSql.Triggers.Enums;
using EF.Toolkits.CustomSql.Triggers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EF.Toolkits.CustomSql.Triggers
{
    public static class TriggersExtensions
    {
        /// <summary>
        /// Триггер перед вставкой.
        /// </summary>
        public static void BeforeInsert<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Insert, TriggerTimeEnum.Before, body);
        }

        /// <summary>
        /// Триггер после вставки.
        /// </summary>
        public static void AfterInsert<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Insert, TriggerTimeEnum.After, body);
        }

        /// <summary>
        /// Триггер вместо вставки.
        /// </summary>
        public static void InsteadInsert<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Insert, TriggerTimeEnum.Instead, body);
        }

        /// <summary>
        /// Триггер перед обновлением.
        /// </summary>
        public static void BeforeUpdate<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Update, TriggerTimeEnum.Before, body);
        }

        /// <summary>
        /// Триггер после обновления.
        /// </summary>
        public static void AfterUpdate<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Update, TriggerTimeEnum.After, body);
        }

        /// <summary>
        /// Триггер вместо обновления.
        /// </summary>
        public static void InsteadUpdate<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Update, TriggerTimeEnum.Instead, body);
        }

        /// <summary>
        /// Триггер перед вставкой или обновлении.
        /// </summary>
        public static void BeforeInsertOrUpdate<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.InsertOrUpdate, TriggerTimeEnum.Before,
                body);
        }

        /// <summary>
        /// Триггер после вставки или обновлении.
        /// </summary>
        public static void AfterInsertOrUpdate<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.InsertOrUpdate, TriggerTimeEnum.After,
                body);
        }

        /// <summary>
        /// Триггер вместо вставки или обновлении.
        /// </summary>
        public static void InsteadInsertOrUpdate<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder,
            string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.InsertOrUpdate, TriggerTimeEnum.Instead,
                body);
        }

        /// <summary>
        /// Триггер перед удалением.
        /// </summary>
        public static void BeforeDelete<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Delete, TriggerTimeEnum.Before, body);
        }

        /// <summary>
        /// Триггер после удаления.
        /// </summary>
        public static void AfterDelete<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Delete, TriggerTimeEnum.After, body);
        }

        /// <summary>
        /// Триггер вместо удаления.
        /// </summary>
        public static void InsteadDelete<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Delete, TriggerTimeEnum.Instead, body);
        }

        private static void AddTriggerAnnotation<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder,
            string name,
            TriggerOperationEnum operation, TriggerTimeEnum time, string body)
            where TEntity : class
        {
            var table = entityTypeBuilder.Metadata.GetTableName();

            var trigger = new TriggerObject
            {
                Name = name,
                Table = table,
                Operation = operation,
                Time = time,
                Body = body
            };

            entityTypeBuilder.AddTriggerAnnotation(trigger);
        }

        private static void AddTriggerAnnotation<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder,
            TriggerObject trigger) where TEntity : class
        {
            entityTypeBuilder.HasAnnotation($"{SqlTriggerConstants.Trigger}_{trigger.GetHashCode()}", trigger);
        }
    }
}