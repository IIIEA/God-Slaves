﻿using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

public class HexMapEditor : MonoBehaviour
{
    private enum OptionalToggle
    {
        Ignore, Yes, No
    }

    public HexGrid HexGrid;
    public Material TerrainMaterial;

    private int _activeElevation;
    private int _activeWaterLevel;
    private int _activeUrbanLevel, _activeFarmLevel, _activePlantLevel, _activeSpecialIndex;
    private int _activeTerrainTypeIndex = -1;
    private int _brushSize;
    private bool _applyElevation = false;
    private bool _applyWaterLevel = false;
    private bool _applyUrbanLevel, _applyFarmLevel, _applyPlantLevel, _applySpecialIndex;

    private OptionalToggle _riverMode, _roadMode, _walledMode;
    private bool _isDrag;
    private HexDirection _dragDirection;
    private HexCell _previousCell;

    void Awake()
    {
        TerrainMaterial.DisableKeyword("GRID_ON");
        Shader.EnableKeyword("HEX_MAP_EDIT_MODE");
        SetEditMode(true);
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButton(0))
            {
                HandleInput();
                return;
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    DestroyUnit();
                }
                else
                {
                    CreateUnit();
                }
                return;
            }
        }

        _previousCell = null;
    }

    public void SetTerrainTypeIndex(int index)
    {
        _activeTerrainTypeIndex = index;
    }

    public void SetApplyElevation(bool toggle)
    {
        _applyElevation = toggle;
    }

    public void SetElevation(float elevation)
    {
        _activeElevation = (int)elevation;
    }

    public void SetApplyWaterLevel(bool toggle)
    {
        _applyWaterLevel = toggle;
    }

    public void SetWaterLevel(float level)
    {
        _activeWaterLevel = (int)level;
    }

    public void SetApplyUrbanLevel(bool toggle)
    {
        _applyUrbanLevel = toggle;
    }

    public void SetUrbanLevel(float level)
    {
        _activeUrbanLevel = (int)level;
    }

    public void SetApplyFarmLevel(bool toggle)
    {
        _applyFarmLevel = toggle;
    }

    public void SetFarmLevel(float level)
    {
        _activeFarmLevel = (int)level;
    }

    public void SetApplyPlantLevel(bool toggle)
    {
        _applyPlantLevel = toggle;
    }

    public void SetPlantLevel(float level)
    {
        _activePlantLevel = (int)level;
    }

    public void SetApplySpecialIndex(bool toggle)
    {
        _applySpecialIndex = toggle;
    }

    public void SetSpecialIndex(float index)
    {
        _activeSpecialIndex = (int)index;
    }

    public void SetBrushSize(float size)
    {
        _brushSize = (int)size;
    }

    public void SetRiverMode(int mode)
    {
        _riverMode = (OptionalToggle)mode;
    }

    public void SetRoadMode(int mode)
    {
        _roadMode = (OptionalToggle)mode;
    }

    public void SetWalledMode(int mode)
    {
        _walledMode = (OptionalToggle)mode;
    }

    public void SetEditMode(bool toggle)
    {
        enabled = toggle;
    }

    public void ShowGrid(bool visible)
    {
        if (visible)
        {
            TerrainMaterial.EnableKeyword("GRID_ON");
        }
        else
        {
            TerrainMaterial.DisableKeyword("GRID_ON");
        }
    }

    private HexCell GetCellUnderCursor()
    {
        return
            HexGrid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
    }

    private void CreateUnit()
    {
        HexCell cell = GetCellUnderCursor();

        if (cell && !cell.Unit)
        {
            HexGrid.AddUnit(Instantiate(HexUnit.unitPrefab), cell, Random.Range(0f, 360f));
        }
    }

    private void DestroyUnit()
    {
        HexCell cell = GetCellUnderCursor();

        if (cell && cell.Unit)
        {
            HexGrid.RemoveUnit(cell.Unit);
        }
    }

    private void HandleInput()
    {
        HexCell currentCell = GetCellUnderCursor();

        if (currentCell)
        {
            if (_previousCell && _previousCell != currentCell)
            {
                ValidateDrag(currentCell);
            }
            else
            {
                _isDrag = false;
            }

            EditCells(currentCell);
            _previousCell = currentCell;
        }
        else
        {
            _previousCell = null;
        }
    }

    private void ValidateDrag(HexCell currentCell)
    {
        for (_dragDirection = HexDirection.NE; _dragDirection <= HexDirection.NW; _dragDirection++) 
        {
            if (_previousCell.GetNeighbor(_dragDirection) == currentCell)
            {
                _isDrag = true;
                return;
            }
        }

        _isDrag = false;
    }

    private void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - _brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + _brushSize; x++)
            {
                EditCell(HexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
        for (int r = 0, z = centerZ + _brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - _brushSize; x <= centerX + r; x++)
            {
                EditCell(HexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }

    private void EditCell(HexCell cell)
    {
        if (cell)
        {
            if (_activeTerrainTypeIndex >= 0)
            {
                cell.TerrainTypeIndex = _activeTerrainTypeIndex;
            }
            if (_applyElevation)
            {
                cell.Elevation = _activeElevation;
            }
            if (_applyWaterLevel)
            {
                cell.WaterLevel = _activeWaterLevel;
            }
            if (_applySpecialIndex)
            {
                cell.SpecialIndex = _activeSpecialIndex;
            }
            if (_applyUrbanLevel)
            {
                cell.UrbanLevel = _activeUrbanLevel;
            }
            if (_applyFarmLevel)
            {
                cell.FarmLevel = _activeFarmLevel;
            }
            if (_applyPlantLevel)
            {
                cell.PlantLevel = _activePlantLevel;
            }
            if (_riverMode == OptionalToggle.No)
            {
                cell.RemoveRiver();
            }
            if (_roadMode == OptionalToggle.No)
            {
                cell.RemoveRoads();
            }
            if (_walledMode != OptionalToggle.Ignore)
            {
                cell.Walled = _walledMode == OptionalToggle.Yes;
            }
            if (_isDrag)
            {
                HexCell otherCell = cell.GetNeighbor(_dragDirection.Opposite());
                if (otherCell)
                {
                    if (_riverMode == OptionalToggle.Yes)
                    {
                        otherCell.SetOutgoingRiver(_dragDirection);
                    }
                    if (_roadMode == OptionalToggle.Yes)
                    {
                        otherCell.AddRoad(_dragDirection);
                    }
                }
            }
        }
    }
}