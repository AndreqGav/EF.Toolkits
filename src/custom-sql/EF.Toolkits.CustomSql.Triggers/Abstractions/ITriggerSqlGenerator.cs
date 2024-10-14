using Toolkits.Triggers.Models;

namespace Toolkits.Triggers.Abstractions
{
    public interface ITriggerSqlGenerator
    {
        string GenerateCreateTriggerSql(TriggerObject trigger);

        string GenerateDeleteTriggerSql(TriggerObject trigger);
    }
}