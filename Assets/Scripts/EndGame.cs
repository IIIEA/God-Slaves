using UnityEngine;

public class EndGame : MonoBehaviour
{
    [SerializeField] private HexGrid _grid;
    [SerializeField] private GameObject[] _objectsToActivate;

    private void OnEnable()
    {
        _grid.UnitCountChanged += OnUnitsChanged;
    }

    private void OnDisable()
    {
        _grid.UnitCountChanged -= OnUnitsChanged;
    }

    private void OnUnitsChanged(int count)
    {
        if(count <= 0)
        {
            foreach (var deactive in _objectsToActivate)
            {
                deactive.SetActive(true);
            }
        }
    }
}
