using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Toolkits.CustomSql.Helpers;

namespace Toolkits.CustomSql.Conventions
{
    public class DbFunctionSqlConvention : IModelFinalizingConvention
    {

        public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
        {
            // Удяляем аннотации у функций и добавляем их к модели т.к. функции не отслеживаются снапшотом миграции.
            foreach (var dbFunction in modelBuilder.Metadata.GetDbFunctions())
            {
                var annotations = dbFunction.GetAnnotations()
                    .Where(RelationalModelHelper.IsCustomSqlAnnotation)
                    .ToList();

                foreach (var annotation in annotations)
                {
                    dbFunction.RemoveAnnotation(annotation.Name);

                    modelBuilder.HasAnnotation(annotation.Name, annotation.Value);
                }
            }
        }
    }
}