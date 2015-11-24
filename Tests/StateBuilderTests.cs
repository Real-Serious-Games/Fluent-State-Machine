using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace RSG.FluentStateMachineTests
{
    public class StateBuilderTests
    {
        [Fact]
        public void state_adds_state_as_child_of_root_state()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void named_state_is_added_with_correct_name()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void enter_sets_onEnter_action()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void exit_sets_onExit_action()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void update_sets_onUpdate_action()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void condition_sets_action_for_condition()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void end_returns_parent_state()
        {
            throw new NotImplementedException();
        }

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
