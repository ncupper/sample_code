using System;
using System.IO;

using UnityEditor;

using UnityEngine;
//using GameAnalyticsSDK;

#pragma warning disable 649

namespace misc.editor
{
    internal class BundleVersionChecker
    {

        private const string ClassName = "CurrentBundleVersion";

        private const string TargetCodeFile = "Assets/Scripts/misc/" + ClassName + ".cs";

        private static string Platform
        {
            get
            {
#if UNITY_IOS
            return "iOS";
#else
                //if (Application.platform == RuntimePlatform.WindowsPlayer) return "StandaloneWindows64";
                return "Android";
#endif
            }
        }
        [MenuItem("Helper/Incriment build version", false, 1)]
        public static void MenuIncrimentBuildVersion()
        {
            IncrimentVersion(false, true);
        }

        [MenuItem("Helper/Incriment minor version", false, 2)]
        public static void MenuIncrimentMinorVersion()
        {
            IncrimentVersion(true, false);
        }

        [MenuItem("Helper/Check version", false, 3)]
        public static void MenuCheckVersion()
        {
            Check();
        }

        [MenuItem("Helper/Reset build version", false, 1)]
        public static void MenuResetBuildVersion()
        {
            Reset();
        }

        [MenuItem("Helper/Build AssetBundles", false, 21)]
        public static void BuildAssetBundles()
        {
            DoBuildAssetBundles();
        }

        [MenuItem("Helper/Clear Cache", false, 22)]
        public static void ClearCache()
        {
            Caching.ClearCache();
            Debug.Log("Cache clearing done.");
        }

        private static string IncrimentVersion(bool minor, bool build)
        {
            int buildVersion = Math.Max(PlayerSettings.Android.bundleVersionCode, Parse(PlayerSettings.iOS.buildNumber));

            if (build)
            {
                buildVersion++;
            }
            PlayerSettings.Android.bundleVersionCode = buildVersion;
            PlayerSettings.iOS.buildNumber = buildVersion.ToString();

            var minorVersion = 0;
            var majorVersion = 0;
            string[] vers = PlayerSettings.bundleVersion.Split('.');
            if (vers.Length >= 2)
            {
                minorVersion = Parse(vers[1]);
                majorVersion = Parse(vers[0]);
            }
            if (minor)
            {
                minorVersion++;
            }

            string version = majorVersion + "." + minorVersion + "." + buildVersion;

            if (minor || build)
            {
                Debug.Log("Update version " + CurrentBundleVersion.Version + " to " + version);
                CreateNewBuildVersionClassFile(version);
                AssetDatabase.Refresh();
            }

            PlayerSettings.bundleVersion = version;
            if (minor || build)
            {
                Check();
            }
            return version;
        }

        public static void Check()
        {
            string version = IncrimentVersion(false, false);

            if (CurrentBundleVersion.Version != version)
            {
                Debug.Log("Update version " + CurrentBundleVersion.Version + " to " + version);
                CreateNewBuildVersionClassFile(version);
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.Log("Current version " + CurrentBundleVersion.Version);
            }
            /*for (int i = 0; i < GameAnalytics.SettingsGA.Build.Count; ++i)
        {
            GameAnalytics.SettingsGA.Build[i] = buildVersion.ToString();
        }
        EditorUtility.SetDirty(GameAnalytics.SettingsGA);*/
        }

        private static string Reset()
        {
            var buildVersion = 1;

            PlayerSettings.Android.bundleVersionCode = buildVersion;
            PlayerSettings.iOS.buildNumber = buildVersion.ToString();

            var minorVersion = 0;
            var majorVersion = 1;

            string version = "1." + majorVersion + "." + minorVersion;

            Debug.Log("Reset version " + CurrentBundleVersion.Version + " to " + version);
            CreateNewBuildVersionClassFile(version + "." + buildVersion);
            AssetDatabase.Refresh();

            PlayerSettings.bundleVersion = version;
            return version;
        }

        private static void CreateNewBuildVersionClassFile(string bundleVersion)
        {
            using (var writer = new StreamWriter(TargetCodeFile, false))
            {
                try
                {
                    string code = GenerateCode(bundleVersion);
                    writer.WriteLine("{0}", code);
                }
                catch (Exception ex)
                {
                    string msg = " threw:\n" + ex;
                    Debug.LogError(msg);
                    EditorUtility.DisplayDialog("Error when trying to regenerate class", msg, "OK");
                }
            }
        }

        private static string GenerateCode(string bundleVersion)
        {
            string code = "public static class " + ClassName + "\r\n{\r\n";
            code += string.Format("\tpublic static readonly string Version = \"{0}\";", bundleVersion);
            code += "\r\n}\r\n";
            return code;
        }

        public static int Parse(string s)
        {
            var res = 0;
            if (!string.IsNullOrEmpty(s))
            {
                var k = 1;
                for (int i = s.Length - 1; i >= 0; --i)
                {
                    char ch = s[i];
                    if (ch >= '0' && ch <= '9')
                    {
                        res += (s[i] - '0') * k;
                    }
                    else if (ch == '-')
                    {
                        res = -res;
                        break;
                    }
                    else
                    {
                        break;
                    }
                    k *= 10;
                }
            }
            return res;
        }

