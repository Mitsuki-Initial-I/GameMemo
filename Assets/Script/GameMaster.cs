using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    [SerializeField]
    GameObject[] prefabs;
    [SerializeField]
    Transform mother;

    int mode = 0;
    GameDataList gameDataList = new GameDataList();
    CategoryDataList categoryDataList = new CategoryDataList();
    StoryDataList storyDataList = new StoryDataList();

    FileAccessControl fileAccess = new FileAccessControl();

    string folder;
    string[] fileNames = new string[3] { "gameData.csv", "categoryData.csv", "storyData.csv" };
    int nowGameId = 0;
    int nowCategoryId = 0;

    enum DataNames
    {
        game,
        category,
        story
    }
    void SaveCSV()
    {
        fileAccess.SaveFileSystem(folder, fileNames[0], gameDataList);
        fileAccess.SaveFileSystem(folder, fileNames[1], categoryDataList);
        fileAccess.SaveFileSystem(folder, fileNames[2], storyDataList);
    }
    public void DataUpdate(StoryInfo getData)
    {
        for (int i = 0; i < storyDataList.storyIds.Count; i++)
        {
            if(getData.gameId== storyDataList.storyIds[i].gameId&& getData.categoryId == storyDataList.storyIds[i].categoryId && getData.storyId == storyDataList.storyIds[i].storyId)
            {
                storyDataList.storyIds[i] = getData;
                break;
            }
        }
        SaveCSV();
    }
    private void Start()
    {
        folder = Path.Combine(Application.persistentDataPath, "Data");
        gameDataList.gameIds = new List<GameInfo>();
        categoryDataList.categoryIds = new List<CategoryInfo>();
        storyDataList.storyIds = new List<StoryInfo>();
        if (Directory.Exists(folder))
        {
            fileAccess.LoadFileSystem(folder, fileNames[0], out gameDataList);
            fileAccess.LoadFileSystem(folder, fileNames[1], out categoryDataList);
            fileAccess.LoadFileSystem(folder, fileNames[2], out storyDataList);
        }
        else
        {
            SaveCSV();
        }
        foreach (var item in gameDataList.gameIds)
        {
            var bo = Instantiate(prefabs[2], mother);
            var bc = bo.GetComponent<ButtonController>();
            bc.Setup(item.gameName, item.gameId, this);
        }
    }
    public void OpenData(int id)
    {
        // 配置中のオブエクトを削除
        foreach (Transform child in mother) Destroy(child);

        // ゲームタイトル表示中
        if (mode == 0)
        {
            mode = 1;           // カテゴリ表示中に切り替え
            nowGameId = id;     // ゲームID保存
            // ゲームタイトルに紐づくカテゴリを表示
            foreach (var item in categoryDataList.categoryIds)
            {
                if (item.gameId == id)
                {
                    var bo = Instantiate(prefabs[1], mother);
                    var bc = bo.GetComponent<ButtonController>();
                    bc.Setup(item.categoryName, item.categoryId, this);
                }
            }
        }

        // カテゴリ表示中
        else if (mode == 1)
        {
            nowCategoryId = id;                         // カテゴリId保存
            string currentIdStr = id.ToString();        // カテゴリIDを文字列へ変換し取得
            foreach (var item in categoryDataList.categoryIds)
            {
                string idstr = item.categoryId.ToString();
                if (idstr.StartsWith(currentIdStr) && item.categoryId != id)
                {
                    string sub = idstr.Substring(currentIdStr.Length);
                    if (!string.IsNullOrEmpty(sub) && !sub.Contains("0") && sub.Length <= 4)
                    {
                        var bo = Instantiate(prefabs[1], mother);
                        var bc = bo.GetComponent<ButtonController>();
                        bc.Setup(item.categoryName, item.categoryId, this);
                    }
                }
            }
            // 配置しているものがない(カテゴリからカテゴリにいかない)場合
            if (mother.childCount <= 0)
            {
                mode = 2;           // チェックリスト表示
                foreach (var item in storyDataList.storyIds)
                {
                    if (item.gameId == nowGameId && item.categoryId == id)
                    {
                        var to = Instantiate(prefabs[0], mother);
                        var tc = to.GetComponent<TogglController>();
                    }
                }
            }
        }
    }
}