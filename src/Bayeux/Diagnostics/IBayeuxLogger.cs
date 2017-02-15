namespace Bayeux.Diagnostics
{
    public interface IBayeuxLogger
    {
        void Write(BayeuxLogLevel level, string format, params object[] args);
    }
}
