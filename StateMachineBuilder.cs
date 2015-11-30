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
        /// <summary>
        /// Root level state.
        /// </summary>
        private State rootState;

        /// <summary>
        /// Entry point for constructing new state machines.
        /// </summary>
        public StateMachineBuilder()
        {
            rootState = new State();
        }

        /// <summary>
        /// Create a new state of a specified type and add it as a child of the root state.
        /// </summary>
        /// <typeparam name="T">Type of the state to add</typeparam>
        /// <returns>Builder used to configure the new state</returns>
        public IStateBuilder<T, StateMachineBuilder> State<T>() where T : AbstractState, new()
        {
            return new StateBuilder<T, StateMachineBuilder>(this, rootState);
        }

        /// <summary>
        /// Create a new state of a specified type with a specified name and add it as a
        /// child of the root state.
        /// </summary>
        /// <typeparam name="T">Type of the state to add</typeparam>
        /// <param name="stateName">Name for the new state</param>
        /// <returns>Builder used to configure the new state</returns>
        public IStateBuilder<T, StateMachineBuilder> State<T>(string stateName) where T : AbstractState, new()
        {
            return new StateBuilder<T, StateMachineBuilder>(this, rootState, stateName);
        }

        /// <summary>
        /// Return the root state once everything has been set up.
        /// </summary>
        public IState Build()
        {
            return rootState;
        }
    }
}
