using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class UnitsCounter : MonoBehaviour
{
    [SerializeField] private HexGrid _grid;

    private TMP_Text _text;

    private void Start()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        _grid.UnitCountChanged += OnUnitsCountChanged;
    }

    private void OnDisable()
    {
        _grid.UnitCountChanged -= OnUnitsCountChanged;
    }

    private void OnUnitsCountChanged(int count)
    {
        _text.text = count.ToString();
    }
}
