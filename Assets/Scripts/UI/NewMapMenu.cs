using UnityEngine;

public class NewMapMenu : MonoBehaviour
{
    public HexGrid HexGrid;
    public HexMapGenerator MapGenerator;

    private bool _generateMaps = true;
    private bool _wrapping = true;

    public void ToggleMapGeneration(bool toggle)
    {
        _generateMaps = toggle;
    }

    public void ToggleWrapping(bool toggle)
    {
        _wrapping = toggle;
    }

    public void Open()
    {
        gameObject.SetActive(true);
        HexMapCamera.Locked = true;
    }

    public void Close()
    {
        gameObject.SetActive(false);
        HexMapCamera.Locked = false;
    }

    public void CreateSmallMap()
    {
        CreateMap(20, 15);
    }

    public void CreateMediumMap()
    {
        CreateMap(40, 30);
    }

    public void CreateLargeMap()
    {
        CreateMap(80, 60);
    }

    private void CreateMap(int x, int z)
    {
        if (_generateMaps)
        {
            MapGenerator.GenerateMap(x, z, _wrapping);
        }
        else
        {
            HexGrid.CreateMap(x, z, _wrapping);
        }

        HexMapCamera.ValidatePosition();
        Close();
    }
}