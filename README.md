# Marmary State Behavior

Sistema de estados para UI en Unity que combina `ScriptableObject` de datos, acciones reutilizables y máquinas de estado asincrónicas para botones, menús y contenedores animados. El paquete se distribuye como `com.marmary.statebehavior` (v1.12.2) y depende de símbolos de compilación (`STATE_BEHAVIOR_ENABLED`) que se registran desde `Editor/StateBehavior_Register.cs`.

## Arquitectura general

1. **Datos de estado** – Cada acción se configura mediante `ActionData<TState, TValue>`, que extiende `ActionDataBase` y mantiene un diccionario `estado → ActionDataStateWrapper`. Cada wrapper define el `BehaviorActionType` a ejecutar y aloja la implementación concreta (`ActionDataState<TValue>`) creada por `ActionDataStateFactory`. Actualmente se incluye `ActionDataSimpleState<TValue>`, que controla tiempos, easing y mezcla de valores (`Runtime/ActionData.cs`, `Runtime/ActionDataStateWrapper.cs`, `Runtime/ActionDataState/ActionDataSimpleState.cs`).
2. **Comportamientos** – `BehaviorActionFactory` aplica parámetros compartidos al `Tweener` (duración, loops, ejecución instantánea, etc.) según el `BehaviorActionType` seleccionado (`Runtime/BehaviorActionFactory.cs`). Estas opciones permiten reutilizar un mismo `Tween` en diferentes modos sin duplicar código.
3. **Acciones** – `Action<TState, TValue, TComponent, TActionData>` captura el valor original del componente (`originalValue`), crea un `Tweener` persistente y delega la configuración en el `ActionDataState` concreto. Las acciones implementan `IStateContract<TState>`, ofreciendo `Setup`, `Set` (asincrónico con UniTask) e `InstantSet` (`Runtime/Action.cs`, `Runtime/IStateContract.cs`).
4. **Colección de datos** – `ActionDataCollection` es un `ScriptableObject` que sólo almacena referencias a `ActionDataBase`. En modo Editor usa reflexión para instanciar acciones compatibles y asignarles su asset (`BuildActions<TState>`). Cada `Element` debe instanciar sus propias acciones para conservar estado local (por ejemplo, posiciones originales). Esto evita compartir `Tweener` u `originalValue` entre elementos distintos (`Runtime/ActionDataCollection.cs`).
5. **Elementos y máquinas de estado** – `Element<TState, TTrigger>` agrega acciones, un `TimeWrapper` común y un diccionario de `UnityEvent`s por estado. Durante `Awake` sincroniza acciones desde la colección y crea una `StateBehaviourStateMachine<TState, TTrigger>`, que a su vez hereda de `StateMachineBase` (Stateless). La máquina ejecuta acciones en paralelo (`UniTask.WhenAll`) o instantáneamente según `ShouldExecuteInstantly`, invocando eventos cuando terminan (`Runtime/Element.cs`, `Runtime/StateBehaviourStateMachine.cs`).
6. **Temporalidad** – `TimeWrapper` encapsula retrasos, duración y tipo de `Ease` compartidos por cada `Element`. Los `ActionDataSimpleState` pueden sobreescribir estos valores por estado (`Runtime/TimeWrapper.cs`).
7. **Secuenciadores** – `SequencerBase<TState, TTrigger, TValue>` ordena elementos según un `ISequencingCriterion`. `MenuSequencer` hereda de esta base y anima arreglos de `SwitchElement` aplicando un desplazamiento (`separation`) entre cada show/hide. Se incluye el criterio `RectTransformHeightCriterion`, que ordena de arriba a abajo usando la coordenada Y (`Runtime/SequencerBase.cs`, `Runtime/ISequencingCriterion.cs`, `Runtime/Criterions/RectTransformHeightCriterion.cs`, `Runtime/Menu/MenuSequencer.cs`).

## Módulos preconstruidos

### SelectableState