        private static void DoBuildAssetBundlesSizeJson(string[] filesPath)
        {
            /*DbBundlesInfo bundlesInfo = new DbBundlesInfo();
        bundlesInfo.BundlesInfo = new List<DbBundleSize>();
        foreach (string tmpPath in filesPath) {
            FileInfo fileInfo = new FileInfo(tmpPath);

            if (fileInfo.Name.Contains(".manifest"))
                continue;
            if (fileInfo.Name.Contains(".meta"))
                continue;

            DbBundleSize bundleSize = new DbBundleSize();
            bundleSize.Name = fileInfo.Name;
            bundleSize.Size = fileInfo.Length;
            bundlesInfo.BundlesInfo.Add(bundleSize);            
        }

        List<string> very = new List<string>();
        List<string> litle = new List<string>();

        long db = 0;
        var old = JsonUtility.FromJson<DbBundlesInfo>(Resources.Load<TextAsset>("Params/bundlesInfo" + Platform).text);
        for (int i = 0; i < bundlesInfo.BundlesInfo.Count; ++i)
        {
            var b = bundlesInfo.BundlesInfo[i];
            for (int j = 0; j < old.BundlesInfo.Count; ++j)
            {
                if (old.BundlesInfo[j].Name == b.Name)
                {
                    db += b.Size - old.BundlesInfo[j].Size;
                    float delta = b.Size / (float)old.BundlesInfo[j].Size;
                    if (delta < 0.95f || delta > 1.05f)
                    {
                        very.Add(b.Name);
                    }
                    else if (!Helper.IsEqual(delta, 1, 0.005f))
                    {
                        litle.Add(b.Name);
                    }
                    break;
                }
            }
        }
        
        var path = Application.dataPath + "/Resources/Params/bundlesInfo" + Platform + ".json";
        var bytes = Encoding.UTF8.GetBytes(JsonUtility.ToJson(bundlesInfo));
        var f = File.Create(path);
        f.Write(bytes, 0, bytes.Length);
        f.Flush();
        f.Close();

        Debug.Log("change size: " + db + "B = (" + (db / 1024 / 1024) + "Mb)");
        var output = "very (" + very.Count + "): ";
        for (int i = 0; i < very.Count; ++i)
        {
            output += very[i] + ",";
        }
        Debug.Log(output);
            
        output = "little (" + litle.Count + "): ";
        for (int i = 0; i < litle.Count; ++i)
        {
            output += litle[i] + ",";
        }
        Debug.Log(output);*/
        }

        public static void DoBuildAssetBundles()
        {
            /*string outputPath = Application.dataPath + "/Bundles/" + EditorUserBuildSettings.activeBuildTarget + "/7";        
        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.Refresh();
        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.ForceRebuildAssetBundle & BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);


        var rootPath = Directory.GetParent(Application.dataPath) + "/Bundles2Server/"; 
        var pagesDir = rootPath + EditorUserBuildSettings.activeBuildTarget + "/pages";
        if (!Directory.Exists(pagesDir))
            Directory.CreateDirectory(pagesDir);
        var m3Dir = rootPath + EditorUserBuildSettings.activeBuildTarget + "/m3";
        if (!Directory.Exists(m3Dir))
            Directory.CreateDirectory(m3Dir);
        var otherDir = rootPath + EditorUserBuildSettings.activeBuildTarget + "/other";
        if (!Directory.Exists(otherDir))
            Directory.CreateDirectory(otherDir);

        var bundles = JsonUtility.FromJson<DbBundles>(Resources.Load<TextAsset>("Params/bundles").text);

        var ar = Directory.GetFiles(outputPath);
        DoBuildAssetBundlesSizeJson(ar);
        for (int i = 0; i < ar.Length; ++i)
        {
            var fn = Path.GetFileName(ar[i]);
            if (fn == null)
                continue;
            if (fn.StartsWith("page_"))
            {
                File.Copy(ar[i], pagesDir + "/" + fn, true);
            }
            else if (fn.StartsWith("mechanics_"))
            {
                for (int b = 0; b < bundles.Bundles.Count; ++b)
                {
                    var bPath = bundles.Bundles[b].Name + ".";
                    if (fn == bundles.Bundles[b].Name || fn.StartsWith(bPath))
                    {
                        if (!Directory.Exists(m3Dir + "/" + bundles.Bundles[b].Version))
                            Directory.CreateDirectory(m3Dir + "/" + bundles.Bundles[b].Version);
                        File.Copy(ar[i], m3Dir + "/" + bundles.Bundles[b].Version + "/" + fn, true);
                        break;
                    }
                }
            }
            else
            {
                File.Copy(ar[i], otherDir + "/" + fn, true);
            }                
		}*/
        }
    }
}
