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

    public class State : IState
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
        public State(string stateName, IState parent)
        {
            this.Parent = parent;
            parent.Children.Add(stateName, this);
            parent.ActiveChildren.Push(this);
        }

        /// <summary>
        /// Parent state, or null if this is the root level state.
        /// </summary>
        public IState Parent { get; set; }

        /// <summary>
        /// Stack of child states.
        /// </summary>
        public Stack<IState> ActiveChildren { get; set; }

        public Action<IState> OnEnter;

        public Action<IState> OnExit;

        public Action<IState, float> OnUpdate;
    }

    public class State<T> : IState<T>
    {
        public T Handler { get; }

        public Action<IState<T>> OnEnter;

        public Action<IState<T>> OnExit;

        public Action<IState<T>, float> OnUpdate;
    }
}
