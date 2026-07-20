using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Marmary.StateBehavior.Runtime.Menu
{
    /// <summary>
    ///     LIFO stack of menus put aside by <see cref="MenuManager.PushMenu" /> (e.g. the menu behind a popup),
    ///     remembering the selectable that was focused when each menu was stacked so it can be restored on pop.
    ///     Pure collection logic — no Unity lifecycle — so it is testable in edit mode.
    /// </summary>
    internal sealed class MenuStack
    {
        /// <summary>
        ///     A stacked menu together with the GameObject that was selected when it was stacked.
        ///     Both members may be null: <see cref="Menu" /> when nothing was active at push time,
        ///     <see cref="LastSelected" /> when nothing was selected.
        /// </summary>
        internal readonly struct Entry
        {
            public Entry(Menu menu, GameObject lastSelected)
            {
                Menu = menu;
                LastSelected = lastSelected;
            }

            /// <summary>
            ///     The menu that was set aside.
            /// </summary>
            public Menu Menu { get; }

            /// <summary>
            ///     The GameObject selected at the moment the menu was stacked.
            /// </summary>
            public GameObject LastSelected { get; }
        }

        /// <summary>
        ///     Backing storage for the stacked entries.
        /// </summary>
        private readonly Stack<Entry> _entries = new();

        /// <summary>
        ///     Number of stacked entries.
        /// </summary>
        public int Count => _entries.Count;

        /// <summary>
        ///     Stacks a menu with the selectable that was focused at that moment.
        /// </summary>
        public void Push(Menu menu, GameObject lastSelected)
        {
            _entries.Push(new Entry(menu, lastSelected));
        }

        /// <summary>
        ///     Removes and returns the most recently stacked entry. Throws if the stack is empty —
        ///     callers must check <see cref="Count" /> first.
        /// </summary>
        public Entry Pop()
        {
            return _entries.Pop();
        }

        /// <summary>
        ///     Whether the given menu is currently stacked.
        /// </summary>
        public bool Contains(Menu menu)
        {
            return menu != null && _entries.Any(entry => entry.Menu == menu);
        }

        /// <summary>
        ///     Removes all stacked entries.
        /// </summary>
        public void Clear()
        {
            _entries.Clear();
        }
    }
}