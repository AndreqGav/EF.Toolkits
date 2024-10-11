using System;
using System.Collections.Generic;

namespace EF.Toolkits.AutoComments.Helpers
{
    /// <summary>
    /// Хелпер для C# типов.
    /// </summary>
    public class TypeHelper
    {
        /// <summary>
        /// Получить все базовые типы.
        /// </summary>
        public static IEnumerable<Type> GetParentTypes(Type type)
        {
            // is there any base type?
            if (type == null)
            {
                yield break;
            }

            // return all inherited types
            var currentType = type;
            while (currentType != null)
            {
                yield return currentType;
                currentType = currentType.BaseType;
            }

            // return all implemented or inherited interfaces
            foreach (var i in type.GetInterfaces())
            {
                yield return i;
            }
        }
    }
}