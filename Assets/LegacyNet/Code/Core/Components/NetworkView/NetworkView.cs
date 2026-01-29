using Riptide;
using UnityEngine;

namespace LegacyNetworking {
    public partial class NetworkView : MonoBehaviour {
        public object instantiateKey { get; internal set; }
        public int viewId { get; internal set; } = -1;
        public int owner { get; internal set; } = -1;
        public bool isMine {
            get {
                bool value = ownershipFlag.HasFlag(OwnershipFlag.Server) || owner < 0 && Network.isServer;
                if (value)
                    return true;
                value = Network.isClient ? owner == Network.localClient.Id : true;
                return value;
            }
        }
        public MessageSendMode reliability;
        public OwnershipFlag ownershipFlag;

        private void Awake() {
            RegisterObservables();
        }

        private void FixedUpdate() {
            HandleObservables();
        }

        public enum OwnershipFlag : byte {
            Fixed,
            Shared,
            Server,
        }
    }   
}
