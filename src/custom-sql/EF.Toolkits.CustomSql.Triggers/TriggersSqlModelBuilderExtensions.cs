using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Toolkits.Triggers.Constants;
using Toolkits.Triggers.Enums;
using Toolkits.Triggers.Models;

namespace Toolkits.Triggers
{
    public static class TriggersExtensions
    {
        /// <summary>
        /// Триггер перед вставкой.
        /// </summary>
        public static EntityTypeBuilder<TEntity> BeforeInsert<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Insert, TriggerTimeEnum.Before, body);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Триггер после вставки.
        /// </summary>
        public static EntityTypeBuilder<TEntity> AfterInsert<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Insert, TriggerTimeEnum.After, body);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Триггер вместо вставки.
        /// </summary>
        public static EntityTypeBuilder<TEntity> InsteadInsert<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Insert, TriggerTimeEnum.Instead, body);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Триггер перед обновлением.
        /// </summary>
        public static EntityTypeBuilder<TEntity> BeforeUpdate<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Update, TriggerTimeEnum.Before, body);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Триггер после обновления.
        /// </summary>
        public static EntityTypeBuilder<TEntity> AfterUpdate<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Update, TriggerTimeEnum.After, body);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Триггер вместо обновления.
        /// </summary>
        public static EntityTypeBuilder<TEntity> InsteadUpdate<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Update, TriggerTimeEnum.Instead, body);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Триггер перед вставкой или обновлении.
        /// </summary>
        public static EntityTypeBuilder<TEntity> BeforeInsertOrUpdate<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder,
            string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.InsertOrUpdate, TriggerTimeEnum.Before, body);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Триггер после вставки или обновлении.
        /// </summary>
        public static EntityTypeBuilder<TEntity> AfterInsertOrUpdate<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder,
            string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.InsertOrUpdate, TriggerTimeEnum.After, body);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Триггер вместо вставки или обновлении.
        /// </summary>
        public static EntityTypeBuilder<TEntity> InsteadInsertOrUpdate<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder,
            string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.InsertOrUpdate, TriggerTimeEnum.Instead, body);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Триггер перед удалением.
        /// </summary>
        public static EntityTypeBuilder<TEntity> BeforeDelete<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Delete, TriggerTimeEnum.Before, body);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Триггер после удаления.
        /// </summary>
        public static EntityTypeBuilder<TEntity> AfterDelete<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Delete, TriggerTimeEnum.After, body);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Триггер вместо удаления.
        /// </summary>
        public static EntityTypeBuilder<TEntity> InsteadDelete<TEntity>(this EntityTypeBuilder<TEntity> entityTypeBuilder, string name,
            string body)
            where TEntity : class
        {
            entityTypeBuilder.AddTriggerAnnotation(name, TriggerOperationEnum.Delete, TriggerTimeEnum.Instead, body);

            return entityTypeBuilder;
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