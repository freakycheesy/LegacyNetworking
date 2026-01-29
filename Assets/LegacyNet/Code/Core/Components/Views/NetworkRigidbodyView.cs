using Riptide;
using System.Net.Sockets;
using UnityEngine;

namespace LegacyNetworking
{
    public class NetworkRigidbodyView : NetworkTransformView
    {
        private Rigidbody m_Rigidbody;
        private void Awake() {
            m_Rigidbody = GetComponent<Rigidbody>();
        }
        public override void OnSerializeView(Message stream, bool isWriting) {
            base.OnSerializeView(stream, isWriting);
            if (isWriting) {
                stream.Add(m_Rigidbody.velocity);
                stream.Add(m_Rigidbody.angularVelocity);
                stream.Add(m_Rigidbody.isKinematic);
            }
            else {
                m_Rigidbody.velocity = stream.GetVector3();
                m_Rigidbody.angularVelocity = stream.GetVector3();
                m_Rigidbody.isKinematic = stream.GetBool();
            }
        }
    }
}
