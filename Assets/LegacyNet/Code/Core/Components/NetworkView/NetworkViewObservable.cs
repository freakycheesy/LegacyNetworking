using Riptide;
using System.Linq;
using UnityEngine;

namespace LegacyNetworking
{
    public partial class NetworkView : MonoBehaviour {
        [SerializeField] private Component[] _observables;
        public ObservableSearch observableSearch;
        public IObservable[] observables;
        public void RegisterObservables() {
            if (observableSearch.HasFlag(ObservableSearch.Auto))
                observables = GetComponents<IObservable>();
            foreach (var observable in _observables) {
                if (observable is not IObservable) {
                    Debug.Log($"{observable.GetType().FullName} DOES NOT IMPLEMENT IObservable INTERFACE", observable);
                    continue;
                }
                if(observables.Contains(observable as IObservable))
                    continue;
                observables = observables.Append(observable as IObservable).ToArray();
            }
            observables.ToList().RemoveAll(o => (o as MonoBehaviour).GetComponentInParent<NetworkView>() != this);
        }

        private void HandleObservables() {
            if (!isMine)
                return;
            SerializeView();
        }

        private void SerializeView() {
            if (isMine) {
                _observedMessage = Message.Create(reliability, NetworkMessages.StreamMessage);
                _observedMessage.Add(viewId);
            }
            foreach (var observable in observables) {
                observable.OnSerializeView(ref _observedMessage, isMine);
            }
            if (Network.isClient && isMine)
                Network.localClient.Send(_observedMessage);
        }

        private Message _observedMessage;

        [MessageHandler((ushort)NetworkMessages.StreamMessage, (byte)NetworkTags.Group)]
        internal static void OnStreamMessage_Server(ushort sender, Message received) {
            var message = received;
            Network.localServer.SendToAll(message, sender);
        }

        [MessageHandler((ushort)NetworkMessages.StreamMessage, (byte)NetworkTags.Group)]
        internal static void OnStreamMessage_Client(Message received) {
            var view = Network.Views[received.GetUShort()];
            view._observedMessage = received;
            view.SerializeView();
        }
    }
    public enum ObservableSearch : byte {
        Auto,
        Manual
    }
}
