using Riptide;
using UnityEngine.SceneManagement;

namespace LegacyNetworking
{
    public class UnityNetworkSceneManager : INetSceneManager
    {
        public void LoadSceneAsync(string key, string instanceKey, LoadSceneMode mode) {
            Message message = Message.Create();
        }
        private async void TryLoadScene(Message message) {
            var key = message.GetString();
            var instanceKey = message.GetString();
            var mode = (LoadSceneMode)message.GetByte();
            await SceneManager.LoadSceneAsync(key, mode);
            if (mode == LoadSceneMode.Single)
                Network.MainScene = new(instanceKey, key);
        }
        public void UnloadSceneAsync(string instanceKey) {
            throw new System.NotImplementedException();
        }
    }
}
