using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridChunk : MonoBehaviour
{
	private HexCell[] _cells;
	private HexMesh _hexMesh;
	private Canvas _gridCanvas;

	private void Awake()
	{
		_gridCanvas = GetComponentInChildren<Canvas>();
		_hexMesh = GetComponentInChildren<HexMesh>();

		_cells = new HexCell[HexMetrics.ChunkSizeX * HexMetrics.ChunkSizeZ];
		ShowUI(false);
	}

	private void LateUpdate()
	{
		_hexMesh.Triangulate(_cells);
		enabled = false;
	}

	public void ShowUI(bool visible)
	{
		_gridCanvas.gameObject.SetActive(visible);
	}

	public void AddCell(int index, HexCell cell)
	{
		_cells[index] = cell;
		cell.Chunk = this;
		cell.transform.SetParent(transform, false);
		cell.UIRect.SetParent(_gridCanvas.transform, false);
	}

	public void Refresh()
	{
		enabled = true;
	}
}
