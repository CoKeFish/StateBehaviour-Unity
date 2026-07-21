using System.Collections.Generic;
using Marmary.StateBehavior.Runtime.Menu;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Marmary.StateBehavior.Tests
{
    /// <summary>
    ///     Edit mode tests for <see cref="FocusProtocol" />: protocol-level usability for
    ///     Selectables, bare ISelectHandler widgets and CanvasGroup gating.
    /// </summary>
    public class FocusProtocolTests
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
        ///     Creates a GameObject tracked for teardown, optionally parented.
        /// </summary>
        private GameObject CreateGameObject(string name, Transform parent = null)
        {
            var gameObject = new GameObject(name);
            if (parent) gameObject.transform.SetParent(parent, false);
            _spawned.Add(gameObject);
            return gameObject;
        }

        /// <summary>
        ///     Minimal stand-in for widgets that speak the EventSystem protocol without being
        ///     a <see cref="Selectable" /> (e.g. New UI Widgets list items). ExecuteAlways so
        ///     isActiveAndEnabled reflects the enabled flag in edit mode tests.
        /// </summary>
        [ExecuteAlways]
        private sealed class ProtocolOnlyHandler : MonoBehaviour, ISelectHandler
        {
            public void OnSelect(BaseEventData eventData)
            {
            }
        }

        [Test]
        public void IsUsable_NullTarget_ReturnsFalse()
        {
            Assert.That(FocusProtocol.IsUsable(null), Is.False);
        }

        [Test]
        public void IsUsable_InactiveGameObject_ReturnsFalse()
        {
            var target = CreateGameObject("inactive");
            target.AddComponent<ProtocolOnlyHandler>();
            target.SetActive(false);

            Assert.That(FocusProtocol.IsUsable(target), Is.False);
        }

        [Test]
        public void IsUsable_NoSelectHandler_ReturnsFalse()
        {
            var target = CreateGameObject("plain");

            Assert.That(FocusProtocol.IsUsable(target), Is.False);
        }

        [Test]
        public void IsUsable_InteractableSelectable_ReturnsTrue()
        {
            var target = CreateGameObject("selectable");
            target.AddComponent<Selectable>();

            Assert.That(FocusProtocol.IsUsable(target), Is.True);
        }

        [Test]
        public void IsUsable_NonInteractableSelectable_ReturnsFalse()
        {
            var target = CreateGameObject("selectable");
            target.AddComponent<Selectable>().interactable = false;

            Assert.That(FocusProtocol.IsUsable(target), Is.False);
        }

        [Test]
        public void IsUsable_ProtocolHandlerWithoutSelectable_ReturnsTrue()
        {
            var target = CreateGameObject("widget item");
            target.AddComponent<ProtocolOnlyHandler>();

            Assert.That(FocusProtocol.IsUsable(target), Is.True);
        }

        [Test]
        public void IsUsable_DisabledHandler_ReturnsFalse()
        {
            var target = CreateGameObject("widget item");
            target.AddComponent<ProtocolOnlyHandler>().enabled = false;

            Assert.That(FocusProtocol.IsUsable(target), Is.False);
        }

        [Test]
        public void IsUsable_ParentCanvasGroupNotInteractable_ReturnsFalse()
        {
            var parent = CreateGameObject("gated menu");
            parent.AddComponent<CanvasGroup>().interactable = false;

            var target = CreateGameObject("widget item", parent.transform);
            target.AddComponent<ProtocolOnlyHandler>();

            Assert.That(FocusProtocol.IsUsable(target), Is.False);
        }

        [Test]
        public void IsUsable_IgnoreParentGroups_OverridesBlockedParent()
        {
            var parent = CreateGameObject("gated menu");
            parent.AddComponent<CanvasGroup>().interactable = false;

            var target = CreateGameObject("widget item", parent.transform);
            var group = target.AddComponent<CanvasGroup>();
            group.interactable = true;
            group.ignoreParentGroups = true;
            target.AddComponent<ProtocolOnlyHandler>();

            Assert.That(FocusProtocol.IsUsable(target), Is.True);
        }

        [Test]
        public void IsUsable_DisabledCanvasGroup_IsIgnored()
        {
            var parent = CreateGameObject("gated menu");
            var group = parent.AddComponent<CanvasGroup>();
            group.interactable = false;
            group.enabled = false;

            var target = CreateGameObject("widget item", parent.transform);
            target.AddComponent<ProtocolOnlyHandler>();

            Assert.That(FocusProtocol.IsUsable(target), Is.True);
        }
    }
}