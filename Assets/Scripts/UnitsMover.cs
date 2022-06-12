using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitsMover : MonoBehaviour
{
    public HexGrid Grid;
    private List<HexUnit> _units = new List<HexUnit>();

    private void OnEnable()
    {
        Grid.UnitAdded += OnUnitAdded;

        foreach (var unit in _units)
        {
            unit.Treveled += OnTreveled;
        }
    }

    private void OnDisable()
    {
        Grid.UnitAdded -= OnUnitAdded;

        foreach (var unit in _units)
        {
            unit.Treveled -= OnTreveled;
        }
    }

    private void OnTreveled(HexUnit unit, HexCell cell)
    {
        if(unit.InTravel == false)
        {
            DoPathfinding(unit, cell);
            DoMove(unit);
        }
    }

    private void OnUnitAdded(HexUnit unit)
    {
        unit.Treveled += OnTreveled;
        _units.Add(unit);
    }

    private void DoPathfinding(HexUnit unit, HexCell cellToTravel)
    {
        if (cellToTravel && unit.IsValidDestination(cellToTravel))
        {
            Grid.FindPath(unit.Location, cellToTravel, unit);
        }
        else
        {
            Grid.ClearPath();
        }
    }

    private void DoMove(HexUnit unit)
    {
        if (Grid.HasPath)
        {
            unit.Travel(Grid.GetPath());
            Grid.ClearPath();
        }
    }
}