using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace RSG.FluentStateMachineTests
{
    public class StateMachineBuilderTests
    {
        class TestState : AbstractState { }

        [Fact]
        public void build_returns_root_state_which_has_added_states_under_it()
        {
            IState expectedParent = null;

            var rootState = new StateMachineBuilder()
                .State<TestState>()
                    .Enter(state => expectedParent = state.Parent)
                .End()
                .Build();

            rootState.ChangeState("TestState");

            Assert.Equal(rootState, expectedParent);
        }

        [Fact]
        public void state_with_string_has_specified_name()
        {
            IState expectedParent = null;

            var rootState = new StateMachineBuilder()
                .State<TestState>("test")
                    .Enter(state => expectedParent = state.Parent)
                .End()
                .Build();

            rootState.ChangeState("test");

            Assert.Equal(rootState, expectedParent);
        }
    }
}
