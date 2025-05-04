using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Oakton.Environment;

#nullable disable annotations // FIXME

public class FileExistsCheck : IEnvironmentCheck
{
    private readonly string _file;

    public FileExistsCheck(string file)
    {
        _file = file;
    }


    public Task Assert(IServiceProvider services, CancellationToken cancellation)
    {
        if (!File.Exists(_file))
        {
            throw new Exception($"File '{_file}' cannot be found!");
        }

        return Task.CompletedTask;
    }

    public string Description => ToString();

    public override string ToString()
    {
        return $"File '{_file}' exists";
    }
}