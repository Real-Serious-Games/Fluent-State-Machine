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
            var condition = false;
            var timesConditionActionCalled = 0;

            var rootState = new StateMachineBuilder()
                .State<TestState>("foo")
                    .Condition(() => condition, _ => timesConditionActionCalled++)
                .End()
                .Build();
            rootState.ChangeState("foo");

            rootState.Update(1f);

            condition = true;

            rootState.Update(1f);

            Assert.Equal(1, timesConditionActionCalled);
        }
    }
}
