using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Marmary.StateBehavior.Runtime.Menu
{
    /// <summary>
    ///     Answers focus questions at the EventSystem protocol level (GameObject + event handler
    ///     interfaces) instead of requiring the concrete <see cref="Selectable" /> class, so widgets
    ///     that implement <see cref="ISelectHandler" /> directly (e.g. virtualized list items) are
    ///     first-class focus citizens. <see cref="Selectable" /> remains the common case: when one
    ///     is present, its own <see cref="Selectable.IsInteractable" /> is honored on top.
    /// </summary>
    public static class FocusProtocol
    {
        #region Fields

        /// <summary>
        ///     Reused buffer for CanvasGroup lookups.
        /// </summary>
        private static readonly List<CanvasGroup> CanvasGroupCache = new();

        /// <summary>
        ///     Reused buffer for select-handler lookups.
        /// </summary>
        private static readonly List<ISelectHandler> SelectHandlerCache = new();

        #endregion

        #region Methods

        /// <summary>
        ///     Whether the GameObject can currently receive and use the EventSystem selection:
        ///     it is active, has an enabled <see cref="ISelectHandler" />, no CanvasGroup in its
        ///     parent chain gates interaction and, when a <see cref="Selectable" /> is present,
        ///     it reports itself interactable.
        ///     Widget-internal disabled states that are invisible to uGUI (a widget's own
        ///     interactable flag, not routed through a CanvasGroup) cannot be detected here.
        /// </summary>
        public static bool IsUsable(GameObject target)
        {
            if (!target || !target.activeInHierarchy) return false;
            if (!HasEnabledSelectHandler(target)) return false;
            if (!CanvasGroupChainAllowsInteraction(target.transform)) return false;

            // Selectable adds its own interactable flag on top of the protocol; honor it.
            return !target.TryGetComponent(out Selectable selectable) || selectable.IsInteractable();
        }

        /// <summary>
        ///     Whether the GameObject has at least one enabled component implementing
        ///     <see cref="ISelectHandler" /> — the EventSystem's own requirement to deliver
        ///     selection events to it.
        /// </summary>
        private static bool HasEnabledSelectHandler(GameObject target)
        {
            target.GetComponents(SelectHandlerCache);

            foreach (var handler in SelectHandlerCache)
                if (handler is not Behaviour behaviour || behaviour.isActiveAndEnabled)
                    return true;

            return false;
        }

        /// <summary>
        ///     Mirrors <see cref="Selectable" />'s CanvasGroup gating: any enabled non-interactable
        ///     group in the parent chain blocks interaction; a group with ignoreParentGroups stops
        ///     the upward walk.
        /// </summary>
        private static bool CanvasGroupChainAllowsInteraction(Transform transform)
        {
            for (var current = transform; current != null; current = current.parent)
            {
                current.GetComponents(CanvasGroupCache);

                var ignoreParents = false;
                foreach (var group in CanvasGroupCache)
                {
                    if (!group.enabled) continue;
                    if (!group.interactable) return false;

                    ignoreParents |= group.ignoreParentGroups;
                }

                if (ignoreParents) return true;
            }

            return true;
        }

        #endregion
    }
}