using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    /// State with a specified handler type.
    /// </summary>
    public abstract class AbstractState : IState
    {
        /// <summary>
        /// Action called when we enter the state.
        /// </summary>
        private Action onEnter;

        /// <summary>
        /// Action called when the state gets updated.
        /// </summary>
        private Action<float> onUpdate;

        /// <summary>
        /// Action called when we exit the state.
        /// </summary>
        private Action onExit;

        private IList<Condition> conditions = new List<Condition>();

        /// <summary>
        /// Parent state, or null if this is the root level state.
        /// </summary>
        public IState Parent { get; set; }

        /// <summary>
        /// Stack of active child states.
        /// </summary>
        private Stack<IState> activeChildren = new Stack<IState>();

        /// <summary>
        /// Dictionary of all children (active and inactive), and their names.
        /// </summary>
        private IDictionary<string, IState> children = new Dictionary<string, IState>();

        /// <summary>
        /// Pops the current state from the stack and pushes the specified one on.
        /// </summary>
        public void ChangeState(string stateName)
        {
            // Exit and pop the current state
            if (activeChildren.Count > 0)
            {
                activeChildren.Pop().Exit();
            }

            // Find the new state and add it
            try
            {
                var newState = children[stateName];
                activeChildren.Push(newState);
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
            // Find the new state and add it
            IState newState;
            if (!children.TryGetValue(stateName, out newState))
            {
                throw new ApplicationException("Tried to change to state \"" + stateName + "\", but it is not in the list of children.");
            }
            activeChildren.Push(newState);
            newState.Enter();
        }

        /// <summary>
        /// Remove the current state from the active state stack and activate the state immediately beneath it.
        /// </summary>
        public void PopState()
        {
            // Exit and pop the current state
            if (activeChildren.Count > 0)
            {
                activeChildren.Pop().Exit();
            }
        }

        /// <summary>
        /// Update this state and its children with a specified delta time.
        /// </summary>
        public void Update(float deltaTime)
        {
            // Only update the child at the end of the tree
            if (activeChildren.Count == 0)
            {
                if (onUpdate != null)
                {
                    onUpdate(deltaTime);
                }

                // Update conditions
                foreach (var conditon in conditions)
                {
                    if (conditon.Predicate())
                    {
                        conditon.Action();
                    }
                }

            }
            else
            {
                activeChildren.Peek().Update(deltaTime);
            }
        }

        /// <summary>
        /// Create a new state as a child of the current state.
        /// </summary>
        public void AddChild(IState newState, string stateName)
        {
            try
            {
                children.Add(stateName, newState);
                newState.Parent = this;
            }
            catch (ArgumentException)
            {
                throw new ApplicationException("State with name \"" + stateName + "\" already exists in list of children.");
            }
        }

        /// <summary>
        /// Create a new state as a child of the current state and automatically derive 
        /// its name from its handler type.
        /// </summary>
        public void AddChild(IState newState)
        {
            var name = newState.GetType().Name;
            this.AddChild(newState, name);
        }

        private struct Condition
        {
            public Func<bool> Predicate;
            public Action Action;
        }

        public void SetCondition(Func<bool> predicate, Action action)
        {
            conditions.Add(new Condition() {
                Predicate = predicate,
                Action = action
            });
        }

        /// <summary>
        /// Action triggered on entering the state.
        /// </summary>
        public void SetEnterAction(Action onEnter)
        {
            this.onEnter = onEnter;
        }

        /// <summary>
        /// Action triggered on exiting the state.
        /// </summary>
        public void SetExitAction(Action onExit)
        {
            this.onExit = onExit;
        }

        /// <summary>
        /// Action which passes the current state object and the delta time since the 
        /// last update to a function.
        /// </summary>
        public void SetUpdateAction(Action<float> onUpdate)
        {
            this.onUpdate = onUpdate;
        }

        /// <summary>
        /// Triggered when we enter the state.
        /// </summary>
        public void Enter()
        {
            if (onEnter != null)
            {
                onEnter();
            }
        }

        /// <summary>
        /// Triggered when we exit the state.
        /// </summary>
        public void Exit()
        {
            if (onExit != null)
            {
                onExit();
            }

            while (activeChildren.Count > 0)
            {
                activeChildren.Pop().Exit();
            }
        }
    }

    /// <summary>
    /// State with no extra functionality used for root of state hierarchy.
    /// </summary>
    public class State : AbstractState
    {

    }
}
