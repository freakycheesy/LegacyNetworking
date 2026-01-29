using Riptide;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegacyNetworking {
    [DefaultExecutionOrder(-100)]
    public partial class NetworkView : MonoBehaviour {
        public object instantiateKey { get; internal set; }
        public int viewId { get; internal set; } = -1;
        public ushort owner { get; internal set; } = 0;
        public bool isInstantiated { get; internal set; } = false;
        public bool isMine {
            get {
                bool value = ownershipOption.HasFlag(OwnershipOption.Server) || owner < 0 && Network.isServer;
                if (value)
                    return true;
                value = Network.isClient ? owner == Network.localClient.Id : true;
                return value;
            }
        }
        public MessageSendMode reliability;
        public OwnershipOption ownershipOption;
        private void Awake() {
            if (!isInstantiated)
                Network.AllocateView(this);
            RegisterObservables();
        }

        private void FixedUpdate() {
            HandleObservables();
            HandleOwner();
        }

        private void HandleOwner() {
            if (!Network.isServer)
                return;
            if(Network.localServer.TryGetClient(owner, out var _))
                return;
            TransferOwner(Network.localServer.Clients[0].Id);
        }

        public void RequestOwnership() {
            Message requestMessage = Message.Create(
                MessageSendMode.Reliable, NetworkMessages.RequestOwnerMessage
            );
            requestMessage.Add(viewId);
            Network.Send(requestMessage);
        }

        [MessageHandler((ushort)NetworkMessages.RequestOwnerMessage)]
        public static void Handle_RequestOwnership(ushort sender, Message message) {
            int viewId = message.GetInt();
            ushort requesterId = sender;
            if (Network.Views.TryGetValue((ushort)viewId, out var netView)) {
                if (netView.ownershipOption != OwnershipOption.Shared)
                    return;
                netView.TransferOwner(requesterId);
            }
        }

        public void TransferOwner(ushort newOwner) {
            Message transferMessage = Message.Create(
                MessageSendMode.Reliable, NetworkMessages.ChangeOwnerMessage               
            );
            transferMessage.Add(viewId).Add(newOwner);
            owner = newOwner;
            Network.SendToAll(transferMessage, asHost: true);
        }

        [MessageHandler((ushort)NetworkMessages.ChangeOwnerMessage)]
        public static void Handle_TransferOwner(Message message) {
            int viewId = message.GetInt();
            ushort newOwner = message.GetUShort();
            if (Network.Views.TryGetValue((ushort)viewId, out var netView))
                netView.owner = newOwner;
        }

        public enum OwnershipOption : byte {
            Fixed,
            Shared,
            Server,
        }
    }   
}
