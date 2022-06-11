using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class HexCell : MonoBehaviour
{
    public HexCoordinates coordinates;
    public RectTransform uiRect;
    public HexGridChunk chunk;

    #region Properties
    public int Index { get; set; }

    public int ColumnIndex { get; set; }

    public int Elevation
    {
        get
        {
            return _elevation;
        }
        set
        {
            if (_elevation == value)
            {
                return;
            }

            int originalViewElevation = ViewElevation;
            _elevation = value;

            if (ViewElevation != originalViewElevation)
            {
                ShaderData.ViewElevationChanged();
            }
            RefreshPosition();
            ValidateRivers();

            for (int i = 0; i < roads.Length; i++)
            {
                if (roads[i] && GetElevationDifference((HexDirection)i) > 1)
                {
                    SetRoad(i, false);
                }
            }

            Refresh();
        }
    }

    public int WaterLevel
    {
        get
        {
            return _waterLevel;
        }
        set
        {
            if (_waterLevel == value)
            {
                return;
            }

            int originalViewElevation = ViewElevation;
            _waterLevel = value;

            if (ViewElevation != originalViewElevation)
            {
                ShaderData.ViewElevationChanged();
            }

            ValidateRivers();
            Refresh();
        }
    }

    public int ViewElevation => _elevation >= _waterLevel ? _elevation : _waterLevel;

    public bool IsUnderwater => _waterLevel > _elevation;

    public bool HasIncomingRiver => _hasIncomingRiver;

    public bool HasOutgoingRiver => _hasOutgoingRiver;

    public bool HasRiver => _hasIncomingRiver || _hasOutgoingRiver;

    public bool HasRiverBeginOrEnd => _hasIncomingRiver != _hasOutgoingRiver;

    public HexDirection RiverBeginOrEndDirection => _hasIncomingRiver ? _incomingRiver : _outgoingRiver;

    public HexDirection IncomingRiver => _incomingRiver;

    public HexDirection OutgoingRiver => _outgoingRiver;

    public Vector3 Position => transform.localPosition;

    public bool HasRoads
    {
        get
        {
            for (int i = 0; i < roads.Length; i++)
            {
                if (roads[i])
                {
                    return true;
                }
            }
            return false;
        }
    }

    public float StreamBedY => (_elevation + HexMetrics.StreamBedElevationOffset) * HexMetrics.ElevationStep;

    public float RiverSurfaceY => (_elevation + HexMetrics.WaterElevationOffset) * HexMetrics.ElevationStep;

    public float WaterSurfaceY => (_waterLevel + HexMetrics.WaterElevationOffset) * HexMetrics.ElevationStep;

    public int UrbanLevel
    {
        get
        {
            return _urbanLevel;
        }
        set
        {
            if (_urbanLevel != value)
            {
                _urbanLevel = value;
                RefreshSelfOnly();
            }
        }
    }

    public int FarmLevel
    {
        get
        {
            return _farmLevel;
        }
        set
        {
            if (_farmLevel != value)
            {
                _farmLevel = value;
                RefreshSelfOnly();
            }
        }
    }

    public int PlantLevel
    {
        get
        {
            return _plantLevel;
        }
        set
        {
            if (_plantLevel != value)
            {
                _plantLevel = value;
                RefreshSelfOnly();
            }
        }
    }

    public int SpecialIndex
    {
        get
        {
            return _specialIndex;
        }
        set
        {
            if (_specialIndex != value && !HasRiver)
            {
                _specialIndex = value;
                RemoveRoads();
                RefreshSelfOnly();
            }
        }
    }

    public bool IsSpecial => _specialIndex > 0;

    public bool Walled
    {
        get
        {
            return _walled;
        }
        set
        {
            if (_walled != value)
            {
                _walled = value;
                Refresh();
            }
        }
    }

    public int TerrainTypeIndex
    {
        get
        {
            return _terrainTypeIndex;
        }
        set
        {
            if (_terrainTypeIndex != value)
            {
                _terrainTypeIndex = value;
                ShaderData.RefreshTerrain(this);
            }
        }
    }

    public bool IsVisible => _visibility > 0 && Explorable;

    public bool IsExplored
    {
        get
        {
            return _explored && Explorable;
        }
        private set
        {
            _explored = value;
        }
    }

    public bool Explorable { get; set; }

    public int Distance
    {
        get
        {
            return _distance;
        }
        set
        {
            _distance = value;
        }
    }

    public HexUnit Unit { get; set; }

    public HexCell PathFrom { get; set; }

    public int SearchHeuristic { get; set; }

    public int SearchPriority => _distance + SearchHeuristic;

    public int SearchPhase { get; set; }

    public HexCell NextWithSamePriority { get; set; }

    public HexCellShaderData ShaderData { get; set; }

#endregion

    [SerializeField] private HexCell[] neighbors;
    [SerializeField] private bool[] roads;

    private int _terrainTypeIndex;
    private int _elevation = int.MinValue;
    private int _waterLevel;
    private int _urbanLevel, _farmLevel, _plantLevel;
    private int _specialIndex;
    private int _distance;
    private int _visibility;
    private bool _explored;
    private bool _walled;
    private bool _hasIncomingRiver, _hasOutgoingRiver;
    private HexDirection _incomingRiver, _outgoingRiver;

    public void IncreaseVisibility()
    {
        _visibility += 1;

        if (_visibility == 1)
        {
            IsExplored = true;
            ShaderData.RefreshVisibility(this);
        }
    }

    public void DecreaseVisibility()
    {
        _visibility -= 1;

        if (_visibility == 0)
        {
            ShaderData.RefreshVisibility(this);
        }
    }

    public void ResetVisibility()
    {
        if (_visibility > 0)
        {
            _visibility = 0;
            ShaderData.RefreshVisibility(this);
        }
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(_elevation, neighbors[(int)direction]._elevation);
    }

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(_elevation, otherCell._elevation);
    }

    public bool HasRiverThroughEdge(HexDirection direction)
    {
        return _hasIncomingRiver && _incomingRiver == direction || _hasOutgoingRiver && _outgoingRiver == direction;
    }

    public void RemoveIncomingRiver()
    {
        if (!_hasIncomingRiver)
        {
            return;
        }

        _hasIncomingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(_incomingRiver);
        neighbor._hasOutgoingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveOutgoingRiver()
    {
        if (!_hasOutgoingRiver)
        {
            return;
        }

        _hasOutgoingRiver = false;
        RefreshSelfOnly();

        HexCell neighbor = GetNeighbor(_outgoingRiver);
        neighbor._hasIncomingRiver = false;
        neighbor.RefreshSelfOnly();
    }

    public void RemoveRiver()
    {
        RemoveOutgoingRiver();
        RemoveIncomingRiver();
    }

    public void SetOutgoingRiver(HexDirection direction)
    {
        if (_hasOutgoingRiver && _outgoingRiver == direction)
        {
            return;
        }

        HexCell neighbor = GetNeighbor(direction);

        if (!IsValidRiverDestination(neighbor))
        {
            return;
        }

        RemoveOutgoingRiver();

        if (_hasIncomingRiver && _incomingRiver == direction)
        {
            RemoveIncomingRiver();
        }

        _hasOutgoingRiver = true;
        _outgoingRiver = direction;
        _specialIndex = 0;

        neighbor.RemoveIncomingRiver();
        neighbor._hasIncomingRiver = true;
        neighbor._incomingRiver = direction.Opposite();
        neighbor._specialIndex = 0;

        SetRoad((int)direction, false);
    }

    public bool HasRoadThroughEdge(HexDirection direction)
    {
        return roads[(int)direction];
    }

    public void AddRoad(HexDirection direction)
    {
        if (!roads[(int)direction] && !HasRiverThroughEdge(direction) && !IsSpecial && !GetNeighbor(direction).IsSpecial && GetElevationDifference(direction) <= 1)
        {
            SetRoad((int)direction, true);
        }
    }

    public void RemoveRoads()
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (roads[i])
            {
                SetRoad(i, false);
            }
        }
    }

    public int GetElevationDifference(HexDirection direction)
    {
        int difference = _elevation - GetNeighbor(direction)._elevation;
        return difference >= 0 ? difference : -difference;
    }

    private bool IsValidRiverDestination(HexCell neighbor)
    {
        return neighbor && (_elevation >= neighbor._elevation || _waterLevel == neighbor._elevation);
    }

    private void ValidateRivers()
    {
        if (_hasOutgoingRiver && !IsValidRiverDestination(GetNeighbor(_outgoingRiver)))
        {
            RemoveOutgoingRiver();
        }
        if (_hasIncomingRiver && !GetNeighbor(_incomingRiver).IsValidRiverDestination(this)) 
        {
            RemoveIncomingRiver();
        }
    }

    private void SetRoad(int index, bool state)
    {
        roads[index] = state;
        neighbors[index].roads[(int)((HexDirection)index).Opposite()] = state;
        neighbors[index].RefreshSelfOnly();
        RefreshSelfOnly();
    }

    private void RefreshPosition()
    {
        Vector3 position = transform.localPosition;
        position.y = _elevation * HexMetrics.ElevationStep;
        position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.ElevationPerturbStrength;
        transform.localPosition = position;

        Vector3 uiPosition = uiRect.localPosition;
        uiPosition.z = -position.y;
        uiRect.localPosition = uiPosition;
    }

    private void Refresh()
    {
        if (chunk)
        {
            chunk.Refresh();

            for (int i = 0; i < neighbors.Length; i++)
            {
                HexCell neighbor = neighbors[i];
                if (neighbor != null && neighbor.chunk != chunk)
                {
                    neighbor.chunk.Refresh();
                }
            }
            if (Unit)
            {
                Unit.ValidateLocation();
            }
        }
    }

    private void RefreshSelfOnly()
    {
        chunk.Refresh();

        if (Unit)
        {
            Unit.ValidateLocation();
        }
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write((byte)_terrainTypeIndex);
        writer.Write((byte)(_elevation + 127));
        writer.Write((byte)_waterLevel);
        writer.Write((byte)_urbanLevel);
        writer.Write((byte)_farmLevel);
        writer.Write((byte)_plantLevel);
        writer.Write((byte)_specialIndex);
        writer.Write(_walled);

        if (_hasIncomingRiver)
        {
            writer.Write((byte)(_incomingRiver + 128));
        }
        else
        {
            writer.Write((byte)0);
        }

        if (_hasOutgoingRiver)
        {
            writer.Write((byte)(_outgoingRiver + 128));
        }
        else
        {
            writer.Write((byte)0);
        }

        int roadFlags = 0;

        for (int i = 0; i < roads.Length; i++)
        {
            if (roads[i])
            {
                roadFlags |= 1 << i;
            }
        }

        writer.Write((byte)roadFlags);
        writer.Write(IsExplored);
    }

    public void Load(BinaryReader reader, int header)
    {
        _terrainTypeIndex = reader.ReadByte();
        ShaderData.RefreshTerrain(this);
        _elevation = reader.ReadByte();

        if (header >= 4)
        {
            _elevation -= 127;
        }

        RefreshPosition();

        _waterLevel = reader.ReadByte();
        _urbanLevel = reader.ReadByte();
        _farmLevel = reader.ReadByte();
        _plantLevel = reader.ReadByte();
        _specialIndex = reader.ReadByte();
        _walled = reader.ReadBoolean();

        byte riverData = reader.ReadByte();

        if (riverData >= 128)
        {
            _hasIncomingRiver = true;
            _incomingRiver = (HexDirection)(riverData - 128);
        }
        else
        {
            _hasIncomingRiver = false;
        }

        riverData = reader.ReadByte();
        if (riverData >= 128)
        {
            _hasOutgoingRiver = true;
            _outgoingRiver = (HexDirection)(riverData - 128);
        }
        else
        {
            _hasOutgoingRiver = false;
        }

        int roadFlags = reader.ReadByte();
        for (int i = 0; i < roads.Length; i++)
        {
            roads[i] = (roadFlags & (1 << i)) != 0;
        }

        IsExplored = header >= 3 ? reader.ReadBoolean() : false;
        ShaderData.RefreshVisibility(this);
    }

    public void SetLabel(string text)
    {
        UnityEngine.UI.Text label = uiRect.GetComponent<Text>();
        label.text = text;
    }

    public void DisableHighlight()
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.enabled = false;
    }

    public void EnableHighlight(Color color)
    {
        Image highlight = uiRect.GetChild(0).GetComponent<Image>();
        highlight.color = color;
        highlight.enabled = true;
    }

    public void SetMapData(float data)
    {
        ShaderData.SetMapData(this, data);
    }
}