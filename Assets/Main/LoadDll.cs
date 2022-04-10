using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

public class LoadDll : MonoBehaviour
{
    void Start()
    {
        BetterStreamingAssets.Initialize();
        LoadGameDll();
        RunMain();
    }

    public static System.Reflection.Assembly gameAss;

    private void LoadGameDll()
    {
#if !UNITY_EDITOR
        // 只有打包后才需要加载 HotFix.dll
        AssetBundle dllAB = BetterStreamingAssets.LoadAssetBundle("huatuo");
        TextAsset dllBytes = dllAB.LoadAsset<TextAsset>("HotFix.bytes");
        gameAss = System.Reflection.Assembly.Load(dllBytes.bytes);
#else
        gameAss = Assembly.Load("HotFix");
#endif
    }

    public void RunMain()
    {
        if (gameAss == null)
        {
            UnityEngine.Debug.LogError("dll未加载");
            return;
        }
        var appType = gameAss.GetType("App");
        var mainMethod = appType.GetMethod("Main");
        mainMethod.Invoke(null, null);

        // 如果是Update之类的函数，推荐先转成Delegate再调用，如
        //var updateMethod = appType.GetMethod("Update");
        //var updateDel = System.Delegate.CreateDelegate(typeof(Action<float>), null, updateMethod);
        //updateMethod(deltaTime);
    }
}
