using Toolkits.Triggers.Enums;
using Toolkits.Triggers.Models;
using Xunit;

namespace EF.Toolkits.Tests.Unit
{
    public class TriggerObjectTests
    {
        private static TriggerObject Make(
            string name = "trigger",
            string table = "tbl",
            TriggerOperationEnum operation = TriggerOperationEnum.Insert,
            TriggerTimeEnum time = TriggerTimeEnum.Before,
            TriggerTypeEnum type = TriggerTypeEnum.Regular,
            string body = "RETURN NEW;")
            => new TriggerObject
            {
                Name = name,
                Table = table,
                Operation = operation,
                Time = time,
                Type = type,
                Body = body
            };

        [Fact]
        public void TriggerObject_Should_BeEqual_WhenAllValuesMatch()
        {
            // Arrange
            var first  = Make();
            var second = Make();

            // Act + Assert
            Assert.Equal(first, second);
        }

        [Fact]
        public void TriggerObject_Should_NotBeEqual_WhenNamesDiffer()
        {
            // Arrange
            var first  = Make(name: "trigger_a");
            var second = Make(name: "trigger_b");

            // Act + Assert
            Assert.NotEqual(first, second);
        }

        [Fact]
        public void TriggerObject_Should_NotBeEqual_WhenTablesDiffer()
        {
            // Arrange
            var first  = Make(table: "table_one");
            var second = Make(table: "table_two");

            // Act + Assert
            Assert.NotEqual(first, second);
        }

        [Fact]
        public void TriggerObject_Should_NotBeEqual_WhenOperationsDiffer()
        {
            // Arrange
            var first  = Make(operation: TriggerOperationEnum.Insert);
            var second = Make(operation: TriggerOperationEnum.Delete);

            // Act + Assert
            Assert.NotEqual(first, second);
        }

        [Fact]
        public void TriggerObject_Should_NotBeEqual_WhenTimesDiffer()
        {
            // Arrange
            var first  = Make(time: TriggerTimeEnum.Before);
            var second = Make(time: TriggerTimeEnum.After);

            // Act + Assert
            Assert.NotEqual(first, second);
        }

        [Fact]
        public void TriggerObject_Should_NotBeEqual_WhenTypesDiffer()
        {
            // Arrange
            var first  = Make(type: TriggerTypeEnum.Regular);
            var second = Make(type: TriggerTypeEnum.ConstraintNotDeferrable);

            // Act + Assert
            Assert.NotEqual(first, second);
        }

        [Fact]
        public void TriggerObject_Should_NotBeEqual_WhenBodiesDiffer()
        {
            // Arrange
            var first  = Make(body: "PERFORM action_a();");
            var second = Make(body: "PERFORM action_b();");

            // Act + Assert
            Assert.NotEqual(first, second);
        }

        [Fact]
        public void TriggerObject_Should_NotBeEqual_ToNull()
        {
            // Arrange
            var trigger = Make();

            // Act
            var result = trigger.Equals((TriggerObject)null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TriggerObject_Should_NotBeEqual_ToNullObject()
        {
            // Arrange
            var trigger = Make();

            // Act
            var result = trigger.Equals((object)null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TriggerObject_Should_ProduceSameHashCode_WhenValuesMatch()
        {
            // Arrange
            var first  = Make();
            var second = Make();

            // Act
            var hashFirst  = first.GetHashCode();
            var hashSecond = second.GetHashCode();

            // Assert
            Assert.Equal(hashFirst, hashSecond);
        }

        [Fact]
        public void TriggerObject_Should_ProduceDifferentHashCode_WhenBodiesDiffer()
        {
            // Arrange
            var first  = Make(body: "PERFORM action_a();");
            var second = Make(body: "PERFORM action_b();");

            // Act
            var hashFirst  = first.GetHashCode();
            var hashSecond = second.GetHashCode();

            // Assert
            Assert.NotEqual(hashFirst, hashSecond);
        }

        [Fact]
        public void TriggerObject_Should_ProduceDifferentHashCode_WhenNamesDiffer()
        {
            // Arrange
            var first  = Make(name: "trigger_x");
            var second = Make(name: "trigger_y");

            // Act
            var hashFirst  = first.GetHashCode();
            var hashSecond = second.GetHashCode();

            // Assert
            Assert.NotEqual(hashFirst, hashSecond);
        }

        [Fact]
        public void TriggerObject_Should_ProduceDifferentHashCodes_ForAllOperationValues()
        {
            // Arrange — все значения TriggerOperationEnum
            var insert      = Make(operation: TriggerOperationEnum.Insert).GetHashCode();
            var update      = Make(operation: TriggerOperationEnum.Update).GetHashCode();
            var delete      = Make(operation: TriggerOperationEnum.Delete).GetHashCode();
            var insertOrUpd = Make(operation: TriggerOperationEnum.InsertOrUpdate).GetHashCode();

            // Assert — каждая операция даёт уникальный хэш
            Assert.NotEqual(insert, update);
            Assert.NotEqual(insert, delete);
            Assert.NotEqual(insert, insertOrUpd);
        }
    }
}
