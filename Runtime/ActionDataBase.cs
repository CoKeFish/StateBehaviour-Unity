#if STATE_BEHAVIOR_ENABLED
using Sirenix.OdinInspector;

namespace Marmary.StateBehavior.Runtime
{
    /// <summary>
    ///     Base <see cref="SerializedScriptableObject" /> used to store configuration for behaviour actions.
    /// </summary>
    public abstract class ActionDataBase : SerializedScriptableObject
    {
    }
}
#endif