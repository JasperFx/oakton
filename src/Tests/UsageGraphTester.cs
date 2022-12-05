using System;
using System.Collections.Generic;
using System.Linq;
using JasperFx.Core.Reflection;
using Oakton;
using Oakton.Help;
using Shouldly;
using Xunit;

namespace Tests
{
    
    public class UsageGraphTester
    {
        private UsageGraph theUsageGraph = new FakeLinkCommand().Usages;

        [Fact]
        public void has_the_command_name()
        {
            theUsageGraph.CommandName.ShouldBe("list the links");
        }

        [Fact]
        public void has_the_description()
        {
            theUsageGraph.Description.ShouldBe("Manage links");
        }

        [Fact]
        public void has_both_usages()
        {
            theUsageGraph.Usages.Select(x => x.Description).OrderBy(x => x).ShouldHaveTheSameElementsAs("Link an application folder to a package folder", "List the links");
        }

        [Fact]
        public void has_the_arguments()
        {
            theUsageGraph.Arguments.Select(x => x.MemberName).ShouldHaveTheSameElementsAs("AppFolder", "PackageFolder", "Stuff");
        }

        [Fact]
        public void has_the_flags()
        {
            theUsageGraph.Flags.Select(x => x.MemberName).ShouldHaveTheSameElementsAs("RemoveFlag", "CleanAllFlag", "CleanFlag", "NotepadFlag");
        }

        [Fact]
        public void first_usage_has_all_the_right_mandatories()
        {
            theUsageGraph.FindUsage("Link an application folder to a package folder").Arguments.Select(x => x.MemberName).ShouldHaveTheSameElementsAs("AppFolder", "PackageFolder");
        }

        [Fact]
        public void first_usage_has_all_the_right_flags()
        {
            theUsageGraph.FindUsage("Link an application folder to a package folder").ValidFlags.Select(x => x.MemberName).OrderBy(x => x).ShouldHaveTheSameElementsAs("CleanAllFlag", "CleanFlag", "RemoveFlag");
        }

        [Fact]
        public void second_usage_has_all_the_right_mandatories()
        {
            theUsageGraph.FindUsage("List the links").Arguments.Select(x => x.MemberName).ShouldHaveTheSameElementsAs("AppFolder");
        }

        [Fact]
        public void second_usage_has_all_the_right_flags()
        {
            theUsageGraph.FindUsage("List the links").ValidFlags.Select(x => x.MemberName).ShouldHaveTheSameElementsAs("RemoveFlag", "CleanAllFlag", "CleanFlag", "NotepadFlag");
        }

        [Fact]
        public void get_the_description_of_both_usages()
        {
            theUsageGraph.FindUsage("Link an application folder to a package folder").ShouldNotBeNull();
            theUsageGraph.FindUsage("List the links").Description.ShouldNotBeNull();
        }

        [Fact]
        public void get_the_command_usage_of_the_list_usage()
        {
            theUsageGraph.FindUsage("Link an application folder to a package folder").ToUsage("fubu", "link").ShouldBe("fubu link <appfolder> <packagefolder> [-r, --remove] [-C, --clean-all] [-c, --clean <clean>]");
        }

        [Fact]
        public void get_the_command_usage_of_the_link_usage()
        {
            var usg = theUsageGraph.FindUsage("List the links");
            usg.ToUsage("fubu", "list").ShouldBe("fubu list <appfolder> [-r, --remove] [-C, --clean-all] [-c, --clean <clean>] [-n, --notepad]");
        }

        [Fact]
        public void smoke_test_writing_usage()
        {
            theUsageGraph.WriteUsages("fubu");
        }

        [Fact]
        public void derive_a_single_usage_for_any_command_that_has_no_specific_usages()
        {
            var graph = new UsageGraph(typeof (SimpleCommand));
            var usage = graph.Usages.Single();
            usage.Description.ShouldBe(typeof (SimpleCommand).GetAttribute<DescriptionAttribute>().Description);
            usage.Arguments.Select(x => x.MemberName).ShouldHaveTheSameElementsAs("Arg1", "Arg2");
        }
    }

    
    public class enumberable_argument_usages
    {
        UsageGraph theUsageGraph = new UsageGraph(typeof(ComplexCommand));

        [Fact]
        public void smoke_test_writing_usage()
        {
            theUsageGraph.WriteUsages("fubu");
        }
    }

    public class ComplexInput
    {
        public string Name { get; set; }
        public IEnumerable<string> NickNames { get; set; }

        public IEnumerable<string> HerpDerpFlag { get; set; }
    }

    [Description("does complex thing")]
    public class ComplexCommand : OaktonCommand<ComplexInput>
    {
        public override bool Execute(ComplexInput input)
        {
            throw new NotImplementedException();
        }
    }

    
    public class valid_usage_tester
    {
        private UsageGraph theUsageGraph = new FakeLinkCommand().Usages;

        private bool isValidUsage(params string[] args)
        {
            var handlers = theUsageGraph.Handlers.Where(x => args.Contains(x.MemberName));
            return theUsageGraph.IsValidUsage(handlers);
        }

        [Fact]
        public void valid_with_all_required_arguments()
        {
            isValidUsage("AppFolder").ShouldBeTrue();
            isValidUsage("AppFolder", "PackageFolder").ShouldBeTrue();
        }

        [Fact]
        public void invalid_with_missing_arguments()
        {
            isValidUsage().ShouldBeFalse();
        }

        [Fact]
        public void invalid_flags()
        {
            isValidUsage("AppFolder", "PackageFolder", "NotepadFlag").ShouldBeFalse();
        }
    }
    
    public class FakeLinkInput
    {
        [Description("The root directory of the web folder")]
        public string AppFolder { get; set; }

        [Description("The root directory of a package project")]
        public string PackageFolder { get; set; }

        [Description("Removes the link from the application to the package")]
        [FlagAlias("remove", 'r')]
        public bool RemoveFlag { get; set; }

        [Description("Removes all links from the application folder")]
        [FlagAlias('C')]
        public bool CleanAllFlag { get; set; }
        
        [Description("clean a single folder")]
        public string CleanFlag { get; set; }

        [Description("Opens the application manifest in notepad")]
        public bool NotepadFlag { get; set; }

        [Description("An array of stuff")]
        public string[] Stuff { get; set; }
    }

    [Description("Manage links", Name = "List the links")]
    public class FakeLinkCommand : OaktonCommand<FakeLinkInput>
    {
        public FakeLinkCommand()
        {
            Usage("List the links").Arguments(x => x.AppFolder);
            Usage("Link an application folder to a package folder").Arguments(x => x.AppFolder, x => x.PackageFolder).ValidFlags(x => x.RemoveFlag, x => x.CleanAllFlag, x => x.CleanFlag);
        }

        public override bool Execute(FakeLinkInput input)
        {
            throw new NotImplementedException();
        }
    }

    public class SimpleInput
    {
        public string Arg1 { get; set; }
        public string Arg2 { get; set; }
    }

   


    [Description("does simple thing")]
    public class SimpleCommand : OaktonCommand<SimpleInput>
    {
        public override bool Execute(SimpleInput input)
        {
            throw new NotImplementedException();
        }
    }


}