using UnityEngine.SceneManagement;

namespace LegacyNetworking
{
    public interface INetSceneManager
    {
        public abstract void LoadSceneAsync(string key, string instanceKey, LoadSceneMode mode);
        public abstract void UnloadSceneAsync(string instanceKey);
    }
}
