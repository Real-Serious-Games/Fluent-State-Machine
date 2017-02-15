using System;

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
        IStateBuilder<NewStateT, IStateBuilder<T, TParent>> State<NewStateT>() where NewStateT : AbstractState, new();

        /// <summary>
        /// Create a named child state with a specified handler type.
        /// </summary>
        /// <typeparam name="NewStateT">Handler type for the new state</typeparam>
        /// <param name="name">String for identifying state in parent</param>
        /// <returns>A new state builder object for the new child state</returns>
        IStateBuilder<NewStateT, IStateBuilder<T, TParent>> State<NewStateT>(string name) where NewStateT : AbstractState, new();

        /// <summary>
        /// Create a child state with the default handler type.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A state builder object for the new child state</returns>
        IStateBuilder<State, IStateBuilder<T, TParent>> State(string name);

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
        /// Set an action to be triggerable when an event with the specified name is raised.
        /// </summary>
        IStateBuilder<T, TParent> Event(string identifier, Action<T> action);

        /// <summary>
        /// Set an action with arguments to be triggerable when an event with the specified name is raised.
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="identifier"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        IStateBuilder<T, TParent> Event<TEvent>(string identifier, Action<T, TEvent> action) where TEvent : EventArgs;

        /// <summary>
        /// Finalise the current state and return the builder for its parent.
        /// </summary>
        TParent End();
    }

    /// <summary>
    /// Builder providing a fluent API for constructing states.
    /// </summary>
    public class StateBuilder<T, TParent> : IStateBuilder<T, TParent> 
        where T : AbstractState, new()
    {
        /// <summary>
        /// Class to return when we call .End()
        /// </summary>
        TParent parentBuilder;

        /// <summary>
        /// The current state we're building.
        /// </summary>
        private T state;

        /// <summary>
        /// Create a new state builder with a specified parent state and parent builder.
        /// </summary>
        /// <param name="parentBuilder">The parent builder, or what we will return 
        /// when .End is called.</param>
        /// <param name="parentState">The parent of the new state to create.</param>
        public StateBuilder(TParent parentBuilder, AbstractState parentState)
        {
            this.parentBuilder = parentBuilder;

            // New-up state of the prescrbed type.
            state = new T();
            parentState.AddChild(state);
        }

        /// <summary>
        /// Create a new state builder with a specified parent state, parent builder,
        /// and name for the new state.
        /// </summary>
        /// <param name="parentBuilder">The parent builder, or what we will return 
        /// when .End is called.</param>
        /// <param name="parentState">The parent of the new state to create.</param>
        /// <param name="name">Name of the state to add.</param>
        public StateBuilder(TParent parentBuilder, AbstractState parentState, string name)
        {
            this.parentBuilder = parentBuilder;

            // New-up state of the prescrbed type.
            state = new T();
            parentState.AddChild(state, name);
        }

        /// <summary>
        /// Create a child state with a specified handler type. The state will take the
        /// name of the handler type.
        /// </summary>
        /// <typeparam name="NewStateT">Handler type for the new state</typeparam>
        /// <returns>A new state builder object for the new child state</returns>
        public IStateBuilder<NewStateT, IStateBuilder<T, TParent>> State<NewStateT>() 
            where NewStateT : AbstractState, new()
        {
            return new StateBuilder<NewStateT, IStateBuilder<T, TParent>>(this, state);
        }

        /// <summary>
        /// Create a named child state with a specified handler type.
        /// </summary>
        /// <typeparam name="NewStateT">Handler type for the new state</typeparam>
        /// <param name="name">String for identifying state in parent</param>
        /// <returns>A new state builder object for the new child state</returns>
        public IStateBuilder<NewStateT, IStateBuilder<T, TParent>> State<NewStateT>(string name) 
            where NewStateT : AbstractState, new()
        {
            return new StateBuilder<NewStateT, IStateBuilder<T, TParent>>(this, state, name);
        }

        /// <summary>
        /// Create a child state with the default handler type.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>A state builder object for the new child state</returns>
        public IStateBuilder<State, IStateBuilder<T, TParent>> State(string name)
        {
            return new StateBuilder<State, IStateBuilder<T, TParent>>(this, state, name);
        }

        /// <summary>
        /// Set an action to be called when we enter the state.
        /// </summary>
        public IStateBuilder<T, TParent> Enter(Action<T> onEnter)
        {
            state.SetEnterAction(() => onEnter(state));

            return this;
        }

        /// <summary>
        /// Set an action to be called when we exit the state.
        /// </summary>
        public IStateBuilder<T, TParent> Exit(Action<T> onExit)
        {
            state.SetExitAction(() => onExit(state));

            return this;
        }

        /// <summary>
        /// Set an action to be called when we update the state.
        /// </summary>
        public IStateBuilder<T, TParent> Update(Action<T, float> onUpdate)
        {
            state.SetUpdateAction(dt => onUpdate(state, dt));

            return this;
        }

        /// <summary>
        /// Set an action to be called on update when a condition is true.
        /// </summary>
        public IStateBuilder<T, TParent> Condition(Func<bool> predicate, Action<T> action)
        {
            state.SetCondition(predicate, () => action(state));

            return this;
        }

        /// <summary>
        /// Set an action to be triggerable when an event with the specified name is raised.
        /// </summary>
        public IStateBuilder<T, TParent> Event(string identifier, Action<T> action)
        {
            state.SetEvent<EventArgs>(identifier, _ => action(state));

            return this;
        }

        /// <summary>
        /// Set an action with arguments to be triggerable when an event with the specified name is raised.
        /// </summary>
        public IStateBuilder<T, TParent> Event<TEvent>(string identifier, Action<T, TEvent> action) 
            where TEvent : EventArgs
        {
            state.SetEvent<TEvent>(identifier, args => action(state, args));

            return this;
        }

        /// <summary>
        /// Finalise the current state and return the builder for its parent.
        /// </summary>
        public TParent End()
        {
            return parentBuilder;
        }
    }
}
