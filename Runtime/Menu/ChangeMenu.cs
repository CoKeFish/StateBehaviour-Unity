using Cysharp.Threading.Tasks;
using Marmary.StateBehavior.Runtime.Menu;
using Marmary.StateBehavior.Runtime.SelectableState;
using Marmary.StateBehavior.Runtime.SwitchState;
using UnityEngine;
using VContainer;

namespace Marmary.Libraries.UI.Menu.Components
{
    /// <summary>
    ///     Only changes the menu
    /// </summary>
    public class ChangeMenu : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>
        ///     The menu to change to
        /// </summary>
        [SerializeField] private StateBehavior.Runtime.Menu.Menu menu;

        /// <summary>
        ///     Indicates if the menu should be animated when it is changed
        /// </summary>
        [SerializeField] private bool animate = true;

        #endregion

        #region Fields

        /// <summary>
        ///     The selectable animation button
        /// </summary>
        private SelectableElement _selectableElement;

        #endregion

        #region Constructors and Injected

        /// <summary>
        /// An instance of the MenuManager class, responsible for managing menus
        /// and their transitions in the scene.
        /// </summary>
        [Inject] private MenuManager _menuManager;

        #endregion

        #region Unity Event Functions

        /// <summary>
        ///     Gets the selectable animation button and adds the listener
        /// </summary>
        private void Start()
        {
            _selectableElement = GetComponent<SelectableElement>();
            _selectableElement.onClick.AddListener(OnChangeMenu);
        }

        #endregion

        #region Event Functions

        /// <summary>
        ///     Changes the menu
        /// </summary>
        private void OnChangeMenu()
        {
            _menuManager.SetMenuActive(menu, animate, false).Forget();
        }

        #endregion
    }
}