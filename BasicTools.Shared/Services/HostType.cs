namespace BasicTools.Shared.Services
{
    public class HostType
    {
        public bool IsServer { get; init; }

        public bool IsWasm => !IsServer;
    }
}
