namespace Marmary.StateBehavior.Runtime.Menu
{
    /// <summary>
    ///     Contract for elements that know how to receive their menu's initial focus themselves
    ///     (e.g. a list widget that focuses its selected item instead of its frame).
    ///     <see cref="Menu.SelectFirst" /> discovers the first active implementor among the menu's
    ///     children and delegates to it, before falling back to plain Selectable selection —
    ///     no manual wiring needed: adding the component to an element is enough.
    /// </summary>
    public interface IInitialFocus
    {
        /// <summary>
        ///     Moves the focus onto this element in whatever way the element considers correct.
        ///     Implementations own their timing (e.g. waiting for their content to exist).
        /// </summary>
        void Focus();
    }
}