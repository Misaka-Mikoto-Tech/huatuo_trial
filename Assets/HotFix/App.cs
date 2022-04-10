using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Better;
using Better.StreamingAssets;
using Better.StreamingAssets.ZipArchive;
using UnityEngine.Scripting;

public class App
{
    public static int Main()
    {
        Debug.Log("hello,huatuo");

        // test load prefab
        AssetBundle prefabAB = BetterStreamingAssets.LoadAssetBundle("artres");
        if(prefabAB == null)
        {
            Debug.LogError("���� ab `artres` ʧ��");
            return 0;
        }

        GameObject prefab = prefabAB.LoadAsset<GameObject>("Prefab");
        if(prefab == null)
        {
            Debug.LogError("���� prefab `Prefab` ʧ��");
            return 0;
        }

        Object.Instantiate(prefab);

        return 0;
    }
}
