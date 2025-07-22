using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    TextMeshProUGUI textMeshProUGUI;
    private int id;
    private GameMaster master;

    public void Setup(string gettext,int getid,GameMaster getmaster)
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        textMeshProUGUI = transform.Find("Text").GetComponent<TextMeshProUGUI>();

        textMeshProUGUI.text = gettext;
        id = getid;
        master = getmaster;
    }
    public void OnClick()
    {
        master.OpenData(id);
    }
}
