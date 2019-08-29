<!--title:Command Assembly Discovery-->

This feature probably won't be commonly used, but there is a mechanism to automatically find and load Oakton commands from other assemblies through file scanning.

The first step is to mark any assembly containing Oakton commands you want discovered and loaded through this mechanism with this attribute:

<[sample:using-OaktonCommandAssemblyAttribute]>

Next, when you build a `CommandFactory`, you need to explicitly opt into the auto-discovery of commands by using the `RegisterCommandsFromExtensionAssemblies()` option as shown below in the Oakton.AspNetCore code:

<[sample:using-extension-assemblies]>