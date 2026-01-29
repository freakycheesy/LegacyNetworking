namespace LegacyNetworking
{
    public enum NetworkTags {
        ServerId = -1,
        Group = 1,  
    }
    public enum NetworkMessages : ushort {
        SceneMessage,
        SpawnMessage,
        RpcMessage,
        StreamMessage,
    }
    public enum NetworkTargets : ushort {
        Target = 0,
        Others = 1,
        All = 2,
        Server = 3,
    }
}
