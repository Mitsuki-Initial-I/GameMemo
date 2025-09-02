using TMPro;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class HistoryButtonController : MonoBehaviour
{
    TextMeshProUGUI textMeshProUGUI;
    GameMaster master;
    long myData;

    public void Setup(int count, GameMaster getmaster,long getid)
    {
        master= getmaster;
        GetComponent<Button>().onClick.AddListener(HistoryButton_OnClick);
        var countText = count.ToString();
        textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();
        textMeshProUGUI.text = countText;

        myData = getid;
    }
    public void HistoryButton_OnClick()
    {
        master.HistoryData(myData);
    }
}
