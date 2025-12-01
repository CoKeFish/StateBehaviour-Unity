using Marmary.StateBehavior.Runtime.Actions;

namespace Marmary.StateBehavior.Runtime.SelectableState.Actions
{
    /// <summary>
    ///     Represents an action that animates TextAnimator_TMP components
    ///     by modifying tagged behavior parameters based on selectable states.
    /// </summary>
    /// <remarks>
    ///     This class specializes the AnimationFontAction to work with SelectableState
    ///     and AnimationFontDataSelectableAction. It enables dynamic animations
    ///     for text components in response to state transitions of selectable UI elements.
    /// </remarks>
    /// <seealso cref="SelectableState" />
    /// <seealso cref="AnimationFontAction{TState,TActionData}" />
    /// <seealso cref="AnimationFontDataSelectableAction" />
    public class AnimationFontSelectableAction : AnimationFontAction<SelectableState, AnimationFontDataSelectableAction>
    {
    }
}