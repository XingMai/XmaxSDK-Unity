namespace Xmax.SDK
{
    public sealed class XmaxClient
    {
        private readonly IApiService _api;
        public XmaxConfiguration Configuration { get; }

        public XmaxClient(XmaxConfiguration configuration)
        {
            Configuration = configuration ?? throw new XmaxException(
                XmaxErrorCode.InvalidConfiguration,
                "Xmax configuration is required.");
            _api = new ApiService(Configuration);
        }

        public XmaxRealtimeManager CreateRealtimeManager(RealtimeConfiguration options)
        {
            Configuration.Validate();
            return new XmaxRealtimeManager(Configuration, options,
                new RealtimeSessionService(_api), new StreamController(new RtcManager()));
        }

        public MediaService CreateMediaService() => new MediaService();
    }
}
