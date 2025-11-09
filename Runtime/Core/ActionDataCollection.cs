#if STATE_BEHAVIOR_ENABLED
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Marmary.StateBehavior.Core
{
    /// <summary>
    ///     Collection of <see cref="ActionDataBase" /> assets used to build runtime actions.
    /// </summary>
    [CreateAssetMenu(fileName = "ActionDataCollection", menuName = "StateBehavior/ActionDataCollection", order = 50)]
    public class ActionDataCollection : SerializedScriptableObject
    {
        #region Serialized Fields

        /// <summary>
        ///     Scriptable objects that define action data for concrete behaviours.
        /// </summary>
        [ListDrawerSettings(ShowFoldout = true, DefaultExpandedState = true, DraggableItems = false)]
        [InlineEditor(ObjectFieldMode = InlineEditorObjectFieldModes.Foldout)]
        public List<ActionDataBase> actionDataAssets = new();

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the collection of action data assets registered in the collection.
        /// </summary>
        public IReadOnlyList<ActionDataBase> ActionDataAssets => actionDataAssets;

        #endregion

#if UNITY_EDITOR

        #region Fields

        /// <summary>
        ///     Cache mapping action data types to their producing action types.
        /// </summary>
        private static readonly Dictionary<Type, Type> ActionDataToActionCache = new();

        /// <summary>
        ///     Cached list of resolved action types to prevent repeated reflection.
        /// </summary>
        private static List<Type> _cachedActionTypes;

        #endregion

        #region Methods

        /// <summary>
        ///     Instantiates action instances compatible with the stored assets.
        /// </summary>
        /// <typeparam name="TState">State enum type used by the actions.</typeparam>
        /// <returns>List of actions ready to be injected into an element.</returns>
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

        /// <summary>
        ///     Resolves the action type that can create the provided action data type.
        /// </summary>
        /// <param name="actionDataType">Type representing the action data.</param>
        /// <returns>Action type able to produce the data or <c>null</c>.</returns>
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

        /// <summary>
        ///     Retrieves all available action types in currently loaded assemblies.
        /// </summary>
        /// <returns>Enumeration of action types.</returns>
        private static IEnumerable<Type> GetActionTypes()
        {
            if (_cachedActionTypes != null) return _cachedActionTypes;

            _cachedActionTypes = new List<Type>();

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

                    _cachedActionTypes.Add(type);
                }
            }

            return _cachedActionTypes;
        }

        /// <summary>
        ///     Determines whether a type implements <see cref="IStateContract{TState}" />.
        /// </summary>
        /// <param name="type">Type to evaluate.</param>
        /// <returns><c>true</c> when the interface is implemented; otherwise, <c>false</c>.</returns>
        private static bool ImplementsIStateContract(Type type)
        {
            return type.GetInterfaces().Any(i => i.IsGenericType &&
                                                 i.GetGenericTypeDefinition() == typeof(IStateContract<>));
        }

        /// <summary>
        ///     Checks whether a type inherits from <see cref="Action{TState,TValue}" />.
        /// </summary>
        /// <param name="type">Type to inspect.</param>
        /// <returns><c>true</c> when the inheritance chain contains <see cref="Action{TState,TValue}" />.</returns>
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

        /// <summary>
        ///     Verifies if an action type produces the specified action data through its factory method.
        /// </summary>
        /// <param name="actionType">Action type to test.</param>
        /// <param name="dataType">Desired action data type.</param>
        /// <returns><c>true</c> if the action creates the data type; otherwise, <c>false</c>.</returns>
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
                DestroyImmediate(previewAsset);

                return matches;
            }
            catch (Exception ex)
            {
                Debug.LogError(
                    $"[ActionDataCollection] Error evaluando '{actionType.FullName}' -> {dataType.FullName}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        ///     Attaches the provided action data to the created action instance.
        /// </summary>
        /// <typeparam name="TState">Type of the state enum.</typeparam>
        /// <param name="actionInstance">Action instance that will receive the data.</param>
        /// <param name="actionType">Type of the action.</param>
        /// <param name="actionData">Action data instance to inject.</param>
        /// <returns><c>true</c> if the assignment succeeded; otherwise, <c>false</c>.</returns>
        private static bool TryAssignActionData<TState>(IStateContract<TState> actionInstance,
            Type actionType,
            ActionDataBase actionData) where TState : Enum
        {
            var current = actionType;

            FieldInfo dataField = null;

            while (current != null)
            {
                dataField = current.GetField("Data",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

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

        /// <summary>
        ///     Verifies the supplied asset derives from <see cref="ActionDataBase" />.
        /// </summary>
        /// <param name="asset">Asset to inspect.</param>
        /// <returns><c>true</c> when the asset is valid.</returns>
        private static bool IsActionDataAsset(ActionDataBase asset)
        {
            return asset != null;
        }

        #endregion

#endif
    }
}
#endif