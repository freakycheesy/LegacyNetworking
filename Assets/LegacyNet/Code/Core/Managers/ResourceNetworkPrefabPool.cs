using Riptide;
using UnityEngine;

namespace LegacyNetworking
{
    public class ResourceNetworkPrefabPool : INetworkPrefabPool {
        public void Destroy(object instance, float timer = 0) {
        }

        public GameObject Instantiate(object key, Vector3 position = default, Quaternion rotation = default, int owner = -1) {
            Message spawnMessage = Message.Create(MessageSendMode.Reliable, NetworkHeaders.SpawnMessage);
            spawnMessage.Add(key.ToString());
            spawnMessage.Add(position.ToString());
            spawnMessage.Add(rotation.ToString());
            spawnMessage.Add(owner);
            if (Network.Server.IsRunning) {
                SendSpawnMessageToClients(spawnMessage);
            }
            else {
                Network.Client.Send(spawnMessage);
            }
            return TryInstantiate(key.ToString(), position, rotation, owner);
        }

        private GameObject TryInstantiate(string key, Vector3 position, Quaternion rotation, int owner) {
            var prefab = Resources.Load<GameObject>(key);
            if (!prefab.GetComponent<NetworkView>()) {
                Debug.LogError($"Prefab: ({key}) does not contain NetworkView Component", prefab);
                return null;
            }
            var instance = UnityEngine.Object.Instantiate(prefab, position, rotation);
            var view = instance.GetComponent<NetworkView>();
            view.InstantiateKey = key;
            Network.AllocateView(view);
            return instance;
        }

        public void OnEnable() {
            Network.Server.MessageReceived += Server_MessageReceived;
            Network.Client.MessageReceived += Client_MessageReceived;
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e) {
            if (e.MessageId != (ushort)NetworkHeaders.SpawnMessage)
                return;
            var message = e.Message;
            TryInstantiate(message.GetString(), message.GetVector3(), message.GetQuaternion(), message.GetInt());
        }

        public void OnDisable() {
        }

        private void Server_MessageReceived(object sender, MessageReceivedEventArgs e) {
            if (e.MessageId != (ushort)NetworkHeaders.SpawnMessage)
                return;
            if (!clientAuth)
                return;
            e.Message.Add(e.FromConnection.Id);
            SendSpawnMessageToClients(e.Message);
        }

        private void SendSpawnMessageToClients(Message spawnMessage) {
            foreach (var connection in Network.Server.Clients) {
                if (Network.Client.IsConnected && connection == Network.Client.Connection)
                    continue;
                InstantiateToConnection(connection.Id, spawnMessage);
            }
        }

        public void InstantiateToConnection(ushort connection, Message spawnMessage) {
            Network.Server.Send(spawnMessage,connection);
        }

        public bool clientAuth = true;   
    }
}
