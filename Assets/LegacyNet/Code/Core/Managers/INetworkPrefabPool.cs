using Riptide;
using UnityEngine;

namespace LegacyNetworking
{
    public interface INetworkPrefabPool
    {
        public GameObject Instantiate(object key, Vector3 position = default, Quaternion rotation = default) => Instantiate(key, position, rotation, Network.localClient.Id);
        public GameObject Instantiate(object key, Vector3 position = default, Quaternion rotation = default, int owner = (int)NetworkTags.ServerId);
        public void InstantiateToConnection(ushort connection, Message spawnMessage);
        public void Destroy(object instance, float timer = 0);
        public void OnEnable();
        public void OnDisable();
    }
}
