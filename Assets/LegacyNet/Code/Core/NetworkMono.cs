using UnityEngine;

namespace LegacyNetworking
{
    public class NetworkMono : MonoBehaviour
    {
        private void Update() {
            Network.localClient.Update();
            if(Network.localServer.IsRunning) Network.localServer.Update();
        }
    }
}
