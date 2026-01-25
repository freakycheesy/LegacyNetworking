using Riptide;
using System.Collections.Generic;
using UnityEngine;

namespace LegacyNetworking {
    public static partial class Network {
        [RuntimeInitializeOnLoadMethod]
        public static void Start() {
            PrefabPool = new ResourceNetworkPrefabPool();
            Server = new("LEGACYNET_SERVER");
            Client = new("LEGACYNET_CLIENT");
            NetworkMono.DontDestroyOnLoad(new GameObject("Network [Mono]",typeof(NetworkMono)));
        }
        private static INetworkPrefabPool prefabPool;
        public static INetworkPrefabPool PrefabPool {
            get => prefabPool;  set {
                prefabPool.OnDisable();
                prefabPool = value;
                prefabPool.OnEnable();
            }
        }
        public static Server Server {
            get; private set;
        }
        public static Client Client {
            get; private set;
        }
        public static void NetShutdown() {
            Client.Disconnect();
            Server.Stop();
        }
        public static void NetStart(ushort port = 8778, ushort maxClients = 16) {
            Server.Start(port, maxClients, (byte)NetworkHeaders.Group);
        }
        public static bool NetStartHost(ushort port = 8778, ushort maxClients = 16) {
            NetStart(port, maxClients);
            bool started = NetConnect("127.0.0.1", port);
            if (!started)
                started = NetConnect("127.0.0.1");
            return started;
        }
        public static bool NetConnect(ushort port, string address, ushort maxConnectionAttempts = 5) {
            return NetConnect($"{address}:{port}", maxConnectionAttempts);
        }
        public static bool NetConnect(string address, ushort maxConnectionAttempts = 5) {
            return Client.Connect(address, maxConnectionAttempts, (byte)NetworkHeaders.Group);
        }
    }
}