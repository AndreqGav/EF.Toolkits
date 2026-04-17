using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using Toolkits.Triggers.Enums;
using Toolkits.Triggers.Models;
using Toolkits.Triggers.Postgresql;
using Xunit;

namespace EF.Toolkits.Tests.Unit
{
    public class PostgreSqlTriggerSqlGeneratorTests
    {
        private readonly PostgreSqlTriggerSqlGenerator _generator;

        public PostgreSqlTriggerSqlGeneratorTests()
        {
            _generator = new PostgreSqlTriggerSqlGenerator(new FakeSqlGenerationHelper());
        }

        private static TriggerObject Make(
            string name = "my_trigger",
            string table = "my_table",
            TriggerOperationEnum operation = TriggerOperationEnum.Insert,
            TriggerTimeEnum time = TriggerTimeEnum.Before,
            TriggerTypeEnum type = TriggerTypeEnum.Regular,
            string body = "PERFORM 1;")
            => new TriggerObject { Name = name, Table = table, Operation = operation, Time = time, Type = type, Body = body };

        // --- Определение функции ---

        [Fact]
        public void GenerateCreateTriggerSql_Should_ContainFunctionDefinition()
        {
            // Arrange
            var trigger = Make(name: "fn_test", body: "PERFORM 1;");

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert — SQL содержит объявление plpgsql-функции
            Assert.Contains("CREATE FUNCTION \"fn_test\"() RETURNS trigger", sql);
            Assert.Contains("$fn_test$", sql);
            Assert.Contains("LANGUAGE plpgsql", sql);
            Assert.Contains("PERFORM 1;", sql);
        }

        [Fact]
        public void GenerateCreateTriggerSql_Should_ContainForEachRowExecuteProcedure()
        {
            // Arrange
            var trigger = Make();

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert
            Assert.Contains("FOR EACH ROW EXECUTE PROCEDURE", sql);
        }

        // --- Возвращаемое значение зависит от операции ---

        [Fact]
        public void GenerateCreateTriggerSql_Should_ReturnNEW_ForInsertOperation()
        {
            // Arrange
            var trigger = Make(operation: TriggerOperationEnum.Insert);

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert — INSERT возвращает новую строку
            Assert.Contains("RETURN NEW;", sql);
        }

        [Fact]
        public void GenerateCreateTriggerSql_Should_ReturnNEW_ForUpdateOperation()
        {
            // Arrange
            var trigger = Make(operation: TriggerOperationEnum.Update);

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert
            Assert.Contains("RETURN NEW;", sql);
        }

        [Fact]
        public void GenerateCreateTriggerSql_Should_ReturnNEW_ForInsertOrUpdateOperation()
        {
            // Arrange
            var trigger = Make(operation: TriggerOperationEnum.InsertOrUpdate);

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert
            Assert.Contains("RETURN NEW;", sql);
        }

        [Fact]
        public void GenerateCreateTriggerSql_Should_ReturnOLD_ForDeleteOperation()
        {
            // Arrange
            var trigger = Make(operation: TriggerOperationEnum.Delete);

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert — DELETE возвращает старую строку
            Assert.Contains("RETURN OLD;", sql);
        }

        // --- Ключевые слова времени срабатывания ---

        [Fact]
        public void GenerateCreateTriggerSql_Should_ContainBEFORE_ForBeforeTime()
        {
            // Arrange
            var trigger = Make(time: TriggerTimeEnum.Before);

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert
            Assert.Contains("BEFORE", sql);
        }

        [Fact]
        public void GenerateCreateTriggerSql_Should_ContainAFTER_ForAfterTime()
        {
            // Arrange
            var trigger = Make(time: TriggerTimeEnum.After);

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert
            Assert.Contains("AFTER", sql);
        }

        [Fact]
        public void GenerateCreateTriggerSql_Should_ContainINSTEAD_OF_ForInsteadTime()
        {
            // Arrange
            var trigger = Make(time: TriggerTimeEnum.Instead);

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert
            Assert.Contains("INSTEAD OF", sql);
        }

        // --- Ключевые слова операции ---

        [Fact]
        public void GenerateCreateTriggerSql_Should_ContainBEFORE_INSERT_ForInsertOperation()
        {
            // Arrange
            var trigger = Make(operation: TriggerOperationEnum.Insert, time: TriggerTimeEnum.Before);

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert
            Assert.Contains("BEFORE INSERT", sql);
        }

        [Fact]
        public void GenerateCreateTriggerSql_Should_ContainBEFORE_UPDATE_ForUpdateOperation()
        {
            // Arrange
            var trigger = Make(operation: TriggerOperationEnum.Update, time: TriggerTimeEnum.Before);

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert
            Assert.Contains("BEFORE UPDATE", sql);
        }

        [Fact]
        public void GenerateCreateTriggerSql_Should_ContainAFTER_DELETE_ForDeleteOperation()
        {
            // Arrange
            var trigger = Make(operation: TriggerOperationEnum.Delete, time: TriggerTimeEnum.After);

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert
            Assert.Contains("AFTER DELETE", sql);
        }

        [Fact]
        public void GenerateCreateTriggerSql_Should_ContainBEFORE_INSERT_OR_UPDATE_ForInsertOrUpdateOperation()
        {
            // Arrange
            var trigger = Make(operation: TriggerOperationEnum.InsertOrUpdate, time: TriggerTimeEnum.Before);

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert
            Assert.Contains("BEFORE INSERT OR UPDATE", sql);
        }

        // --- Тип триггера ---

