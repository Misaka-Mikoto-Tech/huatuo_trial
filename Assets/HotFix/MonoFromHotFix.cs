using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestInfo
{
    public int subA;
    public string subB;
}

public class MonoFromHotFix : MonoBehaviour
{
    public int iA;
    public float fB;
    public string strC;
    public List<int> lstD;
    public TestInfo testInfo;
    public int iA2;
    //public GameObject goHHH;

    void Start()
    {
        Debug.Log("");
        Debug.Log($"MonoFromHotFix.Start: \r\niA: {iA} \r\nfB: {fB} \r\nstrC: {strC} \r\nlstD[0]: {lstD[0]} \r\ntestInfo.SubB: {testInfo.subB} \r\niA2: {iA2}");
        //Debug.Log($"HHH:{goHHH.name}");
    }

}
