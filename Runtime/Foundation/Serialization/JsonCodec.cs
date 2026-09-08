using UNBridgeLib.LitJson;

namespace Xmax.SDK
{
    // Keep the bundled serializer behind Foundation; services and room protocol
    // do not depend on a particular RTC vendor's JSON library.
    internal static class JsonCodec
    {
        internal static string Encode(object value) => JsonMapper.ToJson(value);
        internal static JsonValue Decode(string text) => JsonValue.Wrap(JsonMapper.ToObject(text));
    }
    internal sealed class JsonValue
    {
        private readonly JsonData _data;
        private JsonValue(JsonData data) { _data = data; }
        internal static JsonValue Wrap(JsonData data) => data == null ? null : new JsonValue(data);
        internal bool IsObject => _data.IsObject;
        internal bool IsString => _data.IsString;
        internal bool IsBoolean => _data.IsBoolean;
        internal bool IsInt => _data.IsInt;
        internal bool IsLong => _data.IsLong;
        internal bool ContainsKey(string key) => _data.IsObject && _data.ContainsKey(key);
        internal JsonValue this[string key] => Wrap(_data[key]);
        public static explicit operator string(JsonValue value) => (string)value._data;
        public static explicit operator bool(JsonValue value) => (bool)value._data;
        public static explicit operator int(JsonValue value) => (int)value._data;
        public static explicit operator long(JsonValue value) => (long)value._data;
    }
}
