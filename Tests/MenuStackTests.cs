using System.Collections.Generic;
using Marmary.StateBehavior.Runtime.Menu;
using NUnit.Framework;
using UnityEngine;

namespace Marmary.StateBehavior.Tests
{
    /// <summary>
    ///     Edit mode tests for <see cref="MenuStack" />: LIFO order, Contains semantics and
    ///     null tolerance for both the menu and the remembered selection.
    /// </summary>
    public class MenuStackTests
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
        ///     Creates a GameObject with a Menu component, tracked for teardown.
        /// </summary>
        private Runtime.Menu.Menu CreateMenu(string name)
        {
            var gameObject = new GameObject(name);
            _spawned.Add(gameObject);
            return gameObject.AddComponent<Runtime.Menu.Menu>();
        }

        [Test]
        public void Pop_ReturnsEntriesInLifoOrder()
        {
            var stack = new MenuStack();
            var first = CreateMenu("first");
            var second = CreateMenu("second");

            stack.Push(first, null);
            stack.Push(second, null);

            Assert.That(stack.Pop().Menu, Is.EqualTo(second));
            Assert.That(stack.Pop().Menu, Is.EqualTo(first));
        }

        [Test]
        public void Count_TracksPushesAndPops()
        {
            var stack = new MenuStack();
            Assert.That(stack.Count, Is.EqualTo(0));

            stack.Push(CreateMenu("menu"), null);
            Assert.That(stack.Count, Is.EqualTo(1));

            stack.Pop();
            Assert.That(stack.Count, Is.EqualTo(0));
        }

        [Test]
        public void Pop_ReturnsTheLastSelectedStoredWithTheMenu()
        {
            var stack = new MenuStack();
            var menu = CreateMenu("menu");
            var selected = new GameObject("selected");
            _spawned.Add(selected);

            stack.Push(menu, selected);

            var entry = stack.Pop();
            Assert.That(entry.Menu, Is.EqualTo(menu));
            Assert.That(entry.LastSelected, Is.EqualTo(selected));
        }

        [Test]
        public void Push_AcceptsNullMenuAndNullSelection()
        {
            var stack = new MenuStack();

            stack.Push(null, null);

            var entry = stack.Pop();
            Assert.That(entry.Menu, Is.Null);
            Assert.That(entry.LastSelected, Is.Null);
        }

        [Test]
        public void Contains_FindsOnlyStackedMenus()
        {
            var stack = new MenuStack();
            var stacked = CreateMenu("stacked");
            var other = CreateMenu("other");

            stack.Push(stacked, null);

            Assert.That(stack.Contains(stacked), Is.True);
            Assert.That(stack.Contains(other), Is.False);
            Assert.That(stack.Contains(null), Is.False);
        }

        [Test]
        public void Contains_IsFalseForNullEntriesEvenWhenStacked()
        {
            var stack = new MenuStack();

            stack.Push(null, null);

            Assert.That(stack.Contains(null), Is.False);
        }

        [Test]
        public void Clear_RemovesAllEntries()
        {
            var stack = new MenuStack();
            stack.Push(CreateMenu("a"), null);
            stack.Push(CreateMenu("b"), null);

            stack.Clear();

            Assert.That(stack.Count, Is.EqualTo(0));
        }
    }
}