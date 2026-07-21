using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Marmary.StateBehavior.Runtime;
using Marmary.StateBehavior.Runtime.SelectableState;
using NUnit.Framework;
using UnityEngine;

namespace Marmary.StateBehavior.Tests
{
    /// <summary>
    ///     Edit mode tests for <see cref="SelectableStateElement" />: the guarded trigger and
    ///     the instant reset used by recycled (pooled) instances.
    /// </summary>
    public class SelectableStateElementTests
    {
        private readonly List<GameObject> _spawned = new();

        [TearDown]
        public void TearDown()
        {
            foreach (var gameObject in _spawned)
                if (gameObject)
                    Object.DestroyImmediate(gameObject);

            _spawned.Clear();
        }

        /// <summary>
        ///     Creates an initialized test element, tracked for teardown.
        /// </summary>
        private TestElement CreateElement(bool initialized = true)
        {
            var gameObject = new GameObject("element");
            _spawned.Add(gameObject);

            var element = gameObject.AddComponent<TestElement>();
            if (initialized) element.Build();
            return element;
        }

        /// <summary>
        ///     Concrete element exposing the protected machinery under test.
        /// </summary>
        private sealed class TestElement : SelectableStateElement
        {
            /// <summary>
            ///     Fills the actions list with a no-op and creates the state machine.
            /// </summary>
            public void Build()
            {
                actions.Add(new NoOpAction());
                Initialize();
            }

            public void FireInstant(SelectableTrigger trigger)
            {
                TriggerState(trigger, true);
            }

            public bool TryFire(SelectableTrigger trigger)
            {
                return TryTriggerState(trigger);
            }

            public void ResetInstant()
            {
                ResetStateInstant();
            }

            public SelectableState State => stateMachine.CurrentState;
        }

        /// <summary>
        ///     Action that fulfills the contract without touching anything.
        /// </summary>
        private sealed class NoOpAction : IStateContract<SelectableState>
        {
            public void Setup(GameObject value)
            {
            }

            public UniTask Set(SelectableState state, TimeWrapper timeWrapper)
            {
                return UniTask.CompletedTask;
            }

            public void InstantSet(SelectableState state)
            {
            }
        }

        [Test]
        public void ResetStateInstant_InNormal_StaysNormal()
        {
            var element = CreateElement();

            element.ResetInstant();

            Assert.That(element.State, Is.EqualTo(SelectableState.Normal));
        }

        [Test]
        public void ResetStateInstant_FromHighlighted_ReturnsToNormal()
        {
            var element = CreateElement();
            element.FireInstant(SelectableTrigger.Select);

            element.ResetInstant();

            Assert.That(element.State, Is.EqualTo(SelectableState.Normal));
        }

        [Test]
        public void ResetStateInstant_FromPressedInside_ReturnsToNormal()
        {
            var element = CreateElement();
            element.FireInstant(SelectableTrigger.Select);
            element.FireInstant(SelectableTrigger.PointerDown);

            element.ResetInstant();

            Assert.That(element.State, Is.EqualTo(SelectableState.Normal));
        }

        [Test]
        public void ResetStateInstant_FromPressedOutside_ReturnsToNormal()
        {
            var element = CreateElement();
            element.FireInstant(SelectableTrigger.Select);
            element.FireInstant(SelectableTrigger.PointerDown);
            element.FireInstant(SelectableTrigger.Deselect);

            element.ResetInstant();

            Assert.That(element.State, Is.EqualTo(SelectableState.Normal));
        }

        [Test]
        public void ResetStateInstant_BeforeInitialize_IsNoOp()
        {
            var element = CreateElement(false);

            Assert.DoesNotThrow(() => element.ResetInstant());
        }

        [Test]
        public void TryTriggerState_UnhandledTrigger_ReturnsFalseWithoutThrowing()
        {
            var element = CreateElement();

            var fired = element.TryFire(SelectableTrigger.Deselect);

            Assert.That(fired, Is.False);
            Assert.That(element.State, Is.EqualTo(SelectableState.Normal));
        }

        [Test]
        public void TryTriggerState_HandledTrigger_Transitions()
        {
            var element = CreateElement();

            var fired = element.TryFire(SelectableTrigger.Select);

            Assert.That(fired, Is.True);
            Assert.That(element.State, Is.EqualTo(SelectableState.Highlighted));
        }

        [Test]
        public void TryTriggerState_BeforeInitialize_ReturnsFalse()
        {
            var element = CreateElement(false);

            Assert.That(element.TryFire(SelectableTrigger.Select), Is.False);
        }
    }
}