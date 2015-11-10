using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace RSG.FluentStateMachineTests
{
    public class StateTests
    {
        [Fact]
        public void state_is_added_to_parent()
        {
            var rootState = new State();

            var childState = rootState.CreateChild("foo");

            Assert.Equal(childState, rootState.Children["foo"]);
        }

        [Fact]
        public void new_state_has_correct_parent()
        {
            var rootState = new State();

            var childState = rootState.CreateChild("foo");

            Assert.Equal(rootState, childState.Parent);
        }

        [Fact]
        public void new_state_is_inactive_by_default()
        {
            var rootState = new State();

            var childState = rootState.CreateChild("foo");

            Assert.Empty(rootState.ActiveChildren);
        }
    }
}
