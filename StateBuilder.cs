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
    interface IStateBuilder<T, TParent>
    {
        /// <summary>
        /// Create a child state with a specified handler type. The state will take the
        /// name of the handler type.
        /// </summary>
        /// <typeparam name="NewStateT">Handler type for the new state</typeparam>
        /// <returns>A new state builder object for the new child state</returns>
        IStateBuilder<NewStateT, T> State<NewStateT>();

        /// <summary>
        /// Create a named child state with a specified handler type.
        /// </summary>
        /// <typeparam name="NewStateT">Handler type for the new state</typeparam>
        /// <param name="name">String for identifying state in parent</param>
        /// <returns></returns>
        /// <returns>A new state builder object for the new child state</returns>
        IStateBuilder<NewStateT, T> State<NewStateT>(string name);

        /// <summary>
        /// Set an action to be called when we enter the state.
        /// </summary>
        IStateBuilder<T, TParent> Enter(Action<IState<T>> onEnter);

        /// <summary>
        /// Set an action to be called when we exit the state.
        /// </summary>
        IStateBuilder<T, TParent> Exit(Action<IState<T>> onExit);

        /// <summary>
        /// Set an action to be called when we update the state.
        /// </summary>
        IStateBuilder<T, TParent> Update(Action<IState<T>, float> onUpdate);

        /// <summary>
        /// Set an action to be called on update when a condition is true.
        /// </summary>
        IStateBuilder<T, TParent> Condition(Expression<Func<bool>> predicate, Action<IState<T>> action);

        /// <summary>
        /// Finalise the current state and return the builder for its parent.
        /// </summary>
        /// <returns>Builder for the current state's parent, or null if this is the root state</returns>
        TParent End();
    }

    class StateBuilder<T, TParent> : IStateBuilder<T, TParent>
    {

        public IStateBuilder<NewStateT, T> State<NewStateT>()
        {
            throw new NotImplementedException();
        }

        public IStateBuilder<NewStateT, T> State<NewStateT>(string name)
        {
            throw new NotImplementedException();
        }

        public IStateBuilder<T, TParent> Enter(Action<IState<T>> onEnter)
        {
            throw new NotImplementedException();
        }

        public IStateBuilder<T, TParent> Exit(Action<IState<T>> onExit)
        {
            throw new NotImplementedException();
        }

        public IStateBuilder<T, TParent> Update(Action<IState<T>, float> onUpdate)
        {
            throw new NotImplementedException();
        }

        public IStateBuilder<T, TParent> Condition(Expression<Func<bool>> predicate, Action<IState<T>> action)
        {
            throw new NotImplementedException();
        }

        public TParent End()
        {
            throw new NotImplementedException();
        }
    }
}
