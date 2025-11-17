#if STATE_BEHAVIOR_ENABLED
using System.Collections.Generic;
using System.Linq;
using Marmary.StateBehavior.Core;
using SwitchStateEnum = Marmary.StateBehavior.SwitchState.SwitchState;
using UnityEngine;

namespace Marmary.StateBehavior.Menu
{
    /// <summary>
    ///     Criterion that orders menu elements by the Y position (height) of their RectTransform.
    ///     Elements with higher Y values come first (top to bottom).
    /// </summary>
    public class RectTransformHeightCriterion : ISequencingCriterion<RectTransform, SwitchStateEnum>
    {
        /// <summary>
        /// Retrieves all RectTransform instances related to the provided elements and
        /// returns them ordered by their anchoredPosition.y in descending order
        /// (higher Y first).
        /// </summary>
        /// <param name="elements">List of menu elements to evaluate.</param>
        /// <returns>List of elements sorted top-to-bottom by RectTransform Y position.</returns>
        public List<Element<SwitchStateEnum>> GetSortValues(List<Element<SwitchStateEnum>> elements)
        {
            if (elements == null || elements.Count == 0)
                return new List<Element<SwitchStateEnum>>();

            return elements
                .OrderByDescending(GetYPosition)
                .ToList();
        }

        /// <summary>
        ///     Gets the Y position of the element's RectTransform for sorting.
        ///     Falls back to world position if no RectTransform is found.
        /// </summary>
        /// <param name="element">The menu element to evaluate.</param>
        /// <returns>The Y position value.</returns>
        private float GetYPosition(Element<SwitchStateEnum> element)
        {
            if (element == null) return 0f;

            var rectTransform = element.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // Use anchoredPosition.y for UI elements
                return rectTransform.anchoredPosition.y;
            }

            // Fallback to world position if no RectTransform
            return element.transform.position.y;
        }
    }
}
#endif
