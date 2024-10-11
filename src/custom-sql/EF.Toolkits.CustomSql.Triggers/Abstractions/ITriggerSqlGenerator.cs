using EF.Toolkits.CustomSql.Triggers.Models;

namespace EF.Toolkits.CustomSql.Triggers.Abstractions
{
    public interface ITriggerSqlGenerator
    {
        string GenerateCreateTriggerSql(TriggerObject trigger);

        string GenerateDeleteTriggerSql(TriggerObject trigger);
    }
}