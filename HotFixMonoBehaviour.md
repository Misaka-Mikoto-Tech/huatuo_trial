# 热更资源脚本工作流

本修改版的huatuo可以热更资源脚本，开发及打包工作流如下:

## 准备工作
  >将 `huatuo_repo/libil2cpp` 子目录或者 `https://github.com/Misaka-Mikoto-Tech/huatuo/libil2cpp` 下的文件覆盖 Unity 安装目录对应目录（详情见huatuo官方设置流程）

## 热更脚本设置及加载流程
1. 热更脚本的使用有两种方式, 可以任选一种
    - 在热更脚本目录放置 .asmdef 文件，让Unity自动生成 dll
    - 外部独立的 C# 工程，把dll生成目录设置到Unity项目内    

2. 根据项目Unity版本不同，打开文件 `Assets/Editor/HuaTuo/HuaTuo_BuildProcessor_2020_1_OR_NEWER.cs` 或 `HuaTuo_BuildProcessor_2019.cs`，将需要在资源上挂脚本的所在热更dll名称填入 `monoDllNames` 字段，示例:
    ```csharp
        static List<string> monoDllNames = new List<string>() { "HotFix.dll"};
    ```
3. 在挂热更脚本的任意一个资源加载之前，加载热更dll，示例代码如下:
    ```csharp
        #if !UNITY_EDITOR
            // 只有打包后才需要加载 HotFix.dll
            AssetBundle dllAB = BetterStreamingAssets.LoadAssetBundle("huatuo");
            TextAsset dllBytes = dllAB.LoadAsset<TextAsset>("HotFix.bytes");
            gameAss = System.Reflection.Assembly.Load(dllBytes.bytes);
        #endif

            // 加载热更dll之后再加载挂了热更脚本的资源
            AssetBundle resAB = BetterStreamingAssets.LoadAssetBundle("artRes");
            GameObject go = resAB.LoadAsset<GameObject>("myPrefab");
            Instantiate(go);
    ```
## 打包流程
正常打包即可，但有几个注意事项
* 需要把热更dll打包成ab，参考代码见 `Assets/Editor/HuaTuo/HuaTuoEditorHelper.cs`
* 建议打AB时不要禁用TypeTree，否则普通的AB加载方式会失败。（原因是对于禁用TypeTree的脚本，Unity为了防止二进制不匹配导致反序列化MonoBehaviour过程中进程Crash，会对脚本的签名进行校验，签名的内容是脚本FullName及TypeTree数据生成的Hash, 但由于我们的热更脚本信息不存在于打包后的安装包中，因此校验必定会失败）

* 如果必须要禁用TypeTree，一个变通的方法是禁止脚本的Hash校验, 此种情况下用户必须保证打包时代码与资源版本一致，否则可能会导致Crash，示例代码
    ```csharp
        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(path);
        req.SetEnableCompatibilityChecks(false); // 非public，需要通过反射调用
    ```

## 其它
热更dll无法游戏内Reload，因此下载热更dll后重启才能生效