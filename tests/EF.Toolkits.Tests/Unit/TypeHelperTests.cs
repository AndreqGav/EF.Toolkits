using System;
using System.Linq;
using Toolkits.AutoComments.Helpers;
using Xunit;

namespace EF.Toolkits.Tests.Unit
{
    public class TypeHelperTests
    {
        [Fact]
        public void GetParentTypes_Should_ReturnEmpty_WhenTypeIsNull()
        {
            // Arrange
            Type type = null;

            // Act
            var result = TypeHelper.GetParentTypes(type);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void GetParentTypes_Should_ReturnOnlySelf_ForObjectType()
        {
            // Arrange
            var type = typeof(object);

            // Act
            var result = TypeHelper.GetParentTypes(type).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal(typeof(object), result[0]);
        }

        [Fact]
        public void GetParentTypes_Should_IncludeFullBaseChain_ForValueType()
        {
            // Arrange
            var type = typeof(int);

            // Act
            var result = TypeHelper.GetParentTypes(type).ToList();

            // Assert — int → ValueType → object
            Assert.Contains(typeof(int), result);
            Assert.Contains(typeof(ValueType), result);
            Assert.Contains(typeof(object), result);
        }

        [Fact]
        public void GetParentTypes_Should_IncludeImplementedInterfaces()
        {
            // Arrange
            var type = typeof(string);

            // Act
            var result = TypeHelper.GetParentTypes(type).ToList();

            // Assert — string реализует IComparable
            Assert.Contains(typeof(string), result);
            Assert.Contains(typeof(IComparable), result);
        }

        [Fact]
        public void GetParentTypes_Should_IncludeParentClassAndInterface_ForDerivedClass()
        {
            // Arrange
            var type = typeof(ChildClass);

            // Act
            var result = TypeHelper.GetParentTypes(type).ToList();

            // Assert
            Assert.Contains(typeof(ChildClass), result);
            Assert.Contains(typeof(ParentClass), result);
            Assert.Contains(typeof(IMarker), result);
        }

        [Fact]
        public void GetParentTypes_Should_ReturnSelfAsFirst()
        {
            // Arrange
            var type = typeof(ChildClass);

            // Act
            var result = TypeHelper.GetParentTypes(type).ToList();

            // Assert — сам переданный тип должен быть первым
            Assert.Equal(typeof(ChildClass), result[0]);
        }

        [Fact]
        public void GetParentTypes_Should_ReturnParentBeforeGrandparent()
        {
            // Arrange
            var type = typeof(GrandChildClass);

            // Act
            var result = TypeHelper.GetParentTypes(type).ToList();

            // Assert — родитель идёт раньше прародителя
            var childIdx  = result.IndexOf(typeof(ChildClass));
            var parentIdx = result.IndexOf(typeof(ParentClass));
            Assert.True(childIdx < parentIdx);
        }

        private interface IMarker { }
        private class ParentClass { }
        private class ChildClass : ParentClass, IMarker { }
        private class GrandChildClass : ChildClass { }
    }
}
