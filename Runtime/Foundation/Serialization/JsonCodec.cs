using UNBridgeLib.LitJson;

namespace Xmax.SDK
{
    /// <summary>
    /// 将厂商 JSON 库封装在 Foundation，避免业务层直接依赖其类型。
    /// </summary>
    internal static class JsonCodec
    {
        /// <summary>
        /// 将可序列化对象编码为 JSON。
        /// </summary>
        /// <param name="value">需要编码为 JSON 的对象。</param>
        /// <returns>对象的 JSON 文本。</returns>
        internal static string Encode(object value) => JsonMapper.ToJson(value);

        /// <summary>
        /// 解析 JSON 并转换为 SDK 内部包装类型。
        /// </summary>
        /// <param name="text">需要解析的 JSON 文本。</param>
        /// <returns>解析后的值；JSON null 对应 null。</returns>
        internal static JsonValue Decode(string text) => JsonValue.Wrap(JsonMapper.ToObject(text));
    }

    /// <summary>
    /// 封装 JSON 值的类型判断、对象取值和显式标量转换。
    /// </summary>
    internal sealed class JsonValue
    {
        private readonly JsonData _data;

        /// <summary>
        /// 保存底层 JSON 值供内部访问。
        /// </summary>
        /// <param name="data">底层序列化库的 JSON 值。</param>
        private JsonValue(JsonData data)
        {
            _data = data;
        }

        /// <summary>
        /// 包装底层 JSON 值并保留空值语义。
        /// </summary>
        /// <param name="data">底层序列化库的 JSON 值。</param>
        /// <returns>底层值为 null 时返回 null，否则返回包装对象。</returns>
        internal static JsonValue Wrap(JsonData data) => data == null ? null : new JsonValue(data);

        /// <summary>
        /// 当前值是否为 JSON 对象。
        /// </summary>
        internal bool IsObject => _data.IsObject;

        /// <summary>
        /// 当前值是否为字符串。
        /// </summary>
        internal bool IsString => _data.IsString;

        /// <summary>
        /// 当前值是否为布尔值。
        /// </summary>
        internal bool IsBoolean => _data.IsBoolean;

        /// <summary>
        /// 当前值是否为 32 位整数。
        /// </summary>
        internal bool IsInt => _data.IsInt;

        /// <summary>
        /// 当前值是否为 64 位整数。
        /// </summary>
        internal bool IsLong => _data.IsLong;

        /// <summary>
        /// 仅对 JSON 对象检查指定键是否存在。
        /// </summary>
        /// <param name="key">需要读取或检查的 JSON 对象成员名称。</param>
        /// <returns>当前值是对象且包含键时为 true。</returns>
        internal bool ContainsKey(string key) => _data.IsObject && _data.ContainsKey(key);

        /// <summary>
        /// 按键读取 JSON 对象成员；调用前应检查对象类型和键是否存在。
        /// </summary>
        /// <param name="key">需要读取或检查的 JSON 对象成员名称。</param>
        /// <value>指定键对应的包装值，JSON null 对应 null。</value>
        internal JsonValue this[string key] => Wrap(_data[key]);

        /// <summary>
        /// 按底层 JSON 类型执行显式字符串转换，类型不匹配时抛出异常。
        /// </summary>
        /// <param name="value">待解析或转换的 SDK 内部 JSON 值。</param>
        /// <returns>当前 JSON 值对应的字符串。</returns>
        public static explicit operator string(JsonValue value) => (string)value._data;

        /// <summary>
        /// 按底层 JSON 类型执行显式布尔值转换，类型不匹配时抛出异常。
        /// </summary>
        /// <param name="value">待解析或转换的 SDK 内部 JSON 值。</param>
        /// <returns>当前 JSON 值对应的布尔值。</returns>
        public static explicit operator bool(JsonValue value) => (bool)value._data;

        /// <summary>
        /// 按底层 JSON 类型执行显式32 位整数转换，类型不匹配时抛出异常。
        /// </summary>
        /// <param name="value">待解析或转换的 SDK 内部 JSON 值。</param>
        /// <returns>当前 JSON 值对应的32 位整数。</returns>
        public static explicit operator int(JsonValue value) => (int)value._data;

        /// <summary>
        /// 按底层 JSON 类型执行显式64 位整数转换，类型不匹配时抛出异常。
        /// </summary>
        /// <param name="value">待解析或转换的 SDK 内部 JSON 值。</param>
        /// <returns>当前 JSON 值对应的64 位整数。</returns>
        public static explicit operator long(JsonValue value) => (long)value._data;
    }
}
