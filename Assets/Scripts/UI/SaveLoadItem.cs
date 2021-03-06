using UnityEngine;
using UnityEngine.UI;

public class SaveLoadItem : MonoBehaviour
{
    public SaveLoadMenu Menu;

    private string _mapName;

    public string MapName
    {
        get
        {
            return _mapName;
        }
        set
        {
            _mapName = value;
            transform.GetChild(0).GetComponent<Text>().text = value;
        }
    }

    public void Select()
    {
        Menu.SelectItem(_mapName);
    }
}