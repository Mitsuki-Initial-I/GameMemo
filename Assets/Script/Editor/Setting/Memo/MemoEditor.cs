using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MemoEditor : EditorWindow
{
    private string folderPath;
    private string fileName = "default_memo";
    private string fileContent;
    private string workfilePath;

    private int selectedIndex = 0;
    private int b_selectedIndex = 0;
    private List<string> fileList = new List<string>();

    private bool isInitialized = false;
    private bool isOpenFile = false;

    [MenuItem("MyTool/Memo")]
    private static void ShowWindow()
    {
        var window = GetWindow<MemoEditor>("MemoEditor");
        window.minSize = new Vector2(600, 400);
        window.Show();
    }

    private void OnGUI()
    {
        // 初期処理
        if (!isInitialized)
        {
            fileList = new List<string>();
            LoadTextFile();
            isInitialized = true;
        }

        // ファイル読み込み
        SetthingFileData();

        // ファイル生成
        NewFile();

        // ファイル表示
        OpenFile();
    }

    // フォルダ内のファイル読み込み
    void LoadTextFile()
    {
        fileList.Clear();
        if (string.IsNullOrEmpty(folderPath))
        {
            folderPath = Path.Combine(Application.dataPath, "Memo");
        }
        if (Directory.Exists(folderPath))
        {
            string[] files = Directory.GetFiles(folderPath, "*_memo.txt");
            foreach (string file in files)
            {
                fileList.Add(Path.GetFileName(file));
            }
            Debug.Log($"ファイル読み込み完了");
        }
        else
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"フォルダを生成しました。({folderPath})");
        }
    }

    // ファイル読み込み
    void LoadFile()
    {
        if (string.IsNullOrEmpty(workfilePath))
        {
            workfilePath = Path.Combine(folderPath, fileName);
        }
        if (File.Exists(workfilePath))
        {
            fileContent = File.ReadAllText(workfilePath);
        }
        else
        {
            Debug.Log($"ファイルが存在しません。({workfilePath})");
        }
    }

    // ファイル情報設定
    void SetthingFileData()
    {
        folderPath = EditorGUILayout.TextField("フォルダパス", folderPath);
        if (GUILayout.Button("リスト更新"))
        {
            LoadTextFile();
        }
        if (fileList.Count > 0)
        {
            EditorGUILayout.BeginHorizontal();
            selectedIndex = EditorGUILayout.Popup("ファイル選択", selectedIndex, fileList.ToArray());
            if(b_selectedIndex != selectedIndex)
            {
                b_selectedIndex = selectedIndex;
                workfilePath = Path.Combine(folderPath, fileList[selectedIndex]);
                if (isOpenFile)
                {
                    LoadFile();
                }
            }
            if (GUILayout.Button("ファイルを開く"))
            {
                isOpenFile = true;
                LoadFile();
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.HelpBox("ファイルがありません", MessageType.Warning);
        }
    }

    // ファイル追加
    void NewFile()
    {
        EditorGUILayout.BeginHorizontal();
        fileName = EditorGUILayout.TextField("ファイル名", fileName);
        if (GUILayout.Button("新規追加"))
        {
            if (!fileName.EndsWith("_memo"))
            {
                fileName += "_memo";
            }
            if (!fileName.EndsWith(".txt"))
            {
                fileName += ".txt";
            }
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            workfilePath = Path.Combine(folderPath, fileName);
            StreamWriter sw = new StreamWriter(workfilePath, false, System.Text.Encoding.UTF8);
            sw.Flush();
            sw.Close();
            LoadTextFile();
            AssetDatabase.Refresh();
        }
        EditorGUILayout.EndHorizontal();
    }

    // テキスト表示
    void OpenFile()
    {
        if (isOpenFile && fileList.Count > 0)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField("表示中のファイル: ", Path.GetFileName(workfilePath));
            fileContent = EditorGUILayout.TextArea(fileContent, GUILayout.Height(180));
            if (GUILayout.Button("保存"))
            {
                File.WriteAllText(workfilePath, fileContent);
                Debug.Log($"ファイルを保存しました。({workfilePath})");
            }

            EditorGUILayout.EndVertical();
        }
    }
}