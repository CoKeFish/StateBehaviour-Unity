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
    public abstract class Element<TState> : SerializedMonoBehaviour where TState : Enum
    {
        #region Serialized Fields

        [SerializeReference] protected List<IStateContract<TState>> actions;

        [SerializeField]
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Foldout)]
        protected ActionDataCollection actionDataCollection;

        [SerializeReference] [OdinSerialize] [NonSerialized]
        public Dictionary<TState, UnityEvent> events = new();

        #endregion

#if UNITY_EDITOR
        #region Editor Utilities

        [Button("Sync Actions From Collection")]
        private void SyncActionsFromCollection() => SyncActionsFromCollectionInternal(true);

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

        private bool NeedsCollection => actionDataCollection == null;

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
                UnityEngine.Object.DestroyImmediate(asset);
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