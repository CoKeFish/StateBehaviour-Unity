#if STATE_BEHAVIOR_ENABLED
using System.Collections.Generic;

namespace Marmary.StateBehavior.Menu
{
    /// <summary>
    ///     Interface for defining criteria to determine the order of menu elements in a sequencer.
    ///     This allows for flexible ordering strategies (e.g., by height, position, custom logic).
    /// </summary>
    public interface ISequencingCriterion<TElement>
    {
        /// <summary>
        ///     Gets the sorting value for a given menu element.
        ///     Elements will be sorted based on this value.
        /// </summary>
        /// <param name="element">The menu element to evaluate.</param>
        /// <returns>A value used for sorting. Lower values come first.</returns>
        List<TElement> GetSortValues(List<TElement> element);
    }
}

#endif