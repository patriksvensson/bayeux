 // ReSharper disable once CheckNamespace
namespace Bayeux
{
    public interface IBayeuxMessage
    {
        string Channel { get; }

        string ClientId { get; }

        object Data { get; }
    }
}