- `SelectableState` y `SelectableTrigger` modelan los estados clásicos de un `Selectable` de Unity (Normal, Highlighted, PressedInside/Outside) y los eventos que pueden producir transiciones (`Runtime/SelectableState/SelectableState.cs`).
- `SelectableElement` hereda de `Element<SelectableState, SelectableTrigger>` e implementa los `EventSystem` handlers (pointer, submit, cancel, etc.). Durante `Awake` crea un `SelectableStateMachine` con estado inicial `Normal`. `ButtonSelectableElement` es la implementación concreta lista para usarse con `UnityEngine.UI.Button` (`Runtime/SelectableState/SelectableElement.cs`, `Runtime/SelectableState/Elements/ButtonSelectableElement.cs`).
- `SelectableStateMachine` define la lógica de transición para los cuatro estados principales y opcionalmente invoca `onClick` antes de reprocesar acciones (`Runtime/SelectableState/SelectableStateMachine.cs`).

### SwitchState

- `SwitchState` (Show/Hide) y `SwitchTrigger` (OnShow/OnHide) modelan contenedores que deben aparecer o desaparecer (`Runtime/SwitchState/SwitchState.cs`).
- `SwitchElement` construye un `SwitchStateMachine` y expone métodos públicos (`OnShow`, `OnHide`, y versiones instantáneas) para disparar las animaciones. `MenuElement` es una implementación simple basada en este comportamiento (`Runtime/SwitchState/SwitchElement.cs`, `Runtime/SwitchState/MenuElement.cs`, `Runtime/SwitchState/SwitchStateMachine.cs`).
- Estos elementos son la base del sistema de menús y también pueden usarse de manera independiente para popups o paneles.

### Sistema de menús y secuenciador

- `Menu` agrupa botones (`SelectableElement`s), referencia un `MenuSequencer` y administra eventos de show/hide, delays extras y selección automática del primer botón (`Runtime/Menu/Menu.cs`).
- `MenuManager` mantiene una lista de menús, activa el menú por defecto al iniciar, permite apilar menús tipo popup y ofrece `SetMenuActive(Menu menu, bool animate, bool stack)` para orquestar las transiciones. Gestiona también la cola `_menuQueue` cuando se regresa desde popups (`Runtime/Menu/MenuManager.cs`).
- `ChangeMenu` es un componente auxiliar que, al recibir un click desde su `SelectableElement`, solicita al `MenuManager` cambiar de menú (utiliza inyección de dependencias vía VContainer) (`Runtime/Menu/ChangeMenu.cs`).
- `MenuSequencer` aplica operaciones de show/hide a cada `SwitchElement` hijo respetando un intervalo (`separation`). Las operaciones pueden ser instantáneas (al iniciar y al ocultar inmediatamente) o asincrónicas esperando a que cada elemento termine (`WhenTaskCompleted()`). Para ordenar los elementos se puede usar cualquier implementación de `ISequencingCriterion`; por defecto se incluye `RectTransformHeightCriterion` (ordena de arriba hacia abajo).

## Acciones y datos disponibles

La capa de acciones se construye combinando una clase concreta (componente en el GameObject) con su `ActionData` (asset reusable). A continuación se listan las acciones incluidas y el propósito de cada una. No se detallan implementaciones internas: la idea es que puedas elegir rápidamente qué combinación necesitas y qué ScriptableObject debes crear.

### Acciones para `SelectableState`

| Acción | ActionData | Target principal | Propósito |
| --- | --- | --- | --- |
| `AnimationFontSelectableAction` | `AnimationFontDataSelectableAction` | `Febucci.TextAnimator_TMP` | Modifica parámetros de etiquetas (por ejemplo amplitud `a=`) para animaciones de texto por estado. |
| `ColorFontSelectableAction` | `ColorFontDataSelectableAction` | `TextMeshProUGUI` | Interpola el color del texto cuando el botón cambia de estado (hover, pressed, etc.). |
| `ColorSpriteSelectableAction` | `ColorSpriteDataSelectableAction` | `UnityEngine.UI.Image` | Anima el color de sprites o fondos para reflejar el estado actual. |
| `ScaleSelectableAction` | `ScaleDataSelectableAction` | `RectTransform` | Realiza tweening de escala local para crear efectos de énfasis o depresión al interactuar. |

