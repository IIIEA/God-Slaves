using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HexMapEditor : MonoBehaviour
{
	[SerializeField] private Color[] _colors;
	[SerializeField] private HexGrid _hexGrid;

	private int _brushSize;
	private bool _applyElevation = true;
	private bool _applyColor;
	private Color _activeColor;
	private int _activeElevation;

	void Awake()
	{
		SelectColor(0);
	}

	void Update()
	{
		if (Input.GetMouseButton(0))
		{
			HandleInput();
		}
	}

	public void ShowUI(Toggle visible)
	{
		_hexGrid.ShowUI(visible);
	}

	public void SetApplyElevation(Toggle toggle)
	{
		_applyElevation = toggle;
	}

	public void SetBrushSize(Slider size)
	{
		_brushSize = (int)size.value;
	}

	public void SetElevation(Slider slider)
	{
		_activeElevation = (int)slider.value;
	}

	public void SelectColor(int index)
    {
        _applyColor = index >= 0;

        if (_applyColor)
		{
			_activeColor = _colors[index];
		}
	}

	private void HandleInput()
	{
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(inputRay, out hit))
		{
			EditCells(_hexGrid.GetCell(hit.point));
		}
	}

	private void EditCells(HexCell center)
	{
		int centerX = center.Coordinates.X;
		int centerZ = center.Coordinates.Z;

		for (int r = 0, z = centerZ - _brushSize; z <= centerZ; z++, r++)
		{
			for (int x = centerX - r; x <= centerX + _brushSize; x++)
			{
				EditCell(_hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}

		for (int r = 0, z = centerZ + _brushSize; z > centerZ; z--, r++)
		{
			for (int x = centerX - _brushSize; x <= centerX + r; x++)
			{
				EditCell(_hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
	}

	private void EditCell(HexCell cell)
	{
		if (cell)
		{
			if (_applyColor)
			{
				cell.Color = _activeColor;
			}

			if (_applyElevation)
			{
				cell.Elevation = _activeElevation;
			}
		}
	}
}