using System;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Oakton.Resources;
using Xunit;

namespace Tests.Resources
{

    public class resource_command_execution : ResourceCommandContext
    {
        [Fact]
        public async Task check_happy_path()
        {
            var blue = AddResource("blue", "color");
            var red = AddResource("red", "color");
            var green = AddResource("green", "color");

            
            theInput.Action = ResourceAction.check;
            await theCommandExecutionShouldSucceed();

            await blue.Received().Check(theInput.TokenSource.Token);
            await red.Received().Check(theInput.TokenSource.Token);
            await green.Received().Check(theInput.TokenSource.Token);

        }  
        
        [Fact]
        public async Task check_sad_path()
        {
            var blue = AddResource("blue", "color");
            var red = AddResource("red", "color");
            var green = AddResource("green", "color");
            green.Check(theInput.TokenSource.Token).Throws(new DivideByZeroException());
            
            theInput.Action = ResourceAction.check;
            await theCommandExecutionShouldFail();

            await blue.Received().Check(theInput.TokenSource.Token);
            await red.Received().Check(theInput.TokenSource.Token);
            await green.Received().Check(theInput.TokenSource.Token);

        }   
        
        [Fact]
        public async Task teardown_happy_path()
        {
            var blue = AddResource("blue", "color");
            var red = AddResource("red", "color");
            var green = AddResource("green", "color");

            
            theInput.Action = ResourceAction.teardown;
            await theCommandExecutionShouldSucceed();

            await blue.Received().Teardown(theInput.TokenSource.Token);
            await red.Received().Teardown(theInput.TokenSource.Token);
            await green.Received().Teardown(theInput.TokenSource.Token);

        }  
        
        [Fact]
        public async Task teardown_sad_path()
        {
            var blue = AddResource("blue", "color");
            var red = AddResource("red", "color");
            var green = AddResource("green", "color");
            green.Teardown(theInput.TokenSource.Token).Throws(new DivideByZeroException());
            
            theInput.Action = ResourceAction.teardown;
            await theCommandExecutionShouldFail();

            await blue.Received().Teardown(theInput.TokenSource.Token);
            await red.Received().Teardown(theInput.TokenSource.Token);
            await green.Received().Teardown(theInput.TokenSource.Token);

        }   
        
        [Fact]
        public async Task setup_happy_path()
        {
            var blue = AddResource("blue", "color");
            var red = AddResource("red", "color");
            var green = AddResource("green", "color");

            
            theInput.Action = ResourceAction.setup;
            await theCommandExecutionShouldSucceed();

            await blue.Received().Setup(theInput.TokenSource.Token);
            await red.Received().Setup(theInput.TokenSource.Token);
            await green.Received().Setup(theInput.TokenSource.Token);

        }  
        
        [Fact]
        public async Task setup_sad_path()
        {
            var blue = AddResource("blue", "color");
            var red = AddResource("red", "color");
            var green = AddResource("green", "color");
            green.Setup(theInput.TokenSource.Token).Throws(new DivideByZeroException());
            
            theInput.Action = ResourceAction.setup;
            await theCommandExecutionShouldFail();

            await blue.Received().Setup(theInput.TokenSource.Token);
            await red.Received().Setup(theInput.TokenSource.Token);
            await green.Received().Setup(theInput.TokenSource.Token);

        }   
        
        
        [Fact]
        public async Task clear_happy_path()
        {
            var blue = AddResource("blue", "color");
            var red = AddResource("red", "color");
            var green = AddResource("green", "color");

            
            theInput.Action = ResourceAction.clear;
            await theCommandExecutionShouldSucceed();

            await blue.Received().ClearState(theInput.TokenSource.Token);
            await red.Received().ClearState(theInput.TokenSource.Token);
            await green.Received().ClearState(theInput.TokenSource.Token);

        }

        [Fact] 
        public async Task clear_state_sad_path()
        {
            var blue = AddResource("blue", "color");
            var red = AddResource("red", "color");
            var green = AddResource("green", "color");
            green.ClearState(theInput.TokenSource.Token).Throws(new DivideByZeroException());
            
            theInput.Action = ResourceAction.clear;
            await theCommandExecutionShouldFail();

            await blue.Received().ClearState(theInput.TokenSource.Token);
            await red.Received().ClearState(theInput.TokenSource.Token);
            await green.Received().ClearState(theInput.TokenSource.Token);

        } 
    }
}