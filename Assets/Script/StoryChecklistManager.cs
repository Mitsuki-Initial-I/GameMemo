using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryChecklistManager : MonoBehaviour
{
    public Transform contentParent;
    public GameObject rowPrefab;
    private string csvPath;
    private List<StoryData> storyDatas = new();

    void SaveToCSV()
    {
        List<string> lines = new() { "Game,Category,StoryTitle,Checked" };
        lines.AddRange(storyDatas.Select(i =>
            $"{i.GameName},{i.Category},{i.StoryTitle},{i.Checked.ToString().ToLower()}"));
        File.WriteAllLines(csvPath, lines);
    }

    void LoadFromCSV()
    {
        storyDatas.Clear();
        foreach(string line in File.ReadAllLines(csvPath).Skip(1))
        {
            string[] parts = line.Split(',');
            if (parts.Length < 4) continue;
            storyDatas.Add(new StoryData
            {
                GameName = parts[0],
                Category = parts[1],
                StoryTitle = parts[2],
                Checked = parts[3] == "true"
            });
        }
    }

    void BuildUI()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);
        foreach(var item in storyDatas)
        {
            GameObject row = Instantiate(rowPrefab, contentParent);
            row.transform.Find("GameText").GetComponent<Text>().text = item.GameName;
            row.transform.Find("CategoryText").GetComponent<Text>().text = item.Category;
            row.transform.Find("TitleText").GetComponent<Text>().text = item.StoryTitle;
            Toggle toggle = row.transform.Find("Toggle").GetComponent<Toggle>();
            toggle.isOn = item.Checked;

            toggle.onValueChanged.AddListener((val) =>
            {
                item.Checked = val;
                SaveToCSV();
            });
        }
    }

    void RefreshUI()
    {
        LoadFromCSV();
        BuildUI();
    }

    private void Start()
    {
        csvPath = Path.Combine(Application.persistentDataPath, "StoryChecklist.csv");
        if (File.Exists(csvPath))
            LoadFromCSV();
        BuildUI() ;
    }

    public void AddGame()
    {
        storyDatas.Add(new StoryData { GameName = "NewGame", Category = "Main", StoryTitle = "1˜b", Checked = false });
        SaveToCSV();
        RefreshUI();
    }
}