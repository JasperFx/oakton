namespace Oakton.Testing
{
    public class OptionsSamples
    {
        public static void go()
        {
            // SAMPLE: configuring-opts-file
            var executor = CommandExecutor.For(_ =>
            {
                // configure the command discovery
            });

            executor.OptionsFile = "mytool.opts";
            // ENDSAMPLE
        }
    }

    // SAMPLE: SecuredInput
    public class SecuredInput
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    // ENDSAMPLE
}