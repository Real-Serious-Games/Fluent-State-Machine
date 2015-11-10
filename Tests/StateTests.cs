using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace RSG.FluentStateMachineTests
{
    public class StateTests
    {
        [Fact]
        public void state_with_parent_is_added_to_parent()
        {
            var rootState = new State();

            var childState = rootState.CreateChild("foo");

            Assert.Equal(childState, rootState.Children["foo"]);
        }
    }
}
