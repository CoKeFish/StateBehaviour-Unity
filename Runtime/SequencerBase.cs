using System;
using System.Collections.Generic;
using Marmary.StateBehavior.Core;
using Marmary.StateBehavior.Menu;
using Sirenix.Serialization;
using UnityEngine;

#if STATE_BEHAVIOR_ENABLED
namespace Marmary.StateBehavior
{
    /// <summary>
    ///     Base class for sequencers that control the order of elements using a criterion.
    /// </summary>
    /// <typeparam name="TValue">Type of value used by the criterion.</typeparam>
    /// <typeparam name="TState">Type of state enum.</typeparam>
    [Serializable]
    public class SequencerBase<TValue, TState> where TState : Enum
    {
        #region Fields

        [SerializeField][OdinSerialize][SerializeReference]
        public ISequencingCriterion<TValue, TState> _criterion;

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