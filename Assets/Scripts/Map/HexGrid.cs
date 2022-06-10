using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
	[Header("Links")]
	[SerializeField] private TMP_Text _cellLabelPrefab;
	[SerializeField] private HexCell _cellPrefab;
	[SerializeField] private HexGridChunk _chunkPrefab;
	[SerializeField] private Texture2D _noiseSource;
	[Header("Map parameters")]
	[SerializeField] private int _chunkCountX = 4;
	[SerializeField] private int _chunkCountZ = 3;
	[SerializeField] private Color _defaultColor = Color.white;

	private int _cellCountX, _cellCountZ;
	private HexCell[] _cells;
	private HexGridChunk[] _chunks;

	public int ChunkCountX => _chunkCountX;
	public int ChunkCountZ => _chunkCountZ;

	void Awake()
	{
		HexMetrics.NoiseSource = _noiseSource;

		_cellCountX = _chunkCountX * HexMetrics.ChunkSizeX;
		_cellCountZ = _chunkCountZ * HexMetrics.ChunkSizeZ;

		CreateChunks();
		CreateCells();
	}

    private void OnEnable()
    {
		HexMetrics.NoiseSource = _noiseSource;
	}

	public void ShowUI(bool visible)
	{
		for (int i = 0; i < _chunks.Length; i++)
		{
			_chunks[i].ShowUI(visible);
		}
	}

	private void CreateChunks()
	{
		_chunks = new HexGridChunk[_chunkCountX * _chunkCountZ];

		for (int z = 0, i = 0; z < _chunkCountZ; z++)
		{
			for (int x = 0; x < _chunkCountX; x++)
			{
				HexGridChunk chunk = _chunks[i++] = Instantiate(_chunkPrefab);
				chunk.transform.SetParent(transform);
			}
		}
	}

	public HexCell GetCell(HexCoordinates coordinates)
	{
		int z = coordinates.Z;

		if (z < 0 || z >= _cellCountZ)
		{
			return null;
		}

		int x = coordinates.X + z / 2;

		if (x < 0 || x >= _cellCountX)
		{
			return null;
		}

		return _cells[x + z * _cellCountX];
	}

	public HexCell GetCell(Vector3 position)
	{
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		int index = coordinates.X + coordinates.Z * _cellCountX + coordinates.Z / 2;
		return _cells[index];
	}

	private void CreateCells()
    {
		_cells = new HexCell[_cellCountZ * _cellCountX];

		for (int z = 0, i = 0; z < _cellCountZ; z++)
		{
			for (int x = 0; x < _cellCountX; x++)
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
				cell.SetNeighbor(HexDirection.SE, _cells[i - _cellCountX]);

				if (x > 0)
				{
					cell.SetNeighbor(HexDirection.SW, _cells[i - _cellCountX - 1]);
				}
			}
			else
			{
				cell.SetNeighbor(HexDirection.SW, _cells[i - _cellCountX]);

				if (x < _cellCountX - 1)
				{
					cell.SetNeighbor(HexDirection.SE, _cells[i - _cellCountX + 1]);
				}
			}
		}

		TMP_Text label = Instantiate<TMP_Text>(_cellLabelPrefab);
		label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
		label.text = cell.Coordinates.ToStringOnSeparateLines();

		cell.UIRect = label.rectTransform;
		cell.Elevation = 0;

		AddCellToChunk(x, z, cell);
	}

    private void AddCellToChunk(int x, int z, HexCell cell)
    {
		int chunkX = x / HexMetrics.ChunkSizeX;
		int chunkZ = z / HexMetrics.ChunkSizeZ;
		HexGridChunk chunk = _chunks[chunkX + chunkZ * _chunkCountX];

		int localX = x - chunkX * HexMetrics.ChunkSizeX;
		int localZ = z - chunkZ * HexMetrics.ChunkSizeZ;
		chunk.AddCell(localX + localZ * HexMetrics.ChunkSizeX, cell);
	}
}
