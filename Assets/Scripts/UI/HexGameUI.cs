using UnityEngine;

public class HexGameUI : MonoBehaviour
{
    public HexGrid Grid;

    public void SetEditMode(bool toggle)
    {
        enabled = !toggle;
        Grid.ShowUI(!toggle);
        Grid.ClearPath();

        if (toggle)
        {
            Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
        }
        else
        {
            Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
        }
    }
}