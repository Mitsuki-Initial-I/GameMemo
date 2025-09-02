using System.Collections.Generic;

[System.Serializable]
public class GameInfo
{
    public int gameId;
    public string gameName;
    public string gameNotes;
}

[System.Serializable]
public class CategoryInfo
{
    public int gameId;
    public long categoryId;
    public string categoryName;
    public string categoryNotes;
}

[System.Serializable]
public class StoryInfo
{
    public int gameId;
    public long categoryId;
    public int storyId;
    public string storyName;
    public string storyNotes;
    public bool storyCheck;
}

[System.Serializable]
public class AplSettings
{
    public string workPath;
}

[System.Serializable]
public struct GameDataList
{
    public List<GameInfo> gameIds;
}

[System.Serializable]
public struct CategoryDataList
{
    public List<CategoryInfo> categoryIds;
}

[System.Serializable]
public struct StoryDataList
{
    public List<StoryInfo> storyIds;
}