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
        public IStateBuilder<T, StateMachineBuilder> State<T>() where T : AbstractState
        {
            throw new NotImplementedException();
        }

        public IStateBuilder<T, StateMachineBuilder> State<T>(string stateName) where T : AbstractState
        {
            throw new NotImplementedException();
        }

        public IState Build()
        {
            throw new NotImplementedException();
        }
    }
}
