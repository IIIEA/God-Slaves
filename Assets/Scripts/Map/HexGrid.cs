using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;

public class HexGrid : MonoBehaviour
{
    public int cellCountX = 20, cellCountZ = 15;
    public bool wrapping;
    public HexCell cellPrefab;
    public Text cellLabelPrefab;
    public HexGridChunk chunkPrefab;
    public HexUnit unitPrefab;
    public Texture2D noiseSource;
    public int seed;


    private Transform[] _columns;
    private HexGridChunk[] _chunks;
    private HexCell[] _cells;
    private int _chunkCountX, _chunkCountZ;
    private HexCellPriorityQueue _searchFrontier;
    private int _searchFrontierPhase;
    private HexCell _currentPathFrom, _currentPathTo;
    private bool _currentPathExists;
    private int _currentCenterColumnIndex = -1;
    private List<HexUnit> _units = new List<HexUnit>();
    private HexCellShaderData _cellShaderData;

    public bool HasPath => _currentPathExists;
    public List<HexUnit> Units => _units;

    public event Action<HexUnit> UnitAdded;
    public event Action<int> UnitCountChanged;

    private void Awake()
    {
        HexMetrics.NoiseSource = noiseSource;
        HexMetrics.InitializeHashGrid(seed);
        HexUnit.unitPrefab = unitPrefab;
        _cellShaderData = gameObject.AddComponent<HexCellShaderData>();
        _cellShaderData.Grid = this;
        CreateMap(cellCountX, cellCountZ, wrapping);
    }

    private void Start()
    {
        UnitCountChanged?.Invoke(_units.Count);
    }

    private void OnEnable()
    {
        if (!HexMetrics.NoiseSource)
        {
            HexMetrics.NoiseSource = noiseSource;
            HexMetrics.InitializeHashGrid(seed);
            HexUnit.unitPrefab = unitPrefab;
            HexMetrics.wrapSize = wrapping ? cellCountX : 0;
            ResetVisibility();
        }
    }

    private void OnDisable()
    {
        foreach (var unit in _units)
        {
            unit.Died -= OnUnitDied;
        }
    }

    private void OnUnitDied(HexUnit unit)
    {
        _units.Remove(unit);
        UnitCountChanged?.Invoke(_units.Count);
    }

    public void AddUnit(HexUnit unit, HexCell location, float orientation)
    {
        unit.Died += OnUnitDied;
        UnitAdded?.Invoke(unit);
        _units.Add(unit);
        UnitCountChanged?.Invoke(_units.Count);
        unit.Grid = this;
        unit.Location = location;
        unit.Orientation = orientation;
    }

    public void RemoveUnit(HexUnit unit)
    {
        _units.Remove(unit);
        UnitCountChanged?.Invoke(_units.Count);
        unit.Die();
    }

    public void MakeChildOfColumn(Transform child, int columnIndex)
    {
        child.SetParent(_columns[columnIndex], false);
    }

    public bool CreateMap(int x, int z, bool wrapping)
    {
        if (x <= 0 || x % HexMetrics.ChunkSizeX != 0 || z <= 0 || z % HexMetrics.chunkSizeZ != 0)
        {
            Debug.LogError("Unsupported map size.");
            return false;
        }

        ClearPath();
        ClearUnits();

        if (_columns != null)
        {
            for (int i = 0; i < _columns.Length; i++)
            {
                Destroy(_columns[i].gameObject);
            }
        }

        cellCountX = x;
        cellCountZ = z;
        this.wrapping = wrapping;
        _currentCenterColumnIndex = -1;
        HexMetrics.wrapSize = wrapping ? cellCountX : 0;
        _chunkCountX = cellCountX / HexMetrics.ChunkSizeX;
        _chunkCountZ = cellCountZ / HexMetrics.chunkSizeZ;
        _cellShaderData.Initialize(cellCountX, cellCountZ);

        CreateChunks();
        CreateCells();

        return true;
    }

    private void CreateChunks()
    {
        _columns = new Transform[_chunkCountX];

        for (int x = 0; x < _chunkCountX; x++)
        {
            _columns[x] = new GameObject("Column").transform;
            _columns[x].SetParent(transform, false);
        }

        _chunks = new HexGridChunk[_chunkCountX * _chunkCountZ];

        for (int z = 0, i = 0; z < _chunkCountZ; z++)
        {
            for (int x = 0; x < _chunkCountX; x++)
            {
                HexGridChunk chunk = _chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(_columns[x], false);
            }
        }
    }

    private void CreateCells()
    {
        _cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    private void ClearUnits()
    {
        for (int i = 0; i < _units.Count; i++)
        {
            _units[i].Die();
        }

        _units.Clear();
    }

    public HexCell GetCell(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return GetCell(hit.point);
        }

        return null;
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        return GetCell(coordinates);
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int z = coordinates.Z;

        if (z < 0 || z >= cellCountZ)
        {
            return null;
        }

        int x = coordinates.X + z / 2;

        if (x < 0 || x >= cellCountX)
        {
            return null;
        }

        return _cells[x + z * cellCountX];
    }

