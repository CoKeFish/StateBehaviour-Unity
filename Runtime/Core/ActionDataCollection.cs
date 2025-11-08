#if STATE_BEHAVIOR_ENABLED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Marmary.StateBehavior.Core
{
    [CreateAssetMenu(fileName = "ActionDataCollection", menuName = "StateBehavior/ActionDataCollection", order = 50)]
    public class ActionDataCollection : SerializedScriptableObject
    {
        #region Serialized Fields

        [ListDrawerSettings(ShowFoldout = true, DefaultExpandedState = true, DraggableItems = false)]
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Foldout)]
        public List<ActionDataBase> actionDataAssets = new();

        #endregion

        #region Properties

        public IReadOnlyList<ActionDataBase> ActionDataAssets => actionDataAssets;

        #endregion

#if UNITY_EDITOR
        #region Fields

        private static readonly Dictionary<Type, Type> ActionDataToActionCache = new();
        private static List<Type> cachedActionTypes;

        #endregion

        #region Methods

        public List<IStateContract<TState>> BuildActions<TState>() where TState : Enum
        {
            if (actionDataAssets == null || actionDataAssets.Count == 0)
                return new List<IStateContract<TState>>();

            var actions = new List<IStateContract<TState>>();

            foreach (var actionData in actionDataAssets)
            {
                if (actionData == null) continue;

                if (!IsActionDataAsset(actionData))
                {
                    Debug.LogWarning(
                        $"[ActionDataCollection] '{actionData.name}' ({actionData.GetType().Name}) no hereda de ActionData<,>. Se omitirá.");
                    continue;
                }

                var actionType = ResolveActionType(actionData.GetType());

                if (actionType == null)
                {
                    Debug.LogWarning(
                        $"[ActionDataCollection] No se encontró un Action que produzca '{actionData.name}' ({actionData.GetType().Name}).");
                    continue;
                }

                if (!typeof(IStateContract<TState>).IsAssignableFrom(actionType))
                {
                    Debug.LogWarning(
                        $"[ActionDataCollection] '{actionType.Name}' no implementa IStateContract<{typeof(TState).Name}>.");
                    continue;
                }

                if (Activator.CreateInstance(actionType) is not IStateContract<TState> actionInstance)
                {
                    Debug.LogWarning(
                        $"[ActionDataCollection] No fue posible crear una instancia de '{actionType.Name}'. Asegúrate de que tenga un constructor sin parámetros.");
                    continue;
                }

                if (!TryAssignActionData(actionInstance, actionType, actionData))
                    continue;

                actions.Add(actionInstance);
            }

            return actions;
        }

        private static Type ResolveActionType(Type actionDataType)
        {
            if (actionDataType == null) return null;

            if (ActionDataToActionCache.TryGetValue(actionDataType, out var cachedType))
                return cachedType;

            foreach (var actionType in GetActionTypes())
            {
                if (!DoesActionProduceData(actionType, actionDataType)) continue;

                ActionDataToActionCache[actionDataType] = actionType;
                return actionType;
            }

            ActionDataToActionCache[actionDataType] = null;
            return null;
        }

        private static IEnumerable<Type> GetActionTypes()
        {
            if (cachedActionTypes != null) return cachedActionTypes;

            cachedActionTypes = new List<Type>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] assemblyTypes;

                try
                {
                    assemblyTypes = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    assemblyTypes = e.Types.Where(t => t != null).ToArray();
                }

                foreach (var type in assemblyTypes)
                {
                    if (type == null || type.IsAbstract || type.IsInterface) continue;

                    if (type.GetConstructor(Type.EmptyTypes) == null) continue;

                    if (!ImplementsIStateContract(type)) continue;

                    if (!InheritsFromAction(type)) continue;

                    cachedActionTypes.Add(type);
                }
            }

            return cachedActionTypes;
        }

        private static bool ImplementsIStateContract(Type type)
        {
            return type.GetInterfaces().Any(i => i.IsGenericType &&
                                                i.GetGenericTypeDefinition() == typeof(IStateContract<>));
        }

        private static bool InheritsFromAction(Type type)
        {
            var current = type;

            while (current != null)
            {
                if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(Action<,>))
                    return true;

                current = current.BaseType;
            }

            return false;
        }

        private static bool DoesActionProduceData(Type actionType, Type dataType)
        {
            try
            {
                var instance = Activator.CreateInstance(actionType);
                var method = actionType.GetMethod("CreateInstanceScriptableObject",
                    BindingFlags.Instance | BindingFlags.NonPublic);

                if (method == null) return false;

                if (method.Invoke(instance, null) is not ScriptableObject previewAsset)
                    return false;

                var matches = previewAsset.GetType() == dataType;
                UnityEngine.Object.DestroyImmediate(previewAsset);

                return matches;
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    $"[ActionDataCollection] Error evaluando '{actionType.FullName}' -> {dataType.FullName}: {ex.Message}");
                return false;
            }
        }

        private static bool TryAssignActionData<TState>(IStateContract<TState> actionInstance,
            Type actionType,
            ActionDataBase actionData) where TState : Enum
        {
            var current = actionType;

            FieldInfo dataField = null;

            while (current != null)
            {
                dataField = current.GetField("Data", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (dataField != null) break;

                current = current.BaseType;
            }

            if (dataField == null)
            {
                Debug.LogWarning(
                    $"[ActionDataCollection] No se encontró el campo 'Data' en '{actionType.FullName}'.");
                return false;
            }

            if (!dataField.FieldType.IsInstanceOfType(actionData))
            {
                Debug.LogWarning(
                    $"[ActionDataCollection] El ActionData '{actionData.name}' no coincide con el tipo esperado ({dataField.FieldType.Name}).");
                return false;
            }

            dataField.SetValue(actionInstance, actionData);

            return true;
        }

        private static bool IsActionDataAsset(ActionDataBase asset)
        {
            return asset != null;
        }

        #endregion
#endif
    }
}
#endif

