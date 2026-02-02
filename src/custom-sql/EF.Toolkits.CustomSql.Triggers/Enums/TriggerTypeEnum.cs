namespace Toolkits.Triggers.Enums
{
    public enum TriggerTypeEnum
    {
        // Обычный trigger
        Regular,

        // CONSTRAINT TRIGGER
        ConstraintNotDeferrable,

        ConstraintDeferrableInitiallyImmediate,

        ConstraintDeferrableInitiallyDeferred
    }
}