### Acciones para `SwitchState`

| Acción | ActionData | Target principal | Propósito |
| --- | --- | --- | --- |
| `RectTransformMovementSwitchAction` | `RectTransformMovementDataSwitchAction` | `RectTransform` | Desplaza paneles dentro/fuera de pantalla según `hidingPosition` (Top/Bottom/Left/Right) manteniendo la posición original como referencia. |
| `ScaleSwitchAction` | `ScaleDataSwitchAction` | `RectTransform` | Tweenea la escala de show/hide para paneles o menús, útil para efectos de aparición/desaparición tipo popup. |

> Cada `ActionData` se crea via `CreateAssetMenu` (menú "StateBehavior/..."), se añade a una `ActionDataCollection`, y luego se sincroniza en el `Element` correspondiente mediante el botón **"Sync Actions From Collection"** disponible en el inspector de `Element`.

## Flujo de trabajo recomendado

1. **Habilitar el sistema** – Activa el símbolo `STATE_BEHAVIOR_ENABLED` desde el módulo de símbolos (`Editor/StateBehavior_Register.cs`) o de manera manual en _Player Settings_.
2. **Crear datos** – Para cada efecto necesario, crea un asset `ActionData` (por ejemplo `ColorFontDataSelectableAction`) y define los estados requeridos. `ActionData` garantiza que exista una entrada por cada valor del enum, por lo que sólo tendrás que rellenar los campos relevantes.
3. **Agrupar en colecciones** – Añade todos los assets a una `ActionDataCollection`. Recuerda que la colección sólo almacena referencias; no compartas instancias de acciones entre elementos porque cada uno captura valores iniciales distintos.
4. **Sincronizar en elementos** – En el `SelectableElement`, `SwitchElement` (o derivado) asigna la colección y pulsa **Sync Actions From Collection**. Esto instanciará acciones nuevas que apuntan a tus datos. Ajusta el `TimeWrapper` del elemento si necesitas timings globales.
5. **Configurar eventos y secuencias** – Usa el diccionario `Events` del `Element` para disparar `UnityEvent`s adicionales por estado. En menús, configura `MenuSequencer` (orden y `separation`) y asocia los `SwitchElement` que se animarán.
6. **Orquestar menús** – Registra tus `Menu`s en `MenuManager`, define el menú por defecto y utiliza `ChangeMenu` o llama a `SetMenuActive` para transicionar entre ellos, con opción de apilar popups.

## Extensibilidad y notas

- **Nuevos comportamientos** – Para ampliar `BehaviorActionTypes`, crea una clase que derive de `ActionDataState<T>` e instánciala desde `ActionDataStateFactory`. El `BehaviorActionFactory` puede aplicar configuraciones adicionales al `Tweener`.
- **Nuevos criterios de secuenciación** – Implementa `ISequencingCriterion<TElement>` para definir órdenes personalizados (distancias, tags, capas, etc.) y así controlar la reproducción del `MenuSequencer`.
- **Estados personalizados** – Puedes crear otros módulos siguiendo el patrón de `SelectableState` o `SwitchState`: define enums de estado/trigger, hereda de `Element<>` y crea tu `StateBehaviourStateMachine` especializado.
- **Sincronización de acciones** – No reutilices la misma lista `actions` entre elementos. Cada objeto requiere sus propias instancias para preservar `originalValue` y el `Tweener` subyacente, como se desprende del diseño de `Action<TState, TValue, TComponent, TActionData>`.
- **Asincronía** – Todas las operaciones usan `Cysharp.Threading.Tasks`. Si necesitas esperar a que un conjunto de acciones termine (por ejemplo antes de habilitar interacción), llama a `Element.WhenTaskCompleted()`.

Con esta visión podrás identificar rápidamente dónde extender el sistema o qué piezas reutilizar sin tener que inspeccionar cada implementación concreta de los `Action`.

