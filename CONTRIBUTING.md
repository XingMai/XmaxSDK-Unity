# XmaxSDK Unity 开发约定

## 分支管理

- `main`：稳定版本与发布标签所在分支。
- `develop`：集成分支，完成验证的 feature 提交进入此分支。
- `feature/yueting-v<下一版本>`：从 `develop` 创建的版本开发分支，例如 `feature/yueting-v0.1.1`。
- 日常改动在 feature 分支提交。沿用 iOS 的发布方式，发布验证通过后，将 `develop` 和 `main` 原子快进到已推送的 feature 提交。
- 分支存在分歧时，先在 feature 分支整合并重新验证；不强制覆盖远端历史。
- 正式版本标签采用 `0.1.1` 格式，不加 `v`。标签必须指向通过验证的 `main` 提交。

## 中文提交信息

使用 Conventional Commits 前缀，标题描述使用中文，例如：

```text
feat: 添加实时生命周期协调器
fix: 修复断开连接后旧任务覆盖新状态的问题
chore: 同步 SDK 版本至 0.1.1
docs: 补充 Unity 接入说明
```

允许 `feat`、`fix`、`refactor`、`perf`、`test`、`docs`、`build`、`ci`、`chore`、`revert`，可带作用域和破坏性变更标记。合并前的本地 CI 检查本次 feature 新增提交的标题。

## 本地 CI/CD

与 iOS 仓库一致，`.cicd/` 保存本地工具并由 Git 忽略，`.build/` 保存构建日志及产物。换电脑时需单独同步 `.cicd/`；仓库没有托管的云端 CI。

主要入口：

```bash
# 修改代码期间可单独验证，不移动任何分支
./.cicd/validate.sh

# 在干净的 feature 分支同步版本；不自动提交
./.cicd/ci-prepare.sh 0.1.1
git add package.json Runtime/XmaxSdkInfo.cs
git commit -m "chore: 同步 SDK 版本至 0.1.1"
git push -u origin HEAD

# 验证源码接入和 Android ARM64 构建，再确认同步分支
./.cicd/ci-merge.sh

# 在 main 创建并推送版本标签
git switch main
git tag -a 0.1.1 -m "release: 发布 XmaxSDK Unity 0.1.1"
git push origin 0.1.1

# 准备中文发布说明后，从标签打包、验证压缩包接入并发布
./.cicd/cd-github-release.sh 0.1.1
```

版本准备同步 `package.json` 与 `Runtime/XmaxSdkInfo.cs`。发布产物为可通过 Unity Package Manager 安装的 `ai.xmax.sdk-<版本>.tgz`，并附 SHA-256 校验文件。成功发布后创建下一 patch 版本的本地 feature 分支。

验证使用独立的最小 Unity 工程，不依赖 PICO 包和相邻的 PicoDemo，也不会创建在线 Session。编译和打包不能替代真机 RTC 联调。
