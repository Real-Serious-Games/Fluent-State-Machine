# Fluent-State-Machine  [![Build Status](https://travis-ci.org/Real-Serious-Games/Fluent-State-Machine.svg)](https://travis-ci.org/Real-Serious-Games/Fluent-State-Machine) #
Fluent API for creating [hierarchical finite state machines](http://aigamedev.com/open/article/hfsm-gist/) in C#.

## A basic example - creating a state machine with a single state

Reference the dll and include the namespace:

    using RSG;
    
Create a state via the builder class:

    var rootState = new StateMachineBuilder()
        .State("main")
            .Enter(state => {
                Console.WriteLine("Entered main state");
            })
            .Update((state, deltaTime) => {
                Console.WriteLine("Updating main state");
            })
        .End()
        .Build();
        
Calling `Build()` on our StateMachineBuilder sets up all the states we've specified and returns a root
state which is automatically created to be the parent of any top-level states we create. 

The function in `Enter` will be called once when we first enter the state, and the function in `Update`
will be called every time we update the state.

Once our state machine is set up, we then need to set the initial state:

    rootState.ChangeState("main");
    
Finally, we can update our currently active state with a specified time since the last update as follows:

    rootState.Update(timeSinceLastFrame); 
    
## Conditions 

As well as Enter and Update, conditions can be set up, which are functions called when the state is
updated only when a specified predicate is satisfied.

    var rootState = new StateMachineBuilder()
        .State("main")
            .Enter(state => {
                Console.WriteLine("Entered main state");
            })
            .Update((state, deltaTime) => {
                Console.WriteLine("Updating main state");
            })
            .Condition(() => SomeBoolean, state => {
                Console.WriteLine("Condition satisfied");
            })
        .End()
        .Build();
        
The predicate to the condition is also a function, and this function will be invoked every time the state
is updated.

## Using multiple states

Switch between states by using `IState.ChangeState(stateName)` 

    var rootState = new StateMachineBuilder()
        .State("main")
            .Enter(state => {
                Console.WriteLine("Entered main state");
            })
            .Update((state, deltaTime) => {
                Console.WriteLine("Updating main state");
            })
            .Condition(() => SomeBoolean, state => {
                state.Parent.ChangeState("secondary");
            })
            .Exit(state => {
                Console.WriteLine("Exited main state");
            })
        .End()
        .State("secondary")
            .Enter(state => {
                Console.WriteLine("Entered secondary state")
            })
        .End()
        .Build();
        
`ChangeState` will attempt to exit the current state and change to the specified one. Note that since
both our "main" state and "secondary" states are children of the root state, we actually need to call
ChangeState on the main state's parent (which in this case is the root state). 

The function specified in `Exit` will be called when we exit the main state.

## Nesting states

Nested states, sometimes referred to as *super-states* are useful for encapsulating different parts of
logic and simplifying transitions between states.

    var rootState = new StateMachineBuilder()
        .State("main")
            .Enter(state => {
                Console.WriteLine("Entered main state");
            })
            .Update((state, deltaTime) => {
                Console.WriteLine("Updating main state");
            })
            .Condition(() => SomeBoolean, state => {
                state.ChangeState("child");
            })
            .Exit(state => {
                Console.WriteLine("Exited main state");
            })
            .State("child")
                .Enter(state => {
                    Console.WriteLine("Entered secondary state")
                })
            .End()
        .End()
        .Build();

Our second state is now a child of the main state. This example doesn't really add any extra 
functionality but serves to show how a nesting can work in its most basic form. Note that `Exit` is
not called on the main state when we change to it even though it is no longer being updated, since
it is still technically active but only the end of the tree is updated.

## Pushing and popping nested states

In addition to a list of children, each state contains a stack of active children

    var rootState = new StateMachineBuilder()
        .State("main")
            .Enter(state => {
                Console.WriteLine("Entered main state");
            })
            .Update((state, deltaTime) => {
                Console.WriteLine("Updating main state");
            })
            .Condition(() => SomeBoolean, state => {
                state.PushState("firstChild");
            })
            .Exit(state => {
                Console.WriteLine("Exited main state");
            })
            .State("firstChild")
                .Enter(state => {
                    Console.WriteLine("Entered secondary state")
                })
                .Condition(() => SomeBoolean, state => {
                    state.Parent.PushState("secondChild");
                })
            .End()
            .State("secondChild")
                .Enter(state => {
                    Console.WriteLine("Entered third child state")
                })
                .Condition(() => ShouldPopState, state => {
                    state.Parent.PopState();
                })
            .End()
        .End()
        .Build();

In this example, we will start out in `main`, then when `SomeBoolean` is true we'll go to `firstChild`.
If `SomeBoolean` is still true we will transition to `secondChild`, and then when `ShouldPopState` is
true we will go back the previous state on the stack (in this case `firstChild`). 

## Events

The state builder doesn't give us a reference to the new states it creates, but if we want to trigger 
an action on the currently active state that we don't want to run every frame (like a *condition*)
we can use an *event*.

    var rootState = new StateMachineBuilder()
        .State("main")
            .Enter(state => {
                Console.WriteLine("Entered main state");
            })
            .Update((state, deltaTime) => {
                Console.WriteLine("Updating main state");
            })
            .Condition(() => SomeBoolean, state => {
                state.ChangeState("child");
            })
            .State("child")
                .Enter(state => {
                    Console.WriteLine("Entered secondary state")
                })
                .Event("MyEvent", state => {
                    Console.WriteLine("MyEvent triggered");
                });
            .End()
        .End()
        .Build();

Invoking the event on the root state will trigger it on whatever the currently active state is:

    rootState.TriggerEvent("MyEvent"); 

## Custom states

Sometimes we will want to have data shared by the Enter, Update, Condition, Event and Exit functions
of a state, specific to that state, or we might want to have some methods that are internal to just
that state. This an be achieved by creating a sub-class of `AbstractState` that has our extra 
functionality in it, and specifying this when we create the state.

    class MainState : AbstractState
    {
        public string updateMessage = "Updating main state";
        
        public void PushChildState()
        {
            PushState("child");
        }
    }
    
Note that since this class inherits from AbstractState, it has full access to methods for pushing, 
popping and changing states. Since this will be newed-up by the state builder it must have either
no constructor or a parameterless constructor.

Setting up the state is basically the same as it would otherwise be except that we now also specify
the type of the state. The name field is now optional since the builder can just automatically take 
the name from the name of the class it uses, although since no two states that are children of the
same state can have the same name, this can only be done once per type of state for each group.

    var rootState = new StateMachineBuilder()
        .State<MainState>()
            .Enter(state => {
                Console.WriteLine("Entered main state");
            })
            .Update((state, deltaTime) => {
                Console.WriteLine(state.updateMessage);
            })
            .Condition(() => SomeBoolean, state => {
                state.PushChildState();
            })
            .State("child")
                .Enter(state => {
                    Console.WriteLine("Entered secondary state")
                })
                .Event("MyEvent", state => {
                    Console.WriteLine("MyEvent triggered");
                });
            .End()
        .End()
        .Build();
        
## Examples

In the `Examples` directory there are a couple of example projects to demonstrate usage of the library.

### Example 1

This sample demonstrates a fairly simple state machine with custom nested states, from a console app.

### Unity Example

This sample comes as a Unity project and shows how one could set up the library for use in a Unity 
game, as well as using *events* to trigger actions in response to Unity physics collision events. 