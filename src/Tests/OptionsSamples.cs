using Oakton;

namespace Tests
{
    public class OptionsSamples
    {
        public static void go()
        {
            #region sample_configuring_opts_file
            var executor = CommandExecutor.For(_ =>
            {
                // configure the command discovery
            });

            executor.OptionsFile = "mytool.opts";
            #endregion
        }
    }

    #region sample_SecuredInput
    public class SecuredInput
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    #endregion
}
