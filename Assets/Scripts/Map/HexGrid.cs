using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
	[Header("Links")]
	[SerializeField] private TMP_Text _cellLabelPrefab;
	[SerializeField] private HexCell _cellPrefab;
	[Header("Map parameters")]
	[SerializeField] private int _width = 6;
	[SerializeField] private int _height = 6;
	[SerializeField] private Color _defaultColor = Color.white;

	private HexMesh _hexMesh;
	private Canvas _gridCanvas;
	private HexCell[] _cells;

	void Awake()
	{
		_hexMesh = GetComponentInChildren<HexMesh>();
		_gridCanvas = GetComponentInChildren<Canvas>();
		CreateGrid();
	}

    private void Start()
    {
		_hexMesh.Triangulate(_cells);
    }

	public void Refresh()
	{
		_hexMesh.Triangulate(_cells);
	}

	public HexCell GetCell(Vector3 position)
	{
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * _width + coordinates.Z / 2;
		return _cells[index];
	}

	private void CreateGrid()
    {
		_cells = new HexCell[_height * _width];

		for (int z = 0, i = 0; z < _height; z++)
		{
			for (int x = 0; x < _width; x++)
			{
				CreateCell(x, z, i++);
			}
		}
	}

	private void CreateCell(int x, int z, int i)
	{
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.InnerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.OuterRadius * 1.5f);

		HexCell cell = _cells[i] = Instantiate<HexCell>(_cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.Coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
		cell.Color = _defaultColor;

		if (x > 0)
		{
			cell.SetNeighbor(HexDirection.W, _cells[i - 1]);
		}

		if (z > 0)
		{
			if ((z & 1) == 0)
			{
				cell.SetNeighbor(HexDirection.SE, _cells[i - _width]);

				if (x > 0)
				{
					cell.SetNeighbor(HexDirection.SW, _cells[i - _width - 1]);
				}
			}
			else
			{
				cell.SetNeighbor(HexDirection.SW, _cells[i - _width]);

				if (x < _width - 1)
				{
					cell.SetNeighbor(HexDirection.SE, _cells[i - _width + 1]);
				}
			}
		}

		TMP_Text label = Instantiate<TMP_Text>(_cellLabelPrefab);
		label.rectTransform.SetParent(_gridCanvas.transform, false);
		label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
		label.text = cell.Coordinates.ToStringOnSeparateLines();

		cell.UIRect = label.rectTransform;
	}
}
