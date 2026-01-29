using Riptide;

namespace LegacyNetworking
{
    public interface IObservable
    {
        public void OnSerializeView(Message stream, bool isWriting);
    }
}
