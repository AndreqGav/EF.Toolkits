﻿using System;
using System.Text;
using EF.Toolkits.CustomSql.Triggers.Abstractions;
using EF.Toolkits.CustomSql.Triggers.Enums;
using EF.Toolkits.CustomSql.Triggers.Models;

namespace EF.Toolkits.CustomSql.Triggers.Postgresql
{
    public class PostgreSqlTriggerSqlGenerator : ITriggerSqlGenerator
    {
        public string GenerateCreateTriggerSql(TriggerObject trigger)
        {
            var name = trigger.Name;

            var builder = new StringBuilder();
            builder.AppendLine($"CREATE FUNCTION {name}() RETURNS trigger as ${name}$");
            builder.AppendLine("BEGIN");
            builder.AppendLine($"{trigger.Body}");
            builder.AppendLine(GetResultSql(trigger.Operation));
            builder.AppendLine("END;");
            builder.AppendLine($"${name}$ LANGUAGE plpgsql;");

            builder.AppendLine(string.Empty);

            builder.AppendLine($"CREATE TRIGGER {name} {TimeToSql(trigger.Time)} {OperationToSql(trigger.Operation)}");
            builder.AppendLine($"ON {trigger.Table}");
            builder.AppendLine($"FOR EACH ROW EXECUTE PROCEDURE {name}();");

            return builder.ToString();
        }

        public string GenerateDeleteTriggerSql(TriggerObject trigger)
        {
            var name = trigger.Name;

            var builder = new StringBuilder();
            builder.Append($"DROP FUNCTION {name}() CASCADE;");

            return builder.ToString();
        }

        private static string GetResultSql(TriggerOperationEnum operation)
        {
            return operation switch
            {
                TriggerOperationEnum.Insert => "RETURN NEW;",
                TriggerOperationEnum.Update => "RETURN NEW;",
                TriggerOperationEnum.InsertOrUpdate => "RETURN NEW;",
                TriggerOperationEnum.Delete => "RETURN OLD;",
                _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
            };
        }

        private static string TimeToSql(TriggerTimeEnum time)
        {
            return time switch
            {
                TriggerTimeEnum.Before => "BEFORE",
                TriggerTimeEnum.After => "AFTER",
                TriggerTimeEnum.Instead => "INSTEAD OF",
                _ => throw new ArgumentOutOfRangeException(nameof(time), time, null)
            };
        }

        private static string OperationToSql(TriggerOperationEnum operation)
        {
            return operation switch
            {
                TriggerOperationEnum.Insert => "INSERT",
                TriggerOperationEnum.Update => "UPDATE",
                TriggerOperationEnum.Delete => "DELETE",
                TriggerOperationEnum.InsertOrUpdate => "INSERT OR UPDATE",
                _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
            };
        }
    }
}