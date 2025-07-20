using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class AutoVersionIncrement:IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        string currentVersion = PlayerSettings.bundleVersion;
        if(float.TryParse(currentVersion,out float ver))
        {
            ver += 0.01f;

            ver =Mathf.Round(ver*100f)/100f;

            PlayerSettings.bundleVersion = ver.ToString("F2");
            Debug.Log($"ビルド前にバージョンを自動更新しました。(Ver {PlayerSettings.bundleVersion})");
        }
        else
        {
            Debug.LogWarning("Version文字列の変換に失敗しました。");
        }
        PlayerSettings.Android.bundleVersionCode += 1;
    }

}