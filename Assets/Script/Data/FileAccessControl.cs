using System.IO;
using UnityEngine;

public class FileAccessControl
{
    // 設定ファイル読み込み
    public void LoadSetthingData(ref AplSettings setthingData)
    {
        string workPath = Path.Combine(Application.persistentDataPath, "AplSettinhFile.json");
        string json = File.ReadAllText(workPath);
        setthingData=JsonUtility.FromJson<AplSettings>(json);
    }
    // 設定ファイル更新、生成
    public void SaveSetthingData(AplSettings aplSettings)
    {
        string workPath = Path.Combine(Application.persistentDataPath, "AplSettinhFile.json");
        string json = JsonUtility.ToJson(aplSettings, true);
        File.WriteAllText(workPath, json);
    }
}