using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSG
{
    /// <summary>
    /// Entry point for fluent API for constructing states.
    /// </summary>
    public class StateMachineBuilder
    {
        private State rootState;

        public StateMachineBuilder()
        {
            rootState = new State();
        }

        public IStateBuilder<T, StateMachineBuilder> State<T>() where T : AbstractState, new()
        {
            throw new NotImplementedException();
        }

        public IStateBuilder<T, StateMachineBuilder> State<T>(string stateName) where T : AbstractState, new()
        {
            var builder = new StateBuilder<T, StateMachineBuilder>(this);
            rootState.AddChild(builder.state, stateName);
            return builder;
        }

        public IState Build()
        {
            return rootState;
        }
    }
}