    public HexCell GetCell(int xOffset, int zOffset)
    {
        return _cells[xOffset + zOffset * cellCountX];
    }

    public HexCell GetCell(int cellIndex)
    {
        return _cells[cellIndex];
    }

    public void ShowUI(bool visible)
    {
        for (int i = 0; i < _chunks.Length; i++)
        {
            _chunks[i].ShowUI(visible);
        }
    }

    private void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * HexMetrics.InnerDiameter;
        position.y = 0f;
        position.z = z * (HexMetrics.OuterRadius * 1.5f);

        HexCell cell = _cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.Index = i;
        cell.ColumnIndex = x / HexMetrics.ChunkSizeX;
        cell.ShaderData = _cellShaderData;

        if (wrapping)
        {
            cell.Explorable = z > 0 && z < cellCountZ - 1;
        }
        else
        {
            cell.Explorable =
                x > 0 && z > 0 && x < cellCountX - 1 && z < cellCountZ - 1;
        }

        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, _cells[i - 1]);
            if (wrapping && x == cellCountX - 1)
            {
                cell.SetNeighbor(HexDirection.E, _cells[i - x]);
            }
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, _cells[i - cellCountX]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, _cells[i - cellCountX - 1]);
                }
                else if (wrapping)
                {
                    cell.SetNeighbor(HexDirection.SW, _cells[i - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, _cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, _cells[i - cellCountX + 1]);
                }
                else if (wrapping)
                {
                    cell.SetNeighbor(
                        HexDirection.SE, _cells[i - cellCountX * 2 + 1]
                    );
                }
            }
        }

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        cell.uiRect = label.rectTransform;

        cell.Elevation = 0;

        AddCellToChunk(x, z, cell);
    }

    private void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.ChunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = _chunks[chunkX + chunkZ * _chunkCountX];

        int localX = x - chunkX * HexMetrics.ChunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.ChunkSizeX, cell);
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(cellCountX);
        writer.Write(cellCountZ);
        writer.Write(wrapping);

        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i].Save(writer);
        }

        writer.Write(_units.Count);
        for (int i = 0; i < _units.Count; i++)
        {
            _units[i].Save(writer);
        }
    }

    public void Load(BinaryReader reader, int header)
    {
        ClearPath();
        ClearUnits();
        int x = 20, z = 15;

        if (header >= 1)
        {
            x = reader.ReadInt32();
            z = reader.ReadInt32();
        }

        bool wrapping = header >= 5 ? reader.ReadBoolean() : false;

        if (x != cellCountX || z != cellCountZ || this.wrapping != wrapping)
        {
            if (!CreateMap(x, z, wrapping))
            {
                return;
            }
        }

        bool originalImmediateMode = _cellShaderData.ImmediateMode;
        _cellShaderData.ImmediateMode = true;

        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i].Load(reader, header);
        }
        for (int i = 0; i < _chunks.Length; i++)
        {
            _chunks[i].Refresh();
        }

        if (header >= 2)
        {
            int unitCount = reader.ReadInt32();
            for (int i = 0; i < unitCount; i++)
            {
                HexUnit.Load(reader, this);
            }
        }

        _cellShaderData.ImmediateMode = originalImmediateMode;
    }

    public List<HexCell> GetPath()
    {
        if (!_currentPathExists)
        {
            return null;
        }

        List<HexCell> path = ListPool<HexCell>.Get();

        for (HexCell c = _currentPathTo; c != _currentPathFrom; c = c.PathFrom)
        {
            path.Add(c);
        }

        path.Add(_currentPathFrom);
        path.Reverse();
        return path;
    }

    public void ClearPath()
    {
        if (_currentPathExists)
        {
            HexCell current = _currentPathTo;
            while (current != _currentPathFrom)
            {
                current.SetLabel(null);
                current.DisableHighlight();
                current = current.PathFrom;
            }
            current.DisableHighlight();
            _currentPathExists = false;
        }
        else if (_currentPathFrom)
        {
            _currentPathFrom.DisableHighlight();
            _currentPathTo.DisableHighlight();
        }

        _currentPathFrom = _currentPathTo = null;
    }

    private void ShowPath(int speed)
    {
        if (_currentPathExists)
        {
            HexCell current = _currentPathTo;
            while (current != _currentPathFrom)
            {
                int turn = (current.Distance - 1) / speed;
                current.SetLabel(turn.ToString());
                current.EnableHighlight(Color.white);
                current = current.PathFrom;
            }
        }

        _currentPathFrom.EnableHighlight(Color.blue);
        _currentPathTo.EnableHighlight(Color.red);
    }

    public void FindPath(HexCell fromCell, HexCell toCell, HexUnit unit)
    {
        ClearPath();
        _currentPathFrom = fromCell;
        _currentPathTo = toCell;
        _currentPathExists = Search(fromCell, toCell, unit);
        ShowPath(unit.Speed);
    }

    private bool Search(HexCell fromCell, HexCell toCell, HexUnit unit)
    {
        int speed = unit.Speed;
        _searchFrontierPhase += 2;

        if (_searchFrontier == null)
        {
            _searchFrontier = new HexCellPriorityQueue();
        }
        else
        {
            _searchFrontier.Clear();
        }

        fromCell.SearchPhase = _searchFrontierPhase;
        fromCell.Distance = 0;
        _searchFrontier.Enqueue(fromCell);

        while (_searchFrontier.Count > 0)
        {
            HexCell current = _searchFrontier.Dequeue();
            current.SearchPhase += 1;

            if (current == toCell)
            {
                return true;
            }

            int currentTurn = (current.Distance - 1) / speed;

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);

                if (
                    neighbor == null ||
                    neighbor.SearchPhase > _searchFrontierPhase
                )
                {
                    continue;
                }
                if (!unit.IsValidDestination(neighbor))
                {
                    continue;
                }

                int moveCost = unit.GetMoveCost(current, neighbor, d);

                if (moveCost < 0)
                {
                    continue;
                }

                int distance = current.Distance + moveCost;
                int turn = (distance - 1) / speed;

                if (turn > currentTurn)
                {
                    distance = turn * speed + moveCost;
                }

                if (neighbor.SearchPhase < _searchFrontierPhase)
                {
                    neighbor.SearchPhase = _searchFrontierPhase;
                    neighbor.Distance = distance;
                    neighbor.PathFrom = current;
                    neighbor.SearchHeuristic = neighbor.coordinates.DistanceTo(toCell.coordinates);
                    _searchFrontier.Enqueue(neighbor);
                }
                else if (distance < neighbor.Distance)
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.Distance = distance;
                    neighbor.PathFrom = current;
                    _searchFrontier.Change(neighbor, oldPriority);
                }
            }
        }

        return false;
    }

    public void IncreaseVisibility(HexCell fromCell, int range)
    {
        List<HexCell> cells = GetVisibleCells(fromCell, range);

        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].IncreaseVisibility();
        }

        ListPool<HexCell>.Add(cells);
    }

    public void DecreaseVisibility(HexCell fromCell, int range)
    {
        List<HexCell> cells = GetVisibleCells(fromCell, range);

        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].DecreaseVisibility();
        }

        ListPool<HexCell>.Add(cells);
    }

    public void ResetVisibility()
    {
        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i].ResetVisibility();
        }
        for (int i = 0; i < _units.Count; i++)
        {
            HexUnit unit = _units[i];
            IncreaseVisibility(unit.Location, unit.VisionRange);
        }
    }

    private List<HexCell> GetVisibleCells(HexCell fromCell, int range)
    {
        List<HexCell> visibleCells = ListPool<HexCell>.Get();

        _searchFrontierPhase += 2;

        if (_searchFrontier == null)
        {
            _searchFrontier = new HexCellPriorityQueue();
        }
        else
        {
            _searchFrontier.Clear();
        }

        range += fromCell.ViewElevation;
        fromCell.SearchPhase = _searchFrontierPhase;
        fromCell.Distance = 0;
        _searchFrontier.Enqueue(fromCell);
        HexCoordinates fromCoordinates = fromCell.coordinates;

        while (_searchFrontier.Count > 0)
        {
            HexCell current = _searchFrontier.Dequeue();
            current.SearchPhase += 1;
            visibleCells.Add(current);

            for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
            {
                HexCell neighbor = current.GetNeighbor(d);

                if (neighbor == null || neighbor.SearchPhase > _searchFrontierPhase || !neighbor.Explorable)
                {
                    continue;
                }

                int distance = current.Distance + 1;

                if (distance + neighbor.ViewElevation > range || distance > fromCoordinates.DistanceTo(neighbor.coordinates)) 
                {
                    continue;
                }

                if (neighbor.SearchPhase < _searchFrontierPhase)
                {
                    neighbor.SearchPhase = _searchFrontierPhase;
                    neighbor.Distance = distance;
                    neighbor.SearchHeuristic = 0;
                    _searchFrontier.Enqueue(neighbor);
                }
                else if (distance < neighbor.Distance)
                {
                    int oldPriority = neighbor.SearchPriority;
                    neighbor.Distance = distance;
                    _searchFrontier.Change(neighbor, oldPriority);
                }
            }
        }

        return visibleCells;
    }

    public void CenterMap(float xPosition)
    {
        int centerColumnIndex = (int)(xPosition / (HexMetrics.InnerDiameter * HexMetrics.ChunkSizeX));

        if (centerColumnIndex == _currentCenterColumnIndex)
        {
            return;
        }

        _currentCenterColumnIndex = centerColumnIndex;

        int minColumnIndex = centerColumnIndex - _chunkCountX / 2;
        int maxColumnIndex = centerColumnIndex + _chunkCountX / 2;

        Vector3 position;
        position.y = position.z = 0f;

        for (int i = 0; i < _columns.Length; i++)
        {
            if (i < minColumnIndex)
            {
                position.x = _chunkCountX *
                    (HexMetrics.InnerDiameter * HexMetrics.ChunkSizeX);
            }
            else if (i > maxColumnIndex)
            {
                position.x = _chunkCountX *
                    -(HexMetrics.InnerDiameter * HexMetrics.ChunkSizeX);
            }
            else
            {
                position.x = 0f;
            }
            _columns[i].localPosition = position;
        }
    }
}