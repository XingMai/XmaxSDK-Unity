namespace Xmax.SDK
{
    public sealed class XmaxClient
    {
        public XmaxConfiguration Configuration { get; }

        public XmaxClient(XmaxConfiguration configuration)
        {
            Configuration = configuration ?? throw new XmaxException(
                XmaxErrorCode.InvalidConfiguration,
                "Xmax configuration is required.");
        }

        public XmaxRealtimeManager CreateRealtimeManager(RealtimeConfiguration options)
        {
            Configuration.Validate();
            return new XmaxRealtimeManager(Configuration, options);
        }
    }
}
