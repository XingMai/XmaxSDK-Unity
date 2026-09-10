namespace Xmax.SDK
{
    /// <summary>
    /// 共享配置和 HTTP 服务的 SDK 入口；每次创建客户端都会应用进程级日志选项。
    /// </summary>
    /// <remarks>
    /// 最近创建的客户端决定进程级日志选项；从已有客户端创建管理器不会再次修改日志配置。
    /// </remarks>
    public sealed class XmaxClient
    {
        private readonly IApiService _api;

        /// <summary>
        /// 客户端创建时使用的不可变配置。
        /// </summary>
        public XmaxConfiguration Configuration { get; }

        /// <summary>
        /// 创建客户端并应用全局日志配置；API key 在发起在线请求时校验。
        /// </summary>
        /// <param name="configuration">客户端的不可变服务地址、凭证和日志配置。</param>
        public XmaxClient(XmaxConfiguration configuration)
        {
            Configuration = configuration ?? throw new XmaxException(
                XmaxErrorCode.InvalidConfiguration,
                "Xmax configuration is required.");
            _api = new ApiService(Configuration);
            XmaxLogger.Configure(Configuration.LoggerOptions);
        }

        /// <summary>
        /// 使用共享客户端配置和 HTTP 服务创建独立的实时管理器。
        /// </summary>
        /// <param name="options">当前管理器使用的实时模型配置。</param>
        /// <returns>具有独立连接、生成和本地流状态的管理器。</returns>
        public XmaxRealtimeManager CreateRealtimeManager(RealtimeConfiguration options)
        {
            return new XmaxRealtimeManager(Configuration, options,
                new RealtimeSessionService(_api), null);
        }

        /// <summary>
        /// 创建用于查询模型限制和推荐编码尺寸的媒体服务。
        /// </summary>
        /// <returns>无需在线请求的媒体配置服务。</returns>
        public MediaService CreateMediaService() => new MediaService();
    }
}
