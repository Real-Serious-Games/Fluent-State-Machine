using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace RSG
{
    /// <summary>
    /// Builder providing a fluent API for constructing states.
    /// </summary>
    public interface IStateBuilder<T, TParent> where T : AbstractState, new()
    {
        /// <summary>
        /// Create a child state with a specified handler type. The state will take the
        /// name of the handler type.
        /// </summary>
        /// <typeparam name="NewStateT">Handler type for the new state</typeparam>
        /// <returns>A new state builder object for the new child state</returns>
        IStateBuilder<NewStateT, T> State<NewStateT>() where NewStateT : AbstractState, new();

        /// <summary>
        /// Create a named child state with a specified handler type.
        /// </summary>
        /// <typeparam name="NewStateT">Handler type for the new state</typeparam>
        /// <param name="name">String for identifying state in parent</param>
        /// <returns></returns>
        /// <returns>A new state builder object for the new child state</returns>
        IStateBuilder<NewStateT, T> State<NewStateT>(string name) where NewStateT : AbstractState, new();

        /// <summary>
        /// Set an action to be called when we enter the state.
        /// </summary>
        IStateBuilder<T, TParent> Enter(Action<T> onEnter);

        /// <summary>
        /// Set an action to be called when we exit the state.
        /// </summary>
        IStateBuilder<T, TParent> Exit(Action<T> onExit);

        /// <summary>
        /// Set an action to be called when we update the state.
        /// </summary>
        IStateBuilder<T, TParent> Update(Action<T, float> onUpdate);

        /// <summary>
        /// Set an action to be called on update when a condition is true.
        /// </summary>
        IStateBuilder<T, TParent> Condition(Func<bool> predicate, Action<T> action);

        /// <summary>
        /// Finalise the current state and return the builder for its parent.
        /// </summary>
        /// <returns>Builder for the current state's parent, or null if this is the root state</returns>
        TParent End();
    }

    class StateBuilder<T, TParent> : IStateBuilder<T, TParent> where T : AbstractState, new()
    {
        TParent parentClass;

        internal T state;

        public StateBuilder(TParent parentClass)
        {
            this.parentClass = parentClass;

            // New-up state of the prescrbed type.
            state = new T();
        }

        public IStateBuilder<NewStateT, T> State<NewStateT>() where NewStateT : AbstractState, new()
        {
            throw new NotImplementedException();
        }

        public IStateBuilder<NewStateT, T> State<NewStateT>(string name) where NewStateT : AbstractState, new()
        {
            throw new NotImplementedException();
        }

        public IStateBuilder<T, TParent> Enter(Action<T> onEnter)
        {
            state.SetEnterAction(() => onEnter(state));

            return this;
        }

        public IStateBuilder<T, TParent> Exit(Action<T> onExit)
        {
            throw new NotImplementedException();
        }

        public IStateBuilder<T, TParent> Update(Action<T, float> onUpdate)
        {
            throw new NotImplementedException();
        }

        public IStateBuilder<T, TParent> Condition(Func<bool> predicate, Action<T> action)
        {
            throw new NotImplementedException();
        }

        public TParent End()
        {
            return parentClass;
        }
    }
}
