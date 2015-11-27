using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using RSG;

namespace RSG.FluentStateMachineTests
{
    public class StateBuilderTests
    {
        private class TestState : AbstractState { }

        [Fact]
        public void state_adds_state_as_child_of_current_state()
        {
            IState expectedParent = null;
            IState actualParent = null;

            var rootState = new StateMachineBuilder()
                .State<TestState>("foo")
                    .Enter(state => {
                        expectedParent = state;
                        state.ChangeState("TestState");
                    })
                    .State<TestState>()
                        .Enter(state => actualParent = state.Parent)
                    .End()
                .End()
                .Build();

            rootState.ChangeState("foo");

            Assert.Equal(expectedParent, actualParent);
        }

        [Fact]
        public void named_state_is_added_with_correct_name()
        {
            IState expectedParent = null;
            IState actualParent = null;

            var rootState = new StateMachineBuilder()
                .State<TestState>("foo")
                    .Enter(state => {
                        expectedParent = state;
                        state.ChangeState("bar");
                    })
                    .State<TestState>("bar")
                        .Enter(state => actualParent = state.Parent)
                    .End()
                .End()
                .Build();

            rootState.ChangeState("foo");

            Assert.Equal(expectedParent, actualParent);
        }

        [Fact]
        public void enter_sets_onEnter_action()
        {
            int timesEnterCalled = 0;

            var rootState = new StateMachineBuilder()
                .State<TestState>("foo")
                    .Enter(_ => timesEnterCalled++)
                .End()
                .Build();

            rootState.ChangeState("foo");

            Assert.Equal(1, timesEnterCalled);
        }

        [Fact]
        public void exit_sets_onExit_action()
        {
            int timesExitCalled = 0;

            var rootState = new StateMachineBuilder()
                .State<TestState>("foo")
                    .Exit(_ => timesExitCalled++)
                .End()
                .State<TestState>("bar")
                .End()
                .Build();

            rootState.ChangeState("foo");
            rootState.ChangeState("bar");

            Assert.Equal(1, timesExitCalled);
        }

        [Fact]
        public void update_sets_onUpdate_action()
        {
            int timesUpdateCalled = 0;

            var rootState = new StateMachineBuilder()
                .State<TestState>("foo")
                    .Update((state, dt) => timesUpdateCalled++)
                .End()
                .Build();

            rootState.ChangeState("foo");
            rootState.Update(1f);

            Assert.Equal(1, timesUpdateCalled);
        }

        [Fact]
        public void condition_sets_action_for_condition()
        {
            throw new NotImplementedException();
        }

        //[Fact]
        //public void end_returns_parent_state_builder()
        //{
        //    var parent = new TestState();

        //    var builder = new StateBuilder<TestState, TestState>()
        //}

        /*
        [Fact]
        public void state_is_passed_into_enter_action()
        {
            var state = CreateTestState();

            IState statePassedIn = null;

            state.SetEnterAction(st => statePassedIn = st);

            state.Enter();

            Assert.Equal(state, statePassedIn);
        }

        [Fact]
        public void state_is_passed_into_update_action()
        {
            var state = CreateTestState();

            IState statePassedIn = null;

            state.SetUpdateAction((st, dt) => statePassedIn = st);

            state.Update(1.0f);

            Assert.Equal(state, statePassedIn);
        }

        [Fact]
        public void state_is_passed_into_exit_action()
        {
            var state = CreateTestState();

            IState statePassedIn = null;

            state.SetExitAction(st => statePassedIn = st);

            state.Exit();

            Assert.Equal(state, statePassedIn);
        }

        [Fact]
        public void state_is_passed_into_condition()
        {
            var state = CreateTestState();

            IState statePassedIn = null;

            state.SetCondition(() => true, st => statePassedIn = st);

            state.Update(1f);

            Assert.Equal(state, statePassedIn);
        }
        */
    }
}
