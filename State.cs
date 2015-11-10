using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RSG
{
    /// <summary>
    /// Non-generic state interface.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Parent state, or null if this is the root level state.
        /// </summary>
        IState Parent { get; set; }

        /// <summary>
        /// Stack of active child states.
        /// </summary>
        Stack<IState> ActiveChildren { get; set; }

        /// <summary>
        /// Dictionary of all children (active and inactive), and their names.
        /// </summary>
        IDictionary<string, IState> Children { get; set; }

        /// <summary>
        /// Change to the state with the specified name.
        /// </summary>
        void ChangeState(string stateName);
    }

    /// <summary>
    /// Generic state with a handler class.
    /// </summary>
    public interface IState<T> : IState
    {
        /// <summary>
        /// State handler class.
        /// </summary>
        T Handler { get; }
    }

    /// <summary>
    /// Abstract class with features common to both typed (generic) and non-typed states.
    /// </summary>
    public abstract class AbstractState : IState
    {
        public AbstractState()
        {
            Children = new Dictionary<string, IState>();
            ActiveChildren = new Stack<IState>();
        }

        /// <summary>
        /// Parent state, or null if this is the root level state.
        /// </summary>
        public IState Parent { get; set; }

        /// <summary>
        /// Stack of active child states.
        /// </summary>
        public Stack<IState> ActiveChildren { get; set; }

        /// <summary>
        /// Dictionary of all children (active and inactive), and their names.
        /// </summary>
        public IDictionary<string, IState> Children { get; set; }

        /// <summary>
        /// Pops the current state from the stack and pushes the specified one on.
        /// </summary>
        public void ChangeState(string stateName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Push another state from the existing dictionary of children to the top of the state stack.
        /// </summary>
        public void PushState(string stateName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove the current state from the active state stack and activate the state immediately beneath it.
        /// </summary>
        public void PopState()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a new state as a child of the current state.
        /// </summary>
        public State CreateChild(string stateName)
        {
            var newState = new State();
            newState.Parent = this;
            Children.Add(stateName, newState);
            return newState;
        }

        /// <summary>
        /// Create a new state with a specified handler type. The state will take the name of its handler type.
        /// </summary>
        public State<T> CreateChild<T>()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Construct a child state with a handler type and a name.
        /// </summary>
        public State<T> CreateChild<T>(string stateName)
        {
            //var newState = new State<T>();
            //newState.Parent = this;
            //this.Children.Add(stateName, newState);
            //return newState;
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Non-typed state class.
    /// </summary>
    public class State : AbstractState
    {
        /// <summary>
        /// Action triggered on entering the state.
        /// </summary>
        public Action<IState> OnEnter;

        /// <summary>
        /// Action triggered on exiting the state.
        /// </summary>
        public Action<IState> OnExit;

        /// <summary>
        /// Action which passes the current state object and the delta time since the 
        /// last update to a function.
        /// </summary>
        public Action<IState, float> OnUpdate;
    }

    /// <summary>
    /// State with a specified handler type.
    /// </summary>
    public class State<T> : AbstractState, IState<T>
    {
        public T Handler { get; private set; }

        /// <summary>
        /// Action triggered on entering the state.
        /// </summary>
        public Action<IState<T>> OnEnter;

        /// <summary>
        /// Action triggered on exiting the state.
        /// </summary>
        public Action<IState<T>> OnExit;

        /// <summary>
        /// Action which passes the current state object and the delta time since the 
        /// last update to a function.
        /// </summary>
        public Action<IState<T>, float> OnUpdate;
    }
}