        [Fact]
        public void GenerateCreateTriggerSql_Should_CreateRegularTrigger_WithoutConstraintKeywords()
        {
            // Arrange
            var trigger = Make(type: TriggerTypeEnum.Regular, time: TriggerTimeEnum.After);

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert — обычный триггер: без CONSTRAINT и DEFERRABLE
            Assert.Contains("CREATE TRIGGER", sql);
            Assert.DoesNotContain("CONSTRAINT TRIGGER", sql);
            Assert.DoesNotContain("DEFERRABLE", sql);
            Assert.DoesNotContain("NOT DEFERRABLE", sql);
        }

        [Fact]
        public void GenerateCreateTriggerSql_Should_ContainCONSTRAINT_TRIGGER_And_NOT_DEFERRABLE()
        {
            // Arrange
            var trigger = Make(type: TriggerTypeEnum.ConstraintNotDeferrable, time: TriggerTimeEnum.After);

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert
            Assert.Contains("CREATE CONSTRAINT TRIGGER", sql);
            Assert.Contains("NOT DEFERRABLE", sql);
        }

        [Fact]
        public void GenerateCreateTriggerSql_Should_ContainDEFERRABLE_INITIALLY_IMMEDIATE()
        {
            // Arrange
            var trigger = Make(type: TriggerTypeEnum.ConstraintDeferrableInitiallyImmediate, time: TriggerTimeEnum.After);

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert
            Assert.Contains("CREATE CONSTRAINT TRIGGER", sql);
            Assert.Contains("DEFERRABLE INITIALLY IMMEDIATE", sql);
        }

        [Fact]
        public void GenerateCreateTriggerSql_Should_ContainDEFERRABLE_INITIALLY_DEFERRED()
        {
            // Arrange
            var trigger = Make(type: TriggerTypeEnum.ConstraintDeferrableInitiallyDeferred, time: TriggerTimeEnum.After);

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert
            Assert.Contains("CREATE CONSTRAINT TRIGGER", sql);
            Assert.Contains("DEFERRABLE INITIALLY DEFERRED", sql);
        }

        // --- Экранирование имени таблицы ---

        [Fact]
        public void GenerateCreateTriggerSql_Should_QuoteTableName()
        {
            // Arrange
            var trigger = Make(table: "Orders");

            // Act
            var sql = _generator.GenerateCreateTriggerSql(trigger);

            // Assert — имя таблицы в двойных кавычках (PostgreSQL-стиль)
            Assert.Contains("ON \"Orders\"", sql);
        }

        // --- Удаление триггера ---

        [Fact]
        public void GenerateDeleteTriggerSql_Should_GenerateDropFunctionCascade()
        {
            // Arrange
            var trigger = Make(name: "my_trigger");

            // Act
            var sql = _generator.GenerateDeleteTriggerSql(trigger);

            // Assert
            Assert.Equal("DROP FUNCTION \"my_trigger\"() CASCADE;", sql);
        }

        [Fact]
        public void GenerateDeleteTriggerSql_Should_QuoteTriggerName()
        {
            // Arrange
            var trigger = Make(name: "fn_on_insert");

            // Act
            var sql = _generator.GenerateDeleteTriggerSql(trigger);

            // Assert
            Assert.Contains("\"fn_on_insert\"", sql);
        }

        /// <summary>
        /// Тестовая заглушка ISqlGenerationHelper.
        /// Экранирует идентификаторы двойными кавычками (PostgreSQL-стиль).
        /// </summary>
        private sealed class FakeSqlGenerationHelper : ISqlGenerationHelper
        {
            public string StatementTerminator => ";";
            public string BatchTerminator => string.Empty;

            public string DelimitIdentifier(string identifier) => $"\"{identifier}\"";

            public string DelimitIdentifier(string name, string schema)
                => string.IsNullOrEmpty(schema) ? DelimitIdentifier(name) : $"\"{schema}\".\"{name}\"";

            public void DelimitIdentifier(StringBuilder builder, string identifier)
                => builder.Append(DelimitIdentifier(identifier));

            public void DelimitIdentifier(StringBuilder builder, string name, string schema)
                => builder.Append(DelimitIdentifier(name, schema));

            public string EscapeIdentifier(string identifier) => identifier.Replace("\"", "\"\"");

            public void EscapeIdentifier(StringBuilder builder, string identifier)
                => builder.Append(EscapeIdentifier(identifier));

            public string EscapeLiteral(string literal) => literal.Replace("'", "''");

            public void EscapeLiteral(StringBuilder builder, string literal)
                => builder.Append(EscapeLiteral(literal));

            public string GenerateComment(string text) => $"-- {text}";

            // Члены, добавленные в более новых версиях EF Core
            public string GenerateParameterName(string name) => $"@{name}";
            public void GenerateParameterName(StringBuilder builder, string name) => builder.Append($"@{name}");
            public string GenerateParameterNamePlaceholder(string name) => $"@{name}";
            public void GenerateParameterNamePlaceholder(StringBuilder builder, string name) => builder.Append($"@{name}");
            public string DelimitJsonPathElement(string element) => element;
            public string GenerateCreateSavepointStatement(string name) => $"SAVEPOINT {name}";
            public string GenerateRollbackToSavepointStatement(string name) => $"ROLLBACK TO SAVEPOINT {name}";
            public string GenerateReleaseSavepointStatement(string name) => $"RELEASE SAVEPOINT {name}";
            public string StartTransactionStatement => "BEGIN";
            public string CommitTransactionStatement => "COMMIT";
            public string SingleLineCommentToken => "--";
        }
    }
}
