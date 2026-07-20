using System;
using System.Collections.Generic;
using System.Threading;
using Ardalis.GuardClauses;
using Cysharp.Threading.Tasks;
using DTT.ExtendedDebugLogs;
using Marmary.Utils.Runtime;
using Marmary.Utils.Runtime.Structure;
using Marmary.Utils.Runtime.Structure.FlowControl;
using Marmary.Utils.Runtime.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;

namespace Marmary.StateBehavior.Runtime.Menu
{
    /// <summary>
    ///     Manages the lifecycle and behavior of menus in the application.
    ///     Owns the navigation state: the current menu, the stack of menus set aside by popups,
    ///     the transition-in-progress flag and the input/concurrency policies.
    /// </summary>
    [IgnoreUnityLifecycle]
    public class MenuManager : MonoBehaviour, IDefaultSelectable, IInitialize
    {
        /// <summary>
        ///     What to do when a menu transition is requested while another one is still running.
        /// </summary>
        private enum TransitionConflictPolicy
        {
            /// <summary>The new request is discarded with a warning.</summary>
            Ignore,

            /// <summary>The new request waits for the running transition to finish, then runs.</summary>
            WaitAndRun
        }

        #region Serialized Fields

        /// <summary>
        ///     All menus in the scene
        /// </summary>
        [SerializeField] [BoxGroup("Menus", ShowLabel = false)] [TitleGroup("Menus/Menus")]
        private List<Menu> allMenus = new();

        /// <summary>
        ///     The menu that is active when the game starts
        /// </summary>
        [SerializeField]
        [RequiredIn(PrefabKind.InstanceInScene)]
        [BoxGroup("Options", ShowLabel = false)]
        [TitleGroup("Options/Options")]
        private Menu defaultMenu;

        /// <summary>
        ///     If true, the outgoing menu finishes hiding before the incoming menu starts showing;
        ///     if false both animations run in parallel.
        /// </summary>
        [SerializeField]
        [FormerlySerializedAs("activeAfterHiding")]
        [BoxGroup("Options", ShowLabel = false)]
        [TitleGroup("Options/Options")]
        private bool sequentialTransitions;

        /// <summary>
        ///     Indicates if the first menu should be animated at start
        /// </summary>
        [SerializeField]
        [ToggleGroup("Options/Options/animateFirstMenuAtStart")]
        [BoxGroup("Options", ShowLabel = false)]
        [TitleGroup("Options/Options")]
        private bool animateFirstMenuAtStart = true;

        /// <summary>
        ///     The delay before the first menu is animated
        /// </summary>
        [SerializeField]
        [ToggleGroup("Options/Options/animateFirstMenuAtStart")]
        [BoxGroup("Options", ShowLabel = false)]
        [TitleGroup("Options/Options")]
        private float delayBeforeFirstMenu;

        /// <summary>
        ///     If true, the incoming menu's input gate stays closed until its show sequence completes;
        ///     if false, input is accepted as soon as the menu becomes active.
        /// </summary>
        [SerializeField] [BoxGroup("Options", ShowLabel = false)] [TitleGroup("Options/Options")]
        private bool blockInputDuringTransitions = true;

        /// <summary>
        ///     Policy applied when a transition is requested while another one is still running.
        /// </summary>
        [SerializeField] [BoxGroup("Options", ShowLabel = false)] [TitleGroup("Options/Options")]
        private TransitionConflictPolicy conflictPolicy = TransitionConflictPolicy.Ignore;

        /// <summary>
        ///     If true, popping a menu restores the selectable that was focused when it was stacked;
        ///     if false (or the selectable is gone), the menu's first selectable is selected instead.
        /// </summary>
        [SerializeField] [BoxGroup("Options", ShowLabel = false)] [TitleGroup("Options/Options")]
        private bool useRestoreSelectionOnPop = true;

        #endregion

        #region Fields

        /// <summary>
        ///     Menus set aside by <see cref="PushMenu" />, with the selection to restore on pop.
        /// </summary>
        private readonly MenuStack _menuStack = new();

        /// <summary>
        ///     The menu that is currently active. Assigned at the START of a transition, so it is
        ///     never stale while animations are running.
        /// </summary>
        private Menu _currentMenu;

        /// <summary>
        ///     True while a menu transition (activate/deactivate sequence) is running.
        /// </summary>
        private bool _isTransitioning;

