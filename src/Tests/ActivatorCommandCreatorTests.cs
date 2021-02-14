using System;
using Oakton;
using Shouldly;
using Xunit;

namespace Tests
{
    public class ActivatorCommandCreatorTests
    {
        [Fact]
        public void builds_an_instance_with_no_ctor_parameters()
        {
            var creator = new ActivatorCommandCreator();
            var instance = creator.CreateCommand(typeof (NoParamsCommand));

            instance.ShouldBeOfType<NoParamsCommand>();
        }

        [Fact]
        public void throws_if_the_ctor_has_parameters()
        {
            var creator = new ActivatorCommandCreator();

            Assert.Throws<MissingMethodException>(() => creator.CreateCommand(typeof (ParamsCommand)));
        }

        public class FakeModel
        {
        }

        private class NoParamsCommand : OaktonCommand<FakeModel>
        {
            public override bool Execute(FakeModel input)
            {
                throw new System.NotImplementedException();
            }
        }

        private class ParamsCommand : OaktonCommand<FakeModel>
        {
            public ParamsCommand(string testArgument)
            {
            }

            public override bool Execute(FakeModel input)
            {
                throw new System.NotImplementedException();
            }
        }
    }
}
