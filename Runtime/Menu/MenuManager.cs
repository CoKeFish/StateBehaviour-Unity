using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using Cysharp.Threading.Tasks;
using DTT.ExtendedDebugLogs;
using I2.Loc;
using Marmary.Utils.Runtime;
using Marmary.Utils.Runtime.Structure;
using Marmary.Utils.Runtime.Structure.FlowControl;
using Marmary.Utils.Runtime.UI;
using Sirenix.OdinInspector;
using UnityEngine;
using VContainer;

namespace Marmary.StateBehavior.Runtime.Menu
{
    /// <summary>
    ///     Manages the lifecycle and behavior of menus in the application.
    ///     Provides functionality to activate, deactivate, and switch between menus.
    /// </summary>
    [IgnoreUnityLifecycle]
    public class MenuManager : MonoBehaviour, IDefaultSelectable, IInitialize
    {
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
        ///     Indicates if the current active menu should be activated after it is hidden
        /// </summary>
        [SerializeField] [BoxGroup("Options", ShowLabel = false)] [TitleGroup("Options/Options")]
        private bool activeAfterHiding;


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

        #endregion

        #region Fields

        /// <summary>
        ///     Queue of menus that have been activated
        /// </summary>
        private readonly Queue<Menu> _menuQueue = new();

        #endregion

        #region Properties

        /// <summary>
        ///     The menu that is currently active
        /// </summary>
        private static Menu CurrentActiveMenu { get; set; }

        #endregion

        #region Constructors and Injected

        /// <summary>
        ///     Instance of an event bus used for publishing and subscribing to events within the application.
        /// </summary>
        [Inject] private IEventBus _eventBus;

        #endregion
        

        
        #region Unity Event Functions

        /// <summary>
        ///     1- Setup all menus
        ///     2- Activate the default menu and set it as the current active menu
        /// </summary>
        /// <exception cref="Exception"></exception>
        public async void Start()
        {
            try
            {
                await UniTask.DelayFrame(2);

                Initialize();

                await ActivateDefaultMenuAtStartup();

                _eventBus.Publish(new SendMenuManagerEvent(this));
            }
            catch (Exception e)
            {
                DebugEx.LogException(e);
            }
        }

        /// <summary>
        ///     Handles the cleanup process when the MenuManager is destroyed.
        ///     This method is automatically called by Unity when the associated GameObject is destroyed.
        ///     Specifically, it ensures that the reference to the currently active menu is cleared.
        /// </summary>
        private void OnDestroy()
        {
            CurrentActiveMenu = null;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Activate the given menu and inactivate the current active menu
        /// </summary>
        /// <param name="menu">parameter for the inspector using UnityEvents</param>
        /// <param name="animate"> Indicates if the menu should be animated when it is activated</param>
        /// <param name="stack"> Indicates if the current active menu should be stacked in the queue</param>
        public async UniTask SetMenuActive(Menu menu, bool animate, bool stack)
        {
            Guard.Against.Null(menu, "Menu cannot be null");

            if (menu == CurrentActiveMenu)
                //DebugEx.LogWarning("The menu is already active", UITag.Menu);
                return;

            if (stack)
            {
                CurrentActiveMenu.DisabledInteractables();
                _menuQueue.Enqueue(CurrentActiveMenu);
            }


            if (!animate)
            {
                if (!stack)
                    CurrentActiveMenu?.InstantHide();
                menu.InstantShow();
                CurrentActiveMenu = menu;
            }


            if (activeAfterHiding)
            {
                if (!stack)
                    await (CurrentActiveMenu?.InactivateMenu() ?? UniTask.CompletedTask);

                await menu.ActivateMenu();
                CurrentActiveMenu = menu;

                return;
            }

            var tasks = new List<UniTask>();

            if (!stack)
                tasks.Add(CurrentActiveMenu?.InactivateMenu() ?? UniTask.CompletedTask);

            tasks.Add(menu.ActivateMenu());

            await UniTask.WhenAll(tasks);
            CurrentActiveMenu = menu;
        }


        /// <summary>
        ///     Set the popup menu inactive and the current active menu active
        /// </summary>
        internal void SetPopupInactive()
        {
            if (_menuQueue.Count == 0) return;

            CurrentActiveMenu = _menuQueue.Dequeue();
            CurrentActiveMenu.ActiveInteractables();
            CurrentActiveMenu.firstSelected.Select();
        }

        /// <summary>
        ///     Activate the default menu and set it as the current active menu
        /// </summary>
        private async UniTask ActivateDefaultMenuAtStartup()
        {
            //DebugEx.Log("Default Menu: " + defaultMenu.name, UITag.Menu);

            await UniTask.Delay(TimeSpan.FromSeconds(delayBeforeFirstMenu));

            await SetMenuActive(defaultMenu, animateFirstMenuAtStart, false);
        }


        /// <summary>
        ///     Setup all menus, hide them and inactivate them
        /// </summary>
        public void Initialize()
        {
            foreach (var menu in allMenus)
            {
                menu.Initialize();
                menu.InstantHide();
                menu.gameObject.SetActive(false);
            }
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
            //DebugEx.Log("All menus in the scene: " + allMenus.Count, UITag.Menu);
            foreach (var menu in allMenus)
            {
                //DebugEx.Log("Menu: " + menu.name + " Scene: " + menu.gameObject.scene.name, UITag.MenuDebug);
            }
        }

#endif

        #endregion

        #region IDefaultSelectable Members

        /// <summary>
        ///     Sets the default selectable UI element based on the given position
        ///     within the currently active menu. If a selectable is found, it is
        ///     set as the currently selected UI element.
        /// </summary>
        /// <param name="position">The position that determines which selectable element is to be activated.</param>
        public void SetDefaultSelectable(Position position)
        {
            if (CurrentActiveMenu.DefaultSelectables.TryGetValue(position, out var selectable)) selectable.Select();
        }

        #endregion
    }
}