        /// <summary>
        ///     Cancels startup delays and conflict waits when this manager is destroyed.
        /// </summary>
        private CancellationToken _destroyToken;

        #endregion

        #region Properties

        /// <summary>
        ///     The menu that is currently active (may be null before startup completes).
        /// </summary>
        public Menu CurrentMenu => _currentMenu;

        /// <summary>
        ///     True while a menu transition is running.
        /// </summary>
        public bool IsTransitioning => _isTransitioning;

        #endregion

        #region Constructors and Injected

        /// <summary>
        ///     Instance of an event bus used for publishing and subscribing to events within the application.
        /// </summary>
        [Inject] private IEventBus _eventBus;

        #endregion

        #region Unity Event Functions

        /// <summary>
        ///     Caches the destroy cancellation token before any async flow starts.
        /// </summary>
        private void Awake()
        {
            _destroyToken = this.GetCancellationTokenOnDestroy();
        }

        /// <summary>
        ///     1- Setup all menus (hidden and input-gated)
        ///     2- Activate the default menu and set it as the current active menu
        /// </summary>
        private async void Start()
        {
            try
            {
                await UniTask.DelayFrame(2, cancellationToken: _destroyToken);

                Initialize();

                await ActivateDefaultMenuAtStartup();

                _eventBus.Publish(new SendMenuManagerEvent(this));
            }
            catch (OperationCanceledException)
            {
                // Destroyed during startup (e.g. scene change) — nothing to clean up.
            }
            catch (Exception e)
            {
                DebugEx.LogException(e);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Replaces the current menu with the given one, hiding the current menu.
        /// </summary>
        /// <param name="menu">The menu to activate.</param>
        /// <param name="animate">Indicates if the transition should be animated.</param>
        public async UniTask SetMenuActive(Menu menu, bool animate)
        {
            Guard.Against.Null(menu);

            if (menu == _currentMenu) return;

            if (_menuStack.Contains(menu))
            {
                DebugEx.LogWarning($"SetMenuActive ignored: '{menu.name}' is stacked; pop it instead", UITag.Menu);
                return;
            }

            if (!await TryBeginTransition()) return;

            var previous = _currentMenu;
            _currentMenu = menu;

            try
            {
                previous?.SetInteractable(false);

                if (!animate)
                {
                    previous?.InstantHide();
                    menu.InstantShow();
                    menu.SetInteractable(true);
                    menu.SelectFirst();
                }
                else if (sequentialTransitions)
                {
                    await (previous?.InactivateMenu() ?? UniTask.CompletedTask);
                    await menu.ActivateMenu(blockInputDuringTransitions);
                }
                else
                {
                    var hide = previous?.InactivateMenu() ?? UniTask.CompletedTask;
                    var show = menu.ActivateMenu(blockInputDuringTransitions);
                    await UniTask.WhenAll(hide, show);
                }
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        /// <summary>
        ///     Activates the given menu on top of the current one (e.g. a popup): the current menu stays
        ///     visible but input-gated, and is stacked together with the current selection so
        ///     <see cref="PopMenu" /> can restore both.
        /// </summary>
        /// <param name="menu">The menu to activate on top.</param>
        /// <param name="animate">Indicates if the incoming menu should animate its show sequence.</param>
        public async UniTask PushMenu(Menu menu, bool animate)
        {
            Guard.Against.Null(menu);

            if (menu == _currentMenu || _menuStack.Contains(menu))
            {
                DebugEx.LogWarning($"PushMenu ignored: '{menu.name}' is already active or stacked", UITag.Menu);
                return;
            }

            if (!await TryBeginTransition()) return;

            var previous = _currentMenu;
            _currentMenu = menu;

            try
            {
                var eventSystem = EventSystem.current;
                var lastSelected = eventSystem ? eventSystem.currentSelectedGameObject : null;
                _menuStack.Push(previous, lastSelected);

                previous?.SetInteractable(false);

                if (animate)
                {
                    await menu.ActivateMenu(blockInputDuringTransitions);
                }
                else
                {
                    menu.InstantShow();
                    menu.SetInteractable(true);
                    menu.SelectFirst();
                }
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        /// <summary>
        ///     Deactivates the current (pushed) menu and restores the most recently stacked menu,
        ///     reopening its input gate and restoring its selection.
        /// </summary>
        /// <param name="animate">Indicates if the outgoing menu should animate its hide sequence.</param>
        /// <param name="hideCurrent">
        ///     If false, the outgoing menu is assumed to be hidden already by an external system
        ///     (e.g. a UIWidgets popup that closed itself) and only the navigation state is restored.
        /// </param>
        public async UniTask PopMenu(bool animate = false, bool hideCurrent = true)
        {
            if (_menuStack.Count == 0)
            {
                DebugEx.LogWarning("PopMenu ignored: the menu stack is empty", UITag.Menu);
                return;
            }

            if (!await TryBeginTransition()) return;

            var closing = _currentMenu;
            var entry = _menuStack.Pop();
            _currentMenu = entry.Menu;

            try
            {
                closing?.SetInteractable(false);

                if (hideCurrent && closing != null)
                {
                    if (animate)
                        await closing.InactivateMenu();
                    else
                        closing.InstantHide();
                }

                if (entry.Menu != null)
                {
                    entry.Menu.SetInteractable(true);
                    RestoreSelection(entry);
                }
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        /// <summary>
        ///     Setup all menus: initialized, hidden and input-gated so nothing is clickable
        ///     until the manager activates the default menu.
        /// </summary>
        public void Initialize()
        {
            foreach (var menu in allMenus)
            {
                menu.Initialize();
                menu.InstantHide();
                menu.SetInteractable(false);
            }
        }

        /// <summary>
        ///     Activate the default menu and set it as the current active menu
        /// </summary>
        private async UniTask ActivateDefaultMenuAtStartup()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeFirstMenu), cancellationToken: _destroyToken);

            await SetMenuActive(defaultMenu, animateFirstMenuAtStart);
        }

        /// <summary>
        ///     Applies <see cref="conflictPolicy" /> when a transition is already running.
        ///     Returns true when the caller may proceed (and marks the transition as started).
        /// </summary>
        private async UniTask<bool> TryBeginTransition()
        {
            if (_isTransitioning)
            {
                switch (conflictPolicy)
                {
                    case TransitionConflictPolicy.Ignore:
                        DebugEx.LogWarning("Menu transition ignored: another transition is running", UITag.Menu);
                        return false;

                    case TransitionConflictPolicy.WaitAndRun:
                        // Note: with more than two competing calls the wake-up order is not FIFO.
                        await UniTask.WaitUntil(() => !_isTransitioning, cancellationToken: _destroyToken);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(conflictPolicy));
                }
            }

            _isTransitioning = true;
            return true;
        }

        /// <summary>
        ///     Restores the selection stored in the given stack entry, falling back to the menu's
        ///     first selectable when the stored one is gone, inactive or not interactable.
        /// </summary>
        private void RestoreSelection(in MenuStack.Entry entry)
        {
            if (useRestoreSelectionOnPop && entry.LastSelected && entry.LastSelected.activeInHierarchy)
            {
                var selectable = entry.LastSelected.GetComponent<Selectable>();
                if (selectable && selectable.IsInteractable())
                {
                    selectable.Select();
                    return;
                }
            }

            entry.Menu.SelectFirst();
        }

        #endregion

        #region Editor

#if UNITY_EDITOR
        /// <summary>
        ///     Get all menus in the scene
        /// </summary>
        [Button(ButtonSizes.Large)]
        [BoxGroup("Menus", ShowLabel = false)]
        [TitleGroup("Menus/Menus")]
        public void GetAllMenus()
        {
            allMenus.Clear();
            // Get all menus in the scene, including inactive ones
            allMenus.AddRange(Resources.FindObjectsOfTypeAll<Menu>());

            allMenus = allMenus.FindAll(static menu => !string.IsNullOrEmpty(menu.gameObject.scene.name));
        }

#endif

        #endregion

        #region IDefaultSelectable Members

        /// <summary>
        ///     Sets the default selectable UI element based on the given position
        ///     within the currently active menu. Ignored while no menu is active or a
        ///     transition is running.
        /// </summary>
        /// <param name="position">The position that determines which selectable element is to be activated.</param>
        public void SetDefaultSelectable(Position position)
        {
            if (_currentMenu == null || _isTransitioning) return;

            if (!_currentMenu.DefaultSelectables.TryGetValue(position, out var selectable)) return;

            if (selectable && selectable.gameObject.activeInHierarchy) selectable.Select();
        }

        #endregion
    }
}