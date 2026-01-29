using UnityEngine.SceneManagement;

namespace LegacyNetworking
{
    public interface INetSceneManager
    {
        public void Enable();
        public void Disable();
        public void LoadLevel(string key);
    }
}
