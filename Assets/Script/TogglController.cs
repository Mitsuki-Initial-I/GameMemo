using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TogglController : MonoBehaviour
{
    StoryInfo data;
    TextMeshProUGUI textMeshProUGUI;
    Toggle toggle;
    private GameMaster master;

    public void Setup(StoryInfo getData, GameMaster getmaster)
    {
        textMeshProUGUI = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        toggle = transform.Find("Toggle").GetComponent<Toggle>();

        data = getData;
        master = getmaster;
        textMeshProUGUI.text = data.storyName;
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }
    void OnToggleChanged(bool isOn)
    {
        data.storyCheck= isOn;
        master.DataUpdate(data);
    }
}