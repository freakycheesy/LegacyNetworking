using Riptide;
using System;
using UnityEngine.SceneManagement;

namespace LegacyNetworking
{
    public class LevelNetSceneManager : INetSceneManager {
        public void LoadLevel(string key) {
            Message message = Message.Create(MessageSendMode.Reliable, NetworkMessages.SceneMessage);
            message.Add(key);
            if (!Network.isServer) {
                return;          
            }
            Network.localServer.SendToAll(message);
            TryLoadScene(message);
        }
        private void TryLoadScene(Message message) {
            Network.MainScene = message.GetString();
            SceneManager.LoadSceneAsync(Network.MainScene);
        }

        public void Enable() {
            Network.localClient.MessageReceived += LocalClient_MessageReceived;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void LocalClient_MessageReceived(object sender, MessageReceivedEventArgs e) {
            if (e.MessageId == (ushort)NetworkMessages.SceneMessage)
                return;
            if (Network.isServer)
                return;
            var message = e.Message;
            TryLoadScene(message);
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) {
            Network.Views.Clear();
            Network.AllocateViews();
        }

        public void Disable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
