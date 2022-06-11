using UnityEngine;
using UnityEngine.EventSystems;

public class HexGameUI : MonoBehaviour
{
    public HexGrid Grid;

    private HexCell _currentCell;
    private HexUnit _selectedUnit;

    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                DoSelection();
            }
            else if (_selectedUnit)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    DoMove();
                }
                else
                {
                    DoPathfinding();
                }
            }
        }
    }

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

    private void DoSelection()
    {
        Grid.ClearPath();
        UpdateCurrentCell();

        if (_currentCell)
        {
            _selectedUnit = _currentCell.Unit;
        }
    }

    private void DoPathfinding()
    {
        if (UpdateCurrentCell())
        {
            if (_currentCell && _selectedUnit.IsValidDestination(_currentCell))
            {
                Grid.FindPath(_selectedUnit.Location, _currentCell, _selectedUnit);
            }
            else
            {
                Grid.ClearPath();
            }
        }
    }

    private void DoPathfinding(HexUnit unit, HexCell cellToTravel)
    {
        if(cellToTravel && unit.IsValidDestination(cellToTravel))
        {
            Grid.FindPath(unit.Location, cellToTravel, unit);
        }
        else
        {
            Grid.ClearPath();
        }
    }

    private void DoMove()
    {
        if (Grid.HasPath)
        {
            _selectedUnit.Travel(Grid.GetPath());
            Grid.ClearPath();
        }
    }

    private bool UpdateCurrentCell()
    {
        HexCell cell = Grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));

        if (cell != _currentCell)
        {
            _currentCell = cell;
            return true;
        }

        return false;
    }
}