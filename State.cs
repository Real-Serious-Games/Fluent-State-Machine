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
        /// Change to the state with the specified name.
        /// </summary>
        void ChangeState(string stateName);

        /// <summary>
        /// Push another state above the current one, so that popping it will return to the
        /// current state.
        /// </summary>
        void PushState(string stateName);

        /// <summary>
        /// Exit out of the current state and enter whatever state is below it in the stack.
        /// </summary>
        void PopState();

        /// <summary>
        /// Update this state and its children with a specified delta time.
        /// </summary>
        void Update(float deltaTime);

        /// <summary>
        /// Triggered when we enter the state.
        /// </summary>
        void Enter();

        /// <summary>
        /// Triggered when we exit the state.
        /// </summary>
        void Exit();
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
        private Stack<IState> ActiveChildren { get; set; }

        /// <summary>
        /// Dictionary of all children (active and inactive), and their names.
        /// </summary>
        private IDictionary<string, IState> Children { get; set; }

        /// <summary>
        /// Pops the current state from the stack and pushes the specified one on.
        /// </summary>
        public void ChangeState(string stateName)
        {
            // Exit and pop the current state
            if (ActiveChildren.Count > 0)
            {
                ActiveChildren.Pop().Exit();
            }

            // Find the new state and add it
            try
            {
                var newState = Children[stateName];
                ActiveChildren.Push(newState);
                newState.Enter();
            }
            catch (KeyNotFoundException)
            {
                throw new ApplicationException("Tried to change to state \"" + stateName + "\", but it is not in the list of children.");
            }
        }

        /// <summary>
        /// Push another state from the existing dictionary of children to the top of the state stack.
        /// </summary>
        public void PushState(string stateName)
        {
            // Exit the current state
            if (ActiveChildren.Count > 0)
            {
                ActiveChildren.Peek().Exit();
            }

            // Find the new state and add it
            try
            {
                var newState = Children[stateName];
                ActiveChildren.Push(newState);
                newState.Enter();
            }
            catch (KeyNotFoundException)
            {
                throw new ApplicationException("Tried to change to state \"" + stateName + "\", but it is not in the list of children.");
            }
        }

        /// <summary>
        /// Remove the current state from the active state stack and activate the state immediately beneath it.
        /// </summary>
        public void PopState()
        {
            // Exit and pop the current state
            if (ActiveChildren.Count > 0)
            {
                ActiveChildren.Pop().Exit();
            }

            // Activate the next state down, if there is one
            if (ActiveChildren.Count > 0)
            {
                ActiveChildren.Peek().Enter();
            }
        }

        /// <summary>
        /// Update this state and its children with a specified delta time.
        /// </summary>
        public virtual void Update(float deltaTime)
        {
            // Update children
            if (ActiveChildren.Count > 0)
            {
                ActiveChildren.Peek().Update(deltaTime);
            }
        }

        /// <summary>
        /// Triggered when we enter the state.
        /// </summary>
        public abstract void Enter();

        /// <summary>
        /// Triggered when we exit the state.
        /// </summary>
        public abstract void Exit();

        /// <summary>
        /// Create a new state as a child of the current state.
        /// </summary>
        public void AddChild(IState newState, string stateName)
        {
            newState.Parent = this;
            Children.Add(stateName, newState);
        }

        /// <summary>
        /// Create a new state as a child of the current state and automatically derive 
        /// its name from its handler type.
        /// </summary>
        public void AddChild(IState newState)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Non-typed state class.
    /// </summary>
    public class State : AbstractState
    {
        private Action<IState> onEnter;
        private Action<IState, float> onUpdate;
        private Action<IState> onExit;

        /// <summary>
        /// Action triggered on entering the state.
        /// </summary>
        public void SetEnterAction(Action<IState> onEnter)
        {
            this.onEnter = onEnter;
        }

        /// <summary>
        /// Action triggered on exiting the state.
        /// </summary>
        public void SetExitAction(Action<IState> onExit)
        {
            this.onExit = onExit;
        }

        /// <summary>
        /// Action which passes the current state object and the delta time since the 
        /// last update to a function.
        /// </summary>
        public void SetUpdateAction(Action<IState, float> onUpdate)
        {
            this.onUpdate = onUpdate;
        }

        public override void Update(float deltaTime)
        {
            if (onUpdate != null)
            {
                onUpdate.Invoke(this, deltaTime);
            }

            base.Update(deltaTime);
        }

        /// <summary>
        /// Triggered when we enter the state.
        /// </summary>
        public override void Enter()
        {
            if (onEnter != null)
            {
                onEnter.Invoke(this);
            }
        }

        /// <summary>
        /// Triggered when we exit the state.
        /// </summary>
        public override void Exit()
        {
            if (onExit != null)
            {
                onExit.Invoke(this);
            }
        }
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
        public void SetEnterAction(Action<IState<T>> onEnter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Action triggered on exiting the state.
        /// </summary>
        public void SetExitAction(Action<IState<T>> onExit)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Action which passes the current state object and the delta time since the 
        /// last update to a function.
        /// </summary>
        public void SetUpdateAction(Action<IState<T>, float> onUpdate)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Triggered when we enter the state.
        /// </summary>
        public override void Enter()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Triggered when we exit the state.
        /// </summary>
        public override void Exit()
        {
            throw new NotImplementedException();
        }
    }
}
