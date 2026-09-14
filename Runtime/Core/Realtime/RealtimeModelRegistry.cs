using System;
using System.Collections.Generic;

namespace Xmax.SDK
{
    /// <summary>
    /// 集中登记内置模型的标识、服务端名称和能力参数，供枚举入口与名称入口共同查询。
    /// </summary>
    internal static class RealtimeModelRegistry
    {
        // 每个模型只在此登记一次，名称索引由同一份记录生成。
        private static readonly IReadOnlyDictionary<RealtimeModel, ModelDefinition> Definitions =
            new Dictionary<RealtimeModel, ModelDefinition>
            {
                [RealtimeModel.X2_0] = new ModelDefinition(
                    name: "x2.0",
                    capabilities: new RealtimeModelCapabilities(
                        minimumPixels: 600000,
                        maximumPixels: 1280000,
                        dimensionAlignment: 32,
                        defaultCameraVideoFormat: new RealtimeVideoFormat(832, 1472, 30))),

                [RealtimeModel.X2_0_Pro] = new ModelDefinition(
                    name: "x2.0-pro",
                    capabilities: new RealtimeModelCapabilities(
                        minimumPixels: 600000,
                        maximumPixels: 2100000,
                        dimensionAlignment: 32,
                        defaultCameraVideoFormat: new RealtimeVideoFormat(1024, 1920, 30),
                        resolutionBuckets: new[]
                        {
                            new RealtimeVideoSize(1024, 1920),
                            new RealtimeVideoSize(1920, 1024)
                        }))
            };

        // 派生索引保持名称查询与枚举查询一致，不维护第二份模型配置。
        private static readonly IReadOnlyDictionary<string, ModelDefinition> DefinitionsByName = CreateNameIndex();

        /// <summary>
        /// 查询已登记的内置模型定义。
        /// </summary>
        /// <param name="model">SDK 内置实时模型标识。</param>
        /// <returns>包含服务端名称和能力参数的共享不可变定义。</returns>
        /// <exception cref="ArgumentOutOfRangeException">模型标识没有对应的注册记录。</exception>
        internal static ModelDefinition Get(RealtimeModel model)
        {
            if (Definitions.TryGetValue(model, out var definition))
            {
                return definition;
            }

            throw new ArgumentOutOfRangeException(nameof(model), model, "Realtime model is not registered.");
        }

        /// <summary>
        /// 按规范化后的服务端名称查找内置模型。
        /// </summary>
        /// <param name="name">去除首尾空白后的服务端名称，区分大小写。</param>
        /// <returns>匹配的共享定义；未知名称或 null 返回 null。</returns>
        internal static ModelDefinition Find(string name)
        {
            return name != null && DefinitionsByName.TryGetValue(name, out var definition) ? definition : null;
        }

        /// <summary>
        /// 从枚举注册记录生成服务端名称索引，重复名称会在初始化时被拒绝。
        /// </summary>
        /// <returns>采用序号比较的模型名称索引。</returns>
        /// <exception cref="ArgumentException">多个模型登记了相同的服务端名称。</exception>
        private static IReadOnlyDictionary<string, ModelDefinition> CreateNameIndex()
        {
            var index = new Dictionary<string, ModelDefinition>(StringComparer.Ordinal);
            foreach (var definition in Definitions.Values)
            {
                index.Add(definition.Name, definition);
            }

            return index;
        }
    }
}
