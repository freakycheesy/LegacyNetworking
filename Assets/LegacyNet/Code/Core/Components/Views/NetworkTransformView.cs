using Riptide;
using UnityEngine;

namespace LegacyNetworking
{
    public class NetworkTransformView : MonoBehaviour, IObservable
    {
        public bool syncPosition = true;
        public bool syncRotation = true;
        public bool syncScale = false;
        public float teleportDistance = 3;
        private Vector3 networkPosition;
        private Quaternion networkRotation;
        private Vector3 networkScale;
        private bool _isWriting;

        void Update() {
            if (_isWriting)
                return;
            if (syncPosition)
                transform.position = Vector3.Distance(transform.position, networkPosition) < teleportDistance ? Vector3.MoveTowards(transform.position, networkPosition, Time.deltaTime * teleportDistance) : networkPosition;
            if (syncRotation)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, networkRotation, Time.deltaTime * 100);
            if (syncScale)
                transform.localScale = networkScale;
        }

        public void OnSerializeView(ref Message stream, bool isWriting) {
            _isWriting = isWriting;
            if (_isWriting) {
                if (syncPosition)
                    stream.Add(transform.position);
                if (syncRotation)
                    stream.Add(transform.rotation);
                if (syncScale)
                    stream.Add(transform.localScale);
            }
            else {
                if (syncPosition)
                    networkPosition = stream.GetVector3();
                if (syncRotation)
                    networkRotation = stream.GetQuaternion();
                if (syncScale)
                    networkScale = stream.GetVector3();
            }
        }
    }
}
