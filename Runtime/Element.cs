#if STATE_BEHAVIOR_ENABLED
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Marmary.StateBehavior.Runtime
{
    /// <summary>
    ///     Base MonoBehaviour responsible for orchestrating state-driven actions.
    /// </summary>
    public abstract class Element<TState, TTrigger> : SerializedMonoBehaviour where TState : Enum where TTrigger : Enum
    {
        #region Serialized Fields

        /// <summary>
        ///     The time-related wrapper object used to control and handle scheduling
        ///     and delays for state transitions, actions, and event execution within
        ///     the state machine context of an element, providing mechanisms such as
        ///     custom delay configuration.
        /// </summary>
        public TimeWrapper Time;

        /// <summary>
        ///     The state machine responsible for handling transitions and logic
        ///     between different states of a selectable UI element (e.g., Normal,
        ///     Highlighted, PressedInside, PressedOutside) based on user inputs such as
        ///     pointer interactions, selection, and submission events.
        /// </summary>
        protected StateBehaviourStateMachine<TState, TTrigger> stateMachine;

        /// <summary>
        ///     Determines whether actions should be executed immediately upon the start
        ///     of the element's lifecycle. If set to true, the element will trigger its
        ///     initial state logic and actions without waiting for any external input or
        ///     triggers to occur.
        /// </summary>
        [SerializeField] private bool executeInstantly;


        /// <summary>
        ///     List of actions bound to the element for each state.
        /// </summary>
        [SerializeReference] protected List<IStateContract<TState>> actions = new();

        /// <summary>
        ///     Asset collection used to build the actions list.
        /// </summary>
        [SerializeField] [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Foldout)]
        protected ActionDataCollection actionDataCollection;

        /// <summary>
        ///     Unity events triggered when the element changes to the corresponding state.
        /// </summary>
        [SerializeReference] [OdinSerialize] [NonSerialized]
        public Dictionary<TState, UnityEvent> Events = new();

        #endregion

        #region Properties

        /// <summary>
        ///     A property that determines whether the state machine's transitions and associated actions
        ///     should be executed immediately upon triggering, bypassing any delays or intermediate steps.
        /// </summary>
        public bool ExecuteInstantly
        {
            get => stateMachine.ShouldExecuteInstantly;
            set => stateMachine.ShouldExecuteInstantly = value;
        }

        #endregion

        #region Unity Event Functions

        /// <summary>
        ///     Initializes the element's state machine and configurations during the
        ///     MonoBehaviour lifecycle's Awake phase. This method ensures that the
        ///     execution behavior aligns with the specified runtime configuration,
        ///     such as whether actions should execute instantly.
        /// </summary>
        protected virtual void Awake()
        {
            ExecuteInstantly = executeInstantly;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Executes a completion task that monitors the execution state of the
        ///     associated state machine. This method resolves when all pending
        ///     actions and transitions in the state machine have been completed.
        /// </summary>
        /// <returns>
        ///     A UniTask that completes when the state machine execution process has finished.
        /// </returns>
        public UniTask WhenTaskCompleted()
        {
            return stateMachine.WhenExecutionCompletes();
        }


        /// <summary>
        ///     Triggers the selectable state machine optionally forcing instant execution.
        /// </summary>
        /// <param name="trigger">Trigger to fire.</param>
        /// <param name="forceInstantExecution">Whether the transition should use instant execution.</param>
        protected void TriggerState(TTrigger trigger, bool forceInstantExecution)
        {
            if (stateMachine == null)
                return;

            if (forceInstantExecution)
                stateMachine.FireTriggerInstant(trigger);
            else
                stateMachine.FireTriggerAsync(trigger);
        }

        /// <summary>
        ///     Triggers the selectable state machine optionally forcing instant execution.
        /// </summary>
        /// <param name="trigger">Trigger to fire.</param>
        protected void TriggerState(TTrigger trigger)
        {
            stateMachine?.FireTrigger(trigger);
        }

        #endregion

#if UNITY_EDITOR

        #region Editor Utilities

        /// <summary>
        ///     Synchronises the actions list using the linked <see cref="ActionDataCollection" />.
        /// </summary>
        [Button("Sync Actions From Collection")]
        private void SyncActionsFromCollection()
        {
            SyncActionsFromCollectionInternal(true);
        }

        /// <summary>
        ///     Synchronises the actions list and optionally marks the object dirty.
        /// </summary>
        /// <param name="markDirty">Indicates whether the editor object should be marked dirty.</param>
        private void SyncActionsFromCollectionInternal(bool markDirty)
        {
            if (actionDataCollection == null)
            {
                actions?.Clear();
                if (markDirty) EditorUtility.SetDirty(this);
                return;
            }

            var builtActions = actionDataCollection.BuildActions<TState>();

            actions = builtActions ?? new List<IStateContract<TState>>();

            if (markDirty) EditorUtility.SetDirty(this);
        }

        /// <summary>
        ///     Determines whether the element needs an <see cref="ActionDataCollection" /> asset.
        /// </summary>
        private bool NeedsCollection => actionDataCollection == null;

        /// <summary>
        ///     Creates an <see cref="ActionDataCollection" /> asset and assigns it to the element.
        /// </summary>
        [Button("Create ActionData Collection")]
        [ShowIf(nameof(NeedsCollection))]
        private void CreateActionDataCollection()
        {
            var asset = ScriptableObject.CreateInstance<ActionDataCollection>();
            var path = EditorUtility.SaveFilePanelInProject(
                "Crear ActionDataCollection",
                "ActionDataCollection",
                "asset",
                "Selecciona dónde guardar el ActionDataCollection"
            );

            if (string.IsNullOrEmpty(path))
            {
                DestroyImmediate(asset);
                return;
            }

            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            actionDataCollection = asset;
            SyncActionsFromCollectionInternal(false);
            EditorUtility.SetDirty(this);
        }

        #endregion

#endif
    }
}
#endif