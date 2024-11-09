using System.Threading.Tasks;

namespace Oakton;

#nullable disable annotations // FIXME

public class CommandRun
{
    public IOaktonCommand Command { get; set; }
    public object Input { get; set; }

    public Task<bool> Execute()
    {
        return Command.Execute(Input);
    }
}