using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RSG;
using System.Threading;

namespace Example1
{
    class Program
    {
        /// <summary>
        /// State while the shark is just swimming around.
        /// </summary>
        class NormalState : AbstractState
        {
            public void OnUpdate()
            {
                Console.WriteLine("Swimming around...");
                hunger++;
            }
        }

        /// <summary>
        /// State for when the shark is hungry and decides to look for something to eat.
        /// </summary>
        class HungryState : AbstractState
        {
            private Random random;

            public HungryState()
            {
                random = new Random();
            }

            public void OnUpdate()
            {
                if (random.Next(5) <= 1)
                {
                    Console.WriteLine("Feeding");
                    hunger -= 5; // Decrease hunger
                }
                else
                {
                    Console.WriteLine("Hunting");
                }
                hunger++;
            }
        }

        // How hungry the shark is now
        static int hunger = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Shark state machine. Press Ctrl-C to exit.");

            // Set up the state machine
            var rootState = new StateMachineBuilder()
                .State<NormalState>("Swimming")
                    .Update((state, time) =>
                    {
                        state.OnUpdate();
                        if (hunger > 5)
                        {
                            state.PushState("Hunting");
                        }
                    })
                    .State<HungryState>("Hunting")
                        .Update((state, time) =>
                        {
                            state.OnUpdate();
                            if (hunger <= 5)
                            {
                                state.Parent.PopState();
                                return;
                            }
                        })
                    .End()
                .End()
                .Build();

            // Set the initial state.
            rootState.ChangeState("Swimming");

            // Update the state machine at a set interval.
            while (true)
            {
                rootState.Update(1.0f);
                Thread.Sleep(1000);
            }
        }
    }
}
