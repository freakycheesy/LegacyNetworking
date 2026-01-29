using Riptide;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LegacyNetworking
{
    public partial class NetworkView : MonoBehaviour {
        [SerializeField] private Component[] _observables;
        public ObservableSearch observableSearch;
        public List<IObservable> observables = new();
        public void RegisterObservables() {
            if (observableSearch.HasFlag(ObservableSearch.Auto))
                observables = GetComponents<IObservable>().ToList();
            foreach (var observable in _observables) {
                if (observable is not IObservable) {
                    Debug.Log($"{observable.GetType().FullName} DOES NOT IMPLEMENT IObservable INTERFACE", observable);
                    continue;
                }
                if(observables.Contains(observable as IObservable))
                    continue;
                observables.Add((IObservable)observable);
            }
            observables.ToList().RemoveAll(o => (o as MonoBehaviour).GetComponentInParent<NetworkView>() != this);
        }

        private void HandleObservables() {
            if (!isMine)
                return;
            SerializeView(true);
        }

        private void SerializeView(bool isWriting) {
            var observedTemp = _observedMessage;
            if (isWriting) {
                _observedMessage = Message.Create(reliability, NetworkMessages.StreamMessage);
                _observedMessage.Add(viewId);
            }
            foreach (var observable in observables) {
                observable.OnSerializeView(_observedMessage, isWriting);
            }
            if (Network.isClient && isWriting && _observedMessage != observedTemp)
                Network.Send(_observedMessage);
        }

        private Message _observedMessage;

        [MessageHandler((ushort)NetworkMessages.StreamMessage)]
        internal static void OnStreamMessage_Server(ushort sender, Message received) {
            var message = received;
            if(sender != Network.Views[received.GetUShort()].owner)
                return;
            Network.SendToAll(message, (short)sender);
        }

        [MessageHandler((ushort)NetworkMessages.StreamMessage)]
        internal static void OnStreamMessage_Client(Message received) {
            var view = Network.Views[received.GetUShort()];
            view._observedMessage = received;
            view.SerializeView(false);
        }
    }
    public enum ObservableSearch : byte {
        Auto,
        Manual
    }
}
