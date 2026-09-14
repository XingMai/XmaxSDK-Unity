# XmaxSDK Unity 开发约定

## 分支管理

- `main`：稳定版本与发布标签所在分支。
- `develop`：集成分支，完成验证的 feature 提交进入此分支。
- `feature/yueting-v<下一版本>`：从 `develop` 创建的版本开发分支，例如 `feature/yueting-v1.0.0`。
- 日常改动在 feature 分支提交。沿用 iOS 的发布方式，发布验证通过后，将 `develop` 和 `main` 原子快进到已推送的 feature 提交。
- 分支存在分歧时，先在 feature 分支整合并重新验证；不强制覆盖远端历史。
- 正式版本标签采用 `1.0.0` 格式，不加 `v`。标签必须指向通过验证的 `main` 提交。

## 英文提交信息

后续提交使用英文标题和正文，标题遵循 Conventional Commits，例如：

```text
feat: add realtime lifecycle coordination
fix: prevent stale disconnects from overwriting connection state
chore: bump SDK version to 1.0.0
docs: expand Unity integration instructions
```

允许 `feat`、`fix`、`refactor`、`perf`、`test`、`docs`、`build`、`ci`、`chore`、`revert`，可带作用域和破坏性变更标记。合并前的本地 CI 检查本次 feature 新增提交的标题，要求使用可打印 ASCII 字符并包含英文字母。

所有分支的历史提交信息已统一为英文。CI 对待合并范围中的全部提交执行标题检查，不设置历史语言豁免。

## 代码排版与注释

这些约定适用于手写 SDK 代码（`Runtime/ThirdParty` 除外），基础排版由根目录 `.editorconfig` 描述。测试代码遵循相同排版，测试说明重点描述验证的行为，无需给每个测试补充 API 文档。不要为了格式统一修改厂商源码或生成文件。

- 使用 UTF-8、LF 和 4 个空格缩进。类型、方法、控制块的大括号单独成行。
- 方法之间保留一个空行，包括构造函数、表达式方法和接口方法。带文档注释的属性、事件、枚举成员也用空行分隔。
- 一行只写一条语句，不将多个赋值、分支、异常处理或资源释放操作压在同一行。简单自动属性可保留为 `public string Name { get; }`。
- 方法内部按校验、状态准备、外部调用、状态提交、通知及清理等实际逻辑分段。同一逻辑块保持连续，不机械地给每条语句加空行。
- 长参数列表和调用按参数换行；较长条件、三元表达式按含义分行，避免把多个判断堆在一行。
- 成员变量按实际职责分组，例如依赖与配置、连接资源、生成任务、渲染缓存、清理状态。组间留空行，必要时用简短的 `//` 注释说明；少量职责单一的字段不必强行分组。

SDK 类型、接口、属性、事件以及方法（包括内部实现和私有辅助方法）使用规范的 C# XML 文档注释。注释默认使用中文，标识符和协议字段保留英文；提交信息仍使用英文。

代码注释只说明 SDK 自身的职责和行为，不记录与其他平台对齐的过程。

- `<summary>` 说明职责或实际行为，不只重复方法名；使用独立的开始、正文和结束行。
- 每个参数使用名称完全一致的 `<param name="...">`，说明含义、单位、有效范围、空值或默认值语义。泛型参数使用 `<typeparam>`。
- 非 `void` 方法使用 `<returns>`，说明返回值；`Task` 说明何时完成，`Task<T>` 同时说明完成条件和结果。构造函数、`void` 方法不添加虚假的返回值说明，索引器使用 `<value>`。
- 会影响调用方使用方式的校验和取消异常使用 `<exception cref="...">`；线程要求、资源归属、数据复制、回调有效期、取消后的状态等通过 `<remarks>` 补充。
- 方法内部的 `//` 解释重入保护、资源释放顺序、兼容原因等必要背景；已有 XML 文档覆盖的内容不重复堆叠。

修改签名或行为时同步更新注释。纯排版和文档整理应保持运行逻辑不变，合并前检查差异并运行已有的 SDK 回归验证；不要为了格式改动新增重复实现的测试。

## 内置模型配置

模型的枚举标识、服务端名称和能力参数统一关联在 `Runtime/Core/Realtime/RealtimeModelRegistry.cs`。新增内置模型时，在 `RealtimeModel` 增加枚举项，并在注册表增加一条完整配置记录，包括像素上下界、尺寸对齐、默认 Camera 规格和可选固定分辨率。名称索引自动生成，不另写名称映射或按模型分支的业务逻辑。

`RealtimeModelCapabilities` 只保存能力数据和通用分辨率校验，不持有具体模型实例或固定的像素、对齐参数。模型名称查询、尺寸推荐和管理器均使用注册配置。回归测试会遍历全部枚举，检查注册是否完整、名称是否唯一、默认规格是否符合对应规则。

## 本地 CI/CD

与 iOS 仓库一致，`.cicd/` 保存本地工具并由 Git 忽略，`.build/` 保存构建日志及产物。换电脑时需单独同步 `.cicd/`；仓库没有托管的云端 CI。

主要入口：

```bash
# 修改代码期间可单独验证，不移动任何分支
./.cicd/validate.sh

# 在干净的 feature 分支同步版本；不自动提交
./.cicd/ci-prepare.sh 1.0.0
git add package.json Runtime/XmaxSdkInfo.cs
git commit -m "chore: bump SDK version to 1.0.0"
git push -u origin HEAD

# 验证源码接入和 Android ARM64 构建，再确认同步分支
./.cicd/ci-merge.sh

# 在 main 创建并推送版本标签
git switch main
git tag -a 1.0.0 -m "release: publish XmaxSDK Unity 1.0.0"
git push origin 1.0.0

# 准备中文发布说明后，从标签打包、验证压缩包接入并发布
./.cicd/cd-github-release.sh 1.0.0
```

版本准备同步 `package.json` 与 `Runtime/XmaxSdkInfo.cs`。发布产物为可通过 Unity Package Manager 安装的 `ai.xmax.sdk-<版本>.tgz`，并附 SHA-256 校验文件。成功发布后创建下一 patch 版本的本地 feature 分支。

验证使用独立的最小 Unity 工程，不依赖 PICO 包和相邻的 PicoDemo，也不会创建在线 Session。流程先运行 SDK 的 EditMode 回归测试，再检查公开接口和 Android IL2CPP/ARM64 构建；源码和 tgz 消费均运行同一组测试。编译和打包不能替代真机 RTC 联调。

实时 SDK 按 [架构说明](ARCHITECTURE.md) 的六层维护。原生 RTC 厂商类型限于 Foundation/RTC 和 ThirdParty，序列化厂商类型限于 Foundation/Serialization。新增生命周期行为优先使用 Tests/Editor 的 Session / Stream / RTC 替身验证，避免依赖付费在线 Session。
