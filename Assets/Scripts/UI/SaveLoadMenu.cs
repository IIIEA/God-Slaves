using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class SaveLoadMenu : MonoBehaviour
{
    public Text MenuLabel, ActionButtonLabel;
    public InputField NameInput;
    public RectTransform ListContent;
    public SaveLoadItem ItemPrefab;
    public HexGrid HexGrid;

    private bool _saveMode;

    private const int MapFileVersion = 5;

    public void Open(bool saveMode)
    {
        _saveMode = saveMode;

        if (saveMode)
        {
            MenuLabel.text = "Save Map";
            ActionButtonLabel.text = "Save";
        }
        else
        {
            MenuLabel.text = "Load Map";
            ActionButtonLabel.text = "Load";
        }

        FillList();
        gameObject.SetActive(true);
        HexMapCamera.Locked = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        HexMapCamera.Locked = false;
    }

    public void Action()
    {
        string path = GetSelectedPath();

        if (path == null)
        {
            return;
        }

        if (_saveMode)
        {
            Save(path);
        }
        else
        {
            Load(path);
        }

        Close();
    }

    public void SelectItem(string name)
    {
        NameInput.text = name;
    }

    public void Delete()
    {
        string path = GetSelectedPath();

        if (path == null)
        {
            return;
        }
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        NameInput.text = "";
        FillList();
    }

    private void FillList()
    {
        for (int i = 0; i < ListContent.childCount; i++)
        {
            Destroy(ListContent.GetChild(i).gameObject);
        }

        string[] paths =Directory.GetFiles(Application.persistentDataPath, "*.map");
        Array.Sort(paths);

        for (int i = 0; i < paths.Length; i++)
        {
            SaveLoadItem item = Instantiate(ItemPrefab);
            item.Menu = this;
            item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
            item.transform.SetParent(ListContent, false);
        }
    }

    private string GetSelectedPath()
    {
        string mapName = NameInput.text;

        if (mapName.Length == 0)
        {
            return null;
        }

        return Path.Combine(Application.persistentDataPath, mapName + ".map");
    }

    private void Save(string path)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(MapFileVersion);
            HexGrid.Save(writer);
        }
    }

    private void Load(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("File does not exist " + path);
            return;
        }

        using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
        {
            int header = reader.ReadInt32();
            if (header <= MapFileVersion)
            {
                HexGrid.Load(reader, header);
                HexMapCamera.ValidatePosition();
            }
            else
            {
                Debug.LogWarning("Unknown map format " + header);
            }
        }
    }
}