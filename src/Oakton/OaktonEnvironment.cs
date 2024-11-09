namespace Oakton;

#nullable disable annotations // FIXME

public static class OaktonEnvironment
{
    /// <summary>
    ///     If using Oakton as the run command in .Net Core applications with WebApplication,
    ///     this will force Oakton to automatically start up the IHost when the Program.Main()
    ///     method runs. Very useful for WebApplicationFactory testing
    /// </summary>
    public static bool AutoStartHost { get; set; }
}