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

        public void ChangeState(string stateName)
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// Non-typed state class.
    /// </summary>
    public class State : AbstractState
    {
        /// <summary>
        /// Construct the state as the root state.
        /// </summary>
        public State()
        {
            this.Parent = null;
        }

        /// <summary>
        /// Construct the state with a parent and a name.
        /// </summary>
        public State(IState parent, string stateName)
        {
            this.Parent = parent;
            this.Parent.Children.Add(stateName, this);
            this.Parent.ActiveChildren.Push(this);
        }

        public Action<IState> OnEnter;

        public Action<IState> OnExit;

        public Action<IState, float> OnUpdate;
    }

    /// <summary>
    /// State with a specified handler type.
    /// </summary>
    public class State<T> : AbstractState, IState<T>
    {
        /// <summary>
        /// If no name is specified, default to the name of the class.
        /// </summary>
        public State(IState parent)
        {
            this.Parent = parent;
            this.Parent.Children.Add(typeof(T).Name, this);
            this.Parent.ActiveChildren.Push(this);
        }

        /// <summary>
        /// Construct the state with a specified name and parent.
        /// </summary>
        public State(IState parent, string name)
        {
            this.Parent = parent;
            this.Parent.Children.Add(name, this);
            this.Parent.ActiveChildren.Push(this);
        }

        public T Handler { get; private set; }

        public Action<IState<T>> OnEnter;

        public Action<IState<T>> OnExit;

        public Action<IState<T>, float> OnUpdate;
    }
}
