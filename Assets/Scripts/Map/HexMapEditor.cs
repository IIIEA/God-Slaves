using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HexMapEditor : MonoBehaviour
{
	[SerializeField] private Color[] _colors;
	[SerializeField] private HexGrid _hexGrid;

	private Color _activeColor;
	private int _activeElevation;

	void Awake()
	{
		SelectColor(0);
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			HandleInput();
		}
	}

	public void SetElevation(Slider slider)
	{
		_activeElevation = (int)slider.value;
	}

	public void SelectColor(int index)
	{
		_activeColor = _colors[index];
	}

	private void HandleInput()
	{
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(inputRay, out hit))
		{
			EditCell(_hexGrid.GetCell(hit.point));
		}
	}

	private void EditCell(HexCell cell)
	{
		cell.Color = _activeColor;
		cell.Elevation = _activeElevation;
		_hexGrid.Refresh();
	}
}