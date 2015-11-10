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
        public void new_state_has_correct_parent()
        {
            var rootState = new State();

            var childState = rootState.CreateChild("foo");

            Assert.Equal(rootState, childState.Parent);
        }

        [Fact]
        public void enter_is_called_on_active_state()
        {
            var rootState = new State();

            var childState = rootState.CreateChild("foo");
            var timesEnterCalled = 0;
            childState.SetEnterAction(state => timesEnterCalled++);

            rootState.ChangeState("foo");

            Assert.Equal(1, timesEnterCalled);
        }

        [Fact]
        public void new_state_is_inactive_by_default()
        {
            var rootState = new State();

            var childState = rootState.CreateChild("foo");

            var timesEnterCalled = 0;
            childState.SetEnterAction(state => timesEnterCalled++);
            var timesUpdateCalled = 0;
            childState.SetUpdateAction((state, dt) => timesUpdateCalled++);
            var timesExitCalled = 0;
            childState.SetExitAction(state => timesExitCalled++);

            rootState.Update(1.0f);

            Assert.Equal(0, timesEnterCalled);
            Assert.Equal(0, timesUpdateCalled);
            Assert.Equal(0, timesExitCalled);
        }
    }
}
