using System;
using System.Collections.Generic;
using Marmary.StateBehavior.Core;
using Marmary.StateBehavior.Menu;

#if STATE_BEHAVIOR_ENABLED
namespace Marmary.StateBehavior
{
    /// <summary>
    ///     Base class for sequencers that control the order of elements using a criterion.
    /// </summary>
    /// <typeparam name="TValue">Type of value used by the criterion.</typeparam>
    /// <typeparam name="TState">Type of state enum.</typeparam>
    public class SequencerBase<TValue, TState> where TState : Enum
    {
        #region Fields

        private readonly ISequencingCriterion<TValue, TState> _criterion;

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates a new sequencer base with the specified criterion.
        /// </summary>
        /// <param name="criterion">The criterion used to determine element order.</param>
        protected SequencerBase(ISequencingCriterion<TValue, TState> criterion)
        {
            _criterion = criterion ?? throw new ArgumentNullException(nameof(criterion));
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Sorts the elements according to the sequencing criterion.
        /// </summary>
        /// <param name="elements">The elements to sort.</param>
        /// <returns>Sorted list of elements.</returns>
        protected List<Element<TState>> SortElements(List<Element<TState>> elements)
        {
            if (_criterion == null || elements == null || elements.Count == 0)
                return elements ?? new List<Element<TState>>();

            return _criterion.GetSortValues(elements);
        }

        #endregion
    }
}
#endif