using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCell : MonoBehaviour
{
    [SerializeField] private HexCell[] _neighbors = new HexCell[6];

    private int _elevation = int.MinValue;

    public HexGridChunk Chunk;
    public RectTransform UIRect;
    public HexCoordinates Coordinates;
    public Color _color;

    public Vector3 Position => transform.localPosition;

    public int Elevation
    {
        get
        {
            return _elevation;
        }
        set
        {
            if (_elevation == value)
                return;
            
            Vector3 position = transform.localPosition;
            position.y = value * HexMetrics.ElevationStep;
            position.y += (HexMetrics.SampleNoise(position).y * 2f - 1f) * HexMetrics.ElevationPerturbStrength;
            transform.localPosition = position;

            Vector3 uiPosition = UIRect.localPosition;
            uiPosition.z = -position.y;
            UIRect.localPosition = uiPosition;

            Refresh();
        }
    }

    public Color Color
    {
        get
        {
            return _color;
        }
        set
        {
            if (_color == value)
                return;
            
            _color = value;

            Refresh();
        }
    }

    public HexEdgeType GetEdgeType(HexCell otherCell)
    {
        return HexMetrics.GetEdgeType(Elevation, otherCell.Elevation);
    }

    public HexEdgeType GetEdgeType(HexDirection direction)
    {
        return HexMetrics.GetEdgeType(Elevation, _neighbors[(int)direction].Elevation);
    }

    public HexCell GetNeighbor(HexDirection direction)
    {
        return _neighbors[(int)direction];
    }

    public void SetNeighbor(HexDirection direction, HexCell cell)
    {
        _neighbors[(int)direction] = cell;
        cell._neighbors[(int)direction.Opposite()] = this;
    }

    private void Refresh()
    {
        if (Chunk)
        {
            Chunk.Refresh();

            for (int i = 0; i < _neighbors.Length; i++)
            {
                HexCell neighbor = _neighbors[i];
                if (neighbor != null && neighbor.Chunk != Chunk)
                {
                    neighbor.Chunk.Refresh();
                }
            }
        }
    }
}

public static class HexDirectionExtensions
{
    public static HexDirection Opposite(this HexDirection direction)
    {
        return (int)direction < 3 ? (direction + 3) : (direction - 3);
    }

    public static HexDirection Previous(this HexDirection direction)
    {
        return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
    }

    public static HexDirection Next(this HexDirection direction)
    {
        return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
    }
}

public enum HexDirection
{
    NE, E, SE, SW, W, NW
}

public enum HexEdgeType
{
    Flat, Slope, Cliff
}
