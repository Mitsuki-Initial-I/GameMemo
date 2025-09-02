using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour
{
    [SerializeField]
    GameObject[] prefabs;
    [SerializeField]
    Transform[] mothers;
    [SerializeField]
    GameObject addPanel;

    int mode = 0;
    GameDataList gameDataList = new GameDataList();
    CategoryDataList categoryDataList = new CategoryDataList();
    StoryDataList storyDataList = new StoryDataList();

    FileAccessControl fileAccess = new FileAccessControl();

    string folder;
    string[] fileNames = new string[3] { "\\gameData.csv", "\\categoryData.csv", "\\storyData.csv" };
    int nowGameId = 0;
    long nowCategoryId = 0;

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
        Debug.Log(folder);
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
            var bo = Instantiate(prefabs[1], mothers[0]);
            var bc = bo.GetComponent<ButtonController>();
            bc.Setup(item.gameName, item.gameId, this);
        }
        var addButton = Instantiate(prefabs[2], mothers[0]);
        addButton.GetComponent<Button>().onClick.AddListener(AddDate);
        addButton.GetComponentInChildren<TextMeshProUGUI>().text="追加";
    }
    public void AddDate()
    {
        addPanel.SetActive(true);
        var addPanelText = addPanel.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        switch ((DataNames)mode)
        {
            case DataNames.game:
                addPanelText.text = "ゲームタイトル入力";
                break;
            case DataNames.category:
                addPanelText.text = "カテゴリ入力";
                break;
            case DataNames.story:
                addPanelText.text = "チェックリスト入力";
                break;
            default:
                addPanelText.text = "無効な入力";
                break;
        }
    }
    public void CloseAddPanel()
    {
        addPanel.SetActive(false);
    }
    public void AddDataPanel()
    {
        var name = addPanel.transform.Find("InputField").transform.Find("Text Area").transform.Find("Text").GetComponent<TextMeshProUGUI>().text;
        var notes = addPanel.transform.Find("InputField (1)").transform.Find("Text Area").transform.Find("Text").GetComponent<TextMeshProUGUI>().text;
        bool checkListFlg  = addPanel.transform.Find("Toggle").transform.GetComponent<Toggle>().isOn;
        foreach (Transform child in mothers[0]) Destroy(child.gameObject);

        if(checkListFlg)
        {
            var storyInfo = new StoryInfo();
            storyInfo.gameId = nowGameId;
            storyInfo.categoryId = nowCategoryId;
            storyInfo.storyId = storyDataList.storyIds.Count + 1;
            storyInfo.storyName = name;
            storyInfo.storyNotes = notes;
            storyInfo.storyCheck = false;
            storyDataList.storyIds.Add(storyInfo);
            OpenData(nowCategoryId);
        }
        else if(mode==(int)DataNames.game)
        {
            var gameInfo = new GameInfo();
            gameInfo.gameId = gameDataList.gameIds.Count + 1;
            gameInfo.gameName = name;
            gameInfo.gameNotes = notes;
            gameDataList.gameIds.Add(gameInfo);
            foreach (var item in gameDataList.gameIds)
            {
                var bo = Instantiate(prefabs[1], mothers[0]);
                var bc = bo.GetComponent<ButtonController>();
                bc.Setup(item.gameName, item.gameId, this);
            }
            var addButton = Instantiate(prefabs[2], mothers[0]);
            addButton.GetComponent<Button>().onClick.AddListener(AddDate);
            addButton.GetComponentInChildren<TextMeshProUGUI>().text = "追加";
        }
        else
        {
            var categoryInfo = new CategoryInfo();
            string combined = nowCategoryId.ToString() + (categoryDataList.categoryIds.Count + 1).ToString("D3");
            Debug.Log(combined);
            categoryInfo.gameId = nowGameId;
            categoryInfo.categoryId = long.Parse(combined);
            categoryInfo.categoryName = name;
            categoryInfo.categoryNotes = notes;
            categoryDataList.categoryIds.Add(categoryInfo);
            OpenData(nowCategoryId);
        }
        SaveCSV();
        addPanel.SetActive(false);
    }
    private void SetGameButton()
    {
        mode = 0;
        nowGameId = 0;
        nowCategoryId = 0;
        foreach (Transform child in mothers[0]) Destroy(child.gameObject);

        foreach (var item in gameDataList.gameIds)
        {
            var bo = Instantiate(prefabs[1], mothers[0]);
            var bc = bo.GetComponent<ButtonController>();
            bc.Setup(item.gameName, item.gameId, this);
        }
        var addButton = Instantiate(prefabs[2], mothers[0]);
        addButton.GetComponent<Button>().onClick.AddListener(AddDate);
        addButton.GetComponentInChildren<TextMeshProUGUI>().text = "追加";
    }
    public void OpenData(long id,bool result=true)
    {
        Debug.Log($"{mode}:{nowGameId}:{nowCategoryId}:{id}" );

        // 配置中のオブエクトを削除
        foreach (Transform child in mothers[0]) Destroy(child.gameObject);

        // ゲームタイトル表示中
        if (mode == 0)
        {
            mode = 1;               // カテゴリ表示中に切り替え
            nowGameId = (int)id;    // ゲームID保存
            nowCategoryId = id;     // カテゴリIDを文字列へ変換し取得
            // ゲームタイトルに紐づくカテゴリを表示
            foreach (var item in categoryDataList.categoryIds)
            {
                if (item.gameId == id)
                {
                    var bo = Instantiate(prefabs[1], mothers[0]);
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
                        var bo = Instantiate(prefabs[1], mothers[0]);
                        var bc = bo.GetComponent<ButtonController>();
                        bc.Setup(item.categoryName, item.categoryId, this);
                    }
                }
            }

            Debug.Log(mothers[0].childCount);
            // 配置しているものがない(カテゴリからカテゴリにいかない)場合
            if (mothers[0].childCount <= 0)
            {
                mode = 2;           // チェックリスト表示
                foreach (var item in storyDataList.storyIds)
                {
                    if (item.gameId == nowGameId && item.categoryId == id)
                    {
                        var to = Instantiate(prefabs[0], mothers[0]);
                        var tc = to.GetComponent<TogglController>();
                    }
                }
            }
        }
        Debug.Log($"後{mode}:{nowGameId}:{nowCategoryId}:{id}");

        var addButton = Instantiate(prefabs[2], mothers[0]);
        addButton.GetComponent<Button>().onClick.AddListener(AddDate);
        addButton.GetComponentInChildren<TextMeshProUGUI>().text = "追加";
        if(result)
            HistoryButton(nowCategoryId, mothers[1].childCount);
    }

    //public void HistoryData(long myId)
    //{
    //    Debug.Log(myId);
    //    foreach (Transform child in mothers[1]) Destroy(child.gameObject);
    //    List<long> result = new List<long>();
    //    while(myId>0)
    //    {
    //        result.Add(myId);
    //        myId /= 1000;
    //    }
    //    result.Add(0);
    //    result.Reverse();
    //    var count = 0;
    //    foreach(var id in result)
    //    {
    //        HistoryButton(id, count);
    //        Debug.Log(id);
    //        count++;
    //    }
    //    OpenData(result[result.Count-2],false);
    //}

    public void HistoryData(long myId)
    {
        Debug.Log($"履歴ID: {myId}");

        // 表示UI初期化（全UIクリア）
        foreach (Transform child in mothers[1]) Destroy(child.gameObject);
        if (myId/1000 >= 1)
        {
            // 親階層を再帰的に遡る（例: 1001001 → 1001 → 1 → 0）
            List<long> result = new List<long>();
            while (myId > 0)
            {
                result.Add(myId);
                myId /= 1000;
            }
            result.Add(0);
            result.Reverse();

            // 履歴ボタンを順に生成
            for (int i = 0; i < result.Count; i++)
            {
                long id = result[i];
                HistoryButton(id, i);
                Debug.Log($"履歴ボタン: {id}");
            }

            // 一つ上の階層を表示（例：1001001 → 1001 の内容を表示）
            if (result.Count >= 2)
            {
                OpenData(result[result.Count - 2], false);
            }
            else
            {
                Debug.LogWarning("履歴が1階層しかないため、OpenDataは呼び出しません");
            }
        }
        else
        {
            SetGameButton();
        }
    }

    void HistoryButton(long setid,int count)
    {
        var hb= Instantiate(prefabs[3], mothers[1]);
        var hbc=hb.GetComponent<HistoryButtonController>();
        hbc.Setup(count,this, setid);
    }
}