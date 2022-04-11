using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.IO;
using System;
using UnityEditor.UnityLinker;
using System.Reflection;
using UnityEditor.Il2Cpp;
#if UNITY_ANDROID
using UnityEditor.Android;
#endif

namespace HuaTuo
{
    public class HuaTuo_BuildProcessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
#if UNITY_ANDROID
        , IPostGenerateGradleAndroidProject
#endif
        , IProcessSceneWithReport, IFilterBuildAssemblies, IPostBuildPlayerScriptDLLs, IUnityLinkerProcessor, IIl2CppProcessor
    {
        /// <summary>
        /// ��Ҫ��Prefab�Ϲҽű����ȸ�dll�����б�����Ҫ�ҵ�Prefab�ϵĽű����Բ���������
        /// �����������dll��ʹ��ѡ�� AnyPlatform Ҳ���ڴ�������б��ų�
        /// 
        /// ���������ע�⣡�� ��Ҫ�ҽű���dll��������ñ�ģ���Ϊ����б��޷��ȸ������ߺ�ɾ�������ĳЩ�ǹҽű�dllû���⣩
        /// </summary>
        static List<string> monoDllNames = new List<string>() { "HotFix.dll"};

        static MethodInfo s_BuildReport_AddMessage;

        int IOrderedCallback.callbackOrder => 0;

        static HuaTuo_BuildProcessor()
        {
            s_BuildReport_AddMessage = typeof(BuildReport).GetMethod("AddMessage", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        void IPreprocessBuildWithReport.OnPreprocessBuild(BuildReport report)
        {
            if (!Application.isBatchMode && !EditorUtility.DisplayDialog("ȷ��", "���� Build ֮ǰ�ȴ�� AssetBundle\r\n�Ƿ����?", "����", "ȡ��"))
            {
                s_BuildReport_AddMessage.Invoke(report, new object[] { LogType.Exception, "�û�ȡ��", "BuildFailedException" });
                return;
            }
        }

        string[] IFilterBuildAssemblies.OnFilterAssemblies(BuildOptions buildOptions, string[] assemblies)
        {
            // ���ȸ�dll�Ӵ���б����Ƴ�
            List<string> newNames = new List<string>(assemblies.Length);

            foreach(string assembly in assemblies)
            {
                bool found = false;
                foreach(string removeName in monoDllNames)
                {
                    if(assembly.EndsWith(removeName, StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }

                if(!found)
                    newNames.Add(assembly);
            }
            
            return newNames.ToArray();
        }


        [Serializable]
        public class ScriptingAssemblies
        {
            public List<string> names;
            public List<int> types;
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild(BuildReport report)
        {
            AddBackHotFixAssembliesToJson(report, null);
        }

#if UNITY_ANDROID
        void IPostGenerateGradleAndroidProject.OnPostGenerateGradleAndroidProject(string path)
        {
            // ���� Android ƽ̨�� OnPostprocessBuild ����ʱ�Ѿ������� apk �ļ��������Ҫ��ǰ����
            AddBackHotFixAssembliesToJson(null, path);
        }
#endif

        private void AddBackHotFixAssembliesToJson(BuildReport report, string path)
        {
            /*
             * ScriptingAssemblies.json �ļ��м�¼�����е�dll���ƣ����б�����Ϸ����ʱ�Զ����أ�
             * ���ڴ��б��е�dll����Դ�����л�ʱ�޷����ҵ�������
             * ��� OnFilterAssemblies ���Ƴ�����Ŀ��Ҫ�ټӻ���
             */
#if UNITY_ANDROID
            string[] jsonFiles = new string[] { "Temp/gradleOut/unityLibrary/src/main/assets/bin/Data/ScriptingAssemblies.json" }; // report.files ������ Temp/gradleOut ��Ŀ¼
#else
            // ֱ�ӳ��������vs����ʱ·����ͬ��report.summary.outputPath ��¼����ǰ��·��
            string[] jsonFiles = Directory.GetFiles(Path.GetDirectoryName(report.summary.outputPath), "ScriptingAssemblies.json", SearchOption.AllDirectories);
#endif

            if (jsonFiles.Length == 0)
            {
                Debug.LogError("can not find file ScriptingAssemblies.json");
                return;
            }

            foreach (string file in jsonFiles)
            {
                string content = File.ReadAllText(file);
                ScriptingAssemblies scriptingAssemblies = JsonUtility.FromJson<ScriptingAssemblies>(content);
                foreach (string name in monoDllNames)
                {
                    if(!scriptingAssemblies.names.Contains(name))
                    {
                        scriptingAssemblies.names.Add(name);
                        scriptingAssemblies.types.Add(16); // user dll type
                    }
                }
                content = JsonUtility.ToJson(scriptingAssemblies);

                File.WriteAllText(file, content);
            }
        }


        void IProcessSceneWithReport.OnProcessScene(Scene scene, BuildReport report)
        {

        }

        void IPostBuildPlayerScriptDLLs.OnPostBuildPlayerScriptDLLs(BuildReport report)
        {

        }

        string IUnityLinkerProcessor.GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
        {
            return String.Empty;
        }

        void IUnityLinkerProcessor.OnBeforeRun(BuildReport report, UnityLinkerBuildPipelineData data)
        {
        }

        void IUnityLinkerProcessor.OnAfterRun(BuildReport report, UnityLinkerBuildPipelineData data)
        {
        }

        void IIl2CppProcessor.OnBeforeConvertRun(BuildReport report, Il2CppBuildPipelineData data)
        {

        }


#if UNITY_IOS
    // hook UnityEditor.BuildCompletionEventsHandler.ReportPostBuildCompletionInfo() ? ��Ϊû�� mac ���ƽ̨��˲����
#endif
    }

}
