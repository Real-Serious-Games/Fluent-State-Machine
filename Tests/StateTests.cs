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
        public void calling_update_on_state_triggers_update_action()
        {
            var state = new State();

            var timesUpdateCalled = 0;
            state.SetUpdateAction((s, dt) => timesUpdateCalled++);

            state.Update(1.0f);

            Assert.Equal(1, timesUpdateCalled);
        }

        [Fact]
        public void calling_enter_on_state_triggers_update_action()
        {
            var state = new State();

            var timesEnterCalled = 0;
            state.SetEnterAction(_ => timesEnterCalled++);

            state.Enter();

            Assert.Equal(1, timesEnterCalled);
        }

        [Fact]
        public void calling_exit_on_state_triggers_update_action()
        {
            var state = new State();

            var timesExitCalled = 0;
            state.SetExitAction(_ => timesExitCalled++);

            state.Exit();

            Assert.Equal(1, timesExitCalled);
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

        [Fact]
        public void updating_root_state_updates_active_child()
        {
            var rootState = new State();

            var childState = rootState.CreateChild("foo");
            var timesUpdateCalled = 0;
            childState.SetUpdateAction((state, dt) => timesUpdateCalled++);

            rootState.ChangeState("foo");
            rootState.Update(1.0f);

            Assert.Equal(1, timesUpdateCalled);
        }

        [Fact]
        public void push_state_exits_the_current_state()
        {
            var rootState = new State();

            var childState = rootState.CreateChild("foo");
            var timesExitCalled = 0;
            childState.SetExitAction(state => timesExitCalled++);

            var stateToPush = rootState.CreateChild("bar");

            rootState.ChangeState("foo");
            rootState.PushState("bar");

            Assert.Equal(1, timesExitCalled);
        }

        [Fact]
        public void push_state_enters_new_state()
        {
            var rootState = new State();

            var childState = rootState.CreateChild("foo");
            var stateToPush = rootState.CreateChild("bar");
            var timesEnterCalled = 0;
            stateToPush.SetEnterAction(state => timesEnterCalled++);


            rootState.ChangeState("foo");
            rootState.PushState("bar");

            Assert.Equal(1, timesEnterCalled);
        }

        [Fact]
        public void pop_state_exits_the_current_state()
        {
            var rootState = new State();

            var childState = rootState.CreateChild("foo");
            var timesExitCalled = 0;
            childState.SetExitAction(state => timesExitCalled++);

            rootState.ChangeState("foo");
            rootState.PopState();

            Assert.Equal(1, timesExitCalled);
        }

        [Fact]
        public void pop_state_returns_to_previous_state_in_stack()
        {
            var rootState = new State();

            var childState = rootState.CreateChild("foo");
            var pushedState = rootState.CreateChild("bar");
            var timesEnterCalled = 0;
            childState.SetEnterAction(state => timesEnterCalled++);

            rootState.ChangeState("foo");
            rootState.PushState("bar");
            rootState.PopState();

            Assert.Equal(2, timesEnterCalled);
        }

        [Fact]
        public void update_is_no_longer_called_on_deactivated_states()
        {
            var rootState = new State();

            var childState = rootState.CreateChild("foo");
            var timesUpdateCalled = 0;
            childState.SetUpdateAction((state, dt) => timesUpdateCalled++);

            rootState.ChangeState("foo");
            rootState.Update(1.0f);
            rootState.PopState();
            rootState.Update(1.0f);

            Assert.Equal(1, timesUpdateCalled);
        }
    }
}
