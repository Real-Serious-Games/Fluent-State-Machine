using Moq;
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

            var childState = new State();
            rootState.AddChild(childState, "foo");

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
        public void calling_enter_on_state_triggers_enter_action()
        {
            var state = new State();

            var timesEnterCalled = 0;
            state.SetEnterAction(_ => timesEnterCalled++);

            state.Enter();

            Assert.Equal(1, timesEnterCalled);
        }

        [Fact]
        public void calling_exit_on_state_triggers_exit_action()
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

            var mockState = new Mock<IState>();
            rootState.AddChild(mockState.Object, "foo");

            rootState.ChangeState("foo");

            mockState.Verify(state => state.Enter(), Times.Once());
        }

        [Fact]
        public void change_to_non_existant_state_throws_exception()
        {
            var rootState = new State();

            Assert.Throws<ApplicationException>(() => rootState.ChangeState("unknown state"));
        }

        [Fact]
        public void new_state_is_inactive_by_default()
        {
            var rootState = new State();

            var mockState = new Mock<IState>();
            rootState.AddChild(mockState.Object, "foo");

            rootState.Update(1.0f);

            mockState.Verify(state => state.Enter(), Times.Never());
            mockState.Verify(state => state.Update(It.IsAny<float>()), Times.Never());
            mockState.Verify(state => state.Exit(), Times.Never());
        }

        [Fact]
        public void updating_root_state_updates_active_child()
        {
            var rootState = new State();

            var mockState = new Mock<IState>();
            rootState.AddChild(mockState.Object, "foo");

            rootState.ChangeState("foo");
            rootState.Update(1.0f);

            mockState.Verify(state => state.Update(1.0f), Times.Once());
        }

        [Fact]
        public void push_state_exits_the_current_state()
        {
            var rootState = new State();

            var mockState = new Mock<IState>();
            rootState.AddChild(mockState.Object, "foo");

            var stateToPush = new State();
            rootState.AddChild(stateToPush, "bar");

            rootState.ChangeState("foo");
            rootState.PushState("bar");

            mockState.Verify(state => state.Exit(), Times.Once());
        }

        [Fact]
        public void push_state_enters_new_state()
        {
            var rootState = new State();

            var childState = new State();
            rootState.AddChild(childState, "foo");

            var mockStateToPush = new Mock<IState>();
            rootState.AddChild(mockStateToPush.Object, "bar");

            rootState.ChangeState("foo");
            rootState.PushState("bar");

            mockStateToPush.Verify(state => state.Enter(), Times.Once());
        }

        [Fact]
        public void push_non_existant_state_throws_exception()
        {
            var rootState = new State();

            Assert.Throws<ApplicationException>(() => rootState.PushState("unknown state"));
        }

        [Fact]
        public void pop_state_exits_the_current_state()
        {
            var rootState = new State();

            var mockState = new Mock<IState>();
            rootState.AddChild(mockState.Object, "foo");

            rootState.ChangeState("foo");
            rootState.PopState();

            mockState.Verify(state => state.Exit(), Times.Once());
        }

        [Fact]
        public void pop_state_returns_to_previous_state_in_stack()
        {
            var rootState = new State();

            var mockState = new Mock<IState>();
            rootState.AddChild(mockState.Object, "foo");
            var pushedState = new State();
            rootState.AddChild(pushedState, "bar");

            rootState.ChangeState("foo");
            rootState.PushState("bar");
            rootState.PopState();

            mockState.Verify(state => state.Enter(), Times.Exactly(2));
        }

        [Fact]
        public void update_is_no_longer_called_on_deactivated_states()
        {
            var rootState = new State();

            var mockState = new Mock<IState>();
            rootState.AddChild(mockState.Object, "foo");

            rootState.ChangeState("foo");
            rootState.Update(1.0f);
            rootState.PopState();
            rootState.Update(1.0f);

            mockState.Verify(state => state.Update(It.IsAny<float>()), Times.Once());
        }
    }
}
