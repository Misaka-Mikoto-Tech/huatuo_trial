using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace HuaTuo.Editor.GlobalManagers
{
    public class GlobalManagersFileReader
    {
        static string filePath = Application.streamingAssetsPath + "/globalgamemanagers";

        static UnityBinFile binFile;

        [MenuItem("HuaTuo/ReadGlobalManagers")]
        static void ReadFile()
        {
            binFile = new UnityBinFile(filePath);
            binFile.Load();

            ScriptsData scriptsData = binFile.scriptsData;
            scriptsData.dllNames.Add("Dummy_.dll");
            scriptsData.dllTypes.Add(16);
            binFile.scriptsData = scriptsData;

            binFile.RebuildAndFlushToFile(filePath + "_new");
        }

        static void WriteFile()
        {

        }
    }
}

