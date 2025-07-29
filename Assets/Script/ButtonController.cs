using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    TextMeshProUGUI textMeshProUGUI;
    private long id;
    private GameMaster master;

    public void Setup(string gettext,long getid,GameMaster getmaster)
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();

        textMeshProUGUI.text = gettext;
        id = getid;
        master = getmaster;
    }
    public void OnClick()
    {
        master.OpenData(id);
    }
}
