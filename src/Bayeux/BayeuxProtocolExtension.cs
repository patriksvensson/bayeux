namespace Bayeux
{
    public abstract class BayeuxProtocolExtension
    {
        public string Name { get; }

        protected BayeuxProtocolExtension(string name)
        {
            Name = name;
        }

        public abstract bool TryExtendOutgoing(IBayeuxMessage message, out object extension);
    }
}
