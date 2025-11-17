#if STATE_BEHAVIOR_ENABLED
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Marmary.StateBehavior.Core
{
    /// <summary>
    ///     Base MonoBehaviour responsible for orchestrating state-driven actions.
    /// </summary>
    public abstract class Element<TState> : SerializedMonoBehaviour where TState : Enum
    {
        #region Serialized Fields

        /// <summary>
        ///     List of actions bound to the element for each state.
        /// </summary>
        [SerializeReference] protected List<IStateContract<TState>> actions;

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