using System.Runtime.CompilerServices;

namespace Api.IntegrationTests;

public class VerifyGlobalSettings
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyHttp.Initialize();
        Recording.Start();
    }
}
