#if STATE_BEHAVIOR_ENABLED

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
#endif

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
        [FormerlySerializedAs("Time")] public TimeWrapper time;

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
#if UNITY_EDITOR
        [OnCollectionChanged("OnActionsChanged")]
#endif
        [SerializeReference]
        protected List<IStateContract<TState>> actions = new();

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
        ///     Handles changes to the actions collection, ensuring that any newly added or inserted
        ///     actions are properly configured with the relevant target. This method is automatically
        ///     invoked when the actions list is modified.
        /// </summary>
        /// <param name="info">
        ///     Details about the change in the collection, including the type of change,
        ///     the affected element, and its position within the collection.
        /// </param>
        private void OnActionsChanged(CollectionChangeInfo info)
        {
            // Solo nos interesan elementos añadidos/insertados
            if (info.ChangeType != CollectionChangeType.Add &&
                info.ChangeType != CollectionChangeType.Insert)
                return;

            if (info.Value is not IStateContract<TState> action)
                return;

            AssignTargetByReflection(action);
        }

        /// <summary>
        ///     Assigns a target component to the specified action using reflection. The method identifies a field named "target"
        ///     within the action, determines its type, and attempts to find a matching component in the current GameObject or its
        ///     immediate children. If a matching component is found, it is assigned to the identified "target" field.
        /// </summary>
        /// <param name="action">
        ///     The action instance for which the target component needs to be assigned. This action must contain
        ///     a field named "target" with the type of the required component.
        /// </param>
        private void AssignTargetByReflection(IStateContract<TState> action)
        {
            object actionObj = action;
            var actionType = actionObj.GetType();

            // Buscar el campo "target" (protected TComponent target;)
            var targetField = actionType.GetField(
                "target",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
            );

            if (targetField == null)
            {
                Debug.LogWarning(
                    $"[Element] La acción '{actionType.Name}' no tiene campo 'target'.");
                return;
            }

            var targetType = targetField.FieldType;

            // Buscar componente del tipo correcto en este GO y sus hijos inmediatos
            var target = FindTargetComponentForType(targetType);
            if (target == null)
            {
                Debug.LogWarning(
                    $"[Element] No se encontró componente de tipo '{targetType.Name}' en '{gameObject.name}' ni en sus hijos inmediatos.");
                return;
            }

            targetField.SetValue(actionObj, target);

            Debug.Log(
                $"[Element] Auto-asignado target a '{target.GetType().Name}' " +
                $"(targetFieldType: '{targetType.Name}') " +
                $"para acción '{actionType.Name}' en '{gameObject.name}'");

            EditorUtility.SetDirty(this);
        }

        /// <summary>
        ///     Searches for a component of the specified type within the current GameObject
        ///     or its immediate children. If a component of the desired type exists on the
        ///     current GameObject, it is returned. If not, the search continues in the
        ///     GameObject's immediate children (non-recursive).
        /// </summary>
        /// <param name="targetType">
        ///     The type of the component being searched for. This must be a valid
        ///     type that corresponds to a Unity component.
        /// </param>
        /// <returns>
        ///     The first component of the specified type found on the current GameObject
        ///     or its immediate children, or null if no such component exists.
        /// </returns>
        private Component FindTargetComponentForType(Type targetType)
        {
            // 1) Mismo GameObject
            var direct = GetComponent(targetType);
            if (direct != null)
                return direct;

            // 2) Hijos inmediatos (no recursivo)
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var c = child.GetComponent(targetType);
                if (c != null)
                    return c;
            }

            return null;
        }


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