#if STATE_BEHAVIOR_ENABLED
using System;
using System.Collections.Generic;
using Marmary.StateBehavior.Core;

namespace Marmary.StateBehavior.Menu
{
    /// <summary>
    ///     Interface for defining criteria to determine the order of menu elements in a sequencer.
    ///     This allows for flexible ordering strategies (e.g., by height, position, custom logic).
    /// </summary>
    public interface ISequencingCriterion<TState, TTrigger, TValue> where TState : Enum
        where TTrigger : Enum
        where TValue : Element<TState, TTrigger>
    {
        /// <summary>
        ///     Gets the sorting value for a given menu element.
        ///     Elements will be sorted based on this value.
        /// </summary>
        /// <param name="element">The menu element to evaluate.</param>
        /// <returns>A value used for sorting. Lower values come first.</returns>
        List<TValue> GetSortValues(List<TValue> element);
    }
}

#endif