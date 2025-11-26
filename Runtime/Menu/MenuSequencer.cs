#if STATE_BEHAVIOR_ENABLED
using System;
using System.Collections.Generic;
using System.Linq;
using Marmary.StateBehavior.Core;
using Marmary.StateBehavior.SwitchState;
using UnityEngine;

namespace Marmary.StateBehavior.Menu
{
    /// <summary>
    ///     Controls the order and timing of menu element animations using a sequencing criterion.
    ///     The sequencer determines the order in which elements should be shown or hidden based on a criterion.
    /// </summary>
    [Serializable]
    public class MenuSequencer : SequencerBase<RectTransform, SwitchState.SwitchState>
    {
        #region Fields

        public float _separation;

        #endregion



        #region Methods

        /// <summary>
        ///     Sorts the menu elements according to the sequencing criterion.
        /// </summary>
        /// <param name="elements">The elements to sort.</param>
        /// <returns>Sorted array of elements.</returns>
        public SwitchElement[] SortElements(SwitchElement[] elements)
        {
            if (elements == null || elements.Length == 0)
                return Array.Empty<SwitchElement>();

            // Convert array to list of Element<SwitchState>
            var elementList = elements.Cast<Element<SwitchState.SwitchState>>().ToList();

            // Sort using base class method
            var sortedList = SortElements(elementList);

            // Convert back to SwitchElement array
            return sortedList.Cast<SwitchElement>().ToArray();
        }

        /// <summary>
        ///     Gets the delay time for a specific element based on its position in the sorted sequence.
        /// </summary>
        /// <param name="elementIndex">The index of the element in the sorted sequence.</param>
        /// <returns>The delay time in seconds.</returns>
        public float GetDelayForElement(int elementIndex)
        {
            return elementIndex * _separation;
        }

        #endregion
    }
}
#endif
