using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellHolder : MonoBehaviour
{
    [SerializeField] private HexGrid _grid;
    [SerializeField] private GameObject[] _prefabs;

    private GameObject _prefab = null;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UseAbility();
        }
    }

    public void SetAbility(int index)
    {
        if (index < 0)
        {
            _prefab = null;
            return;
        }

        if (index > _prefabs.Length)
            return;

        _prefab = _prefabs[index];
    }

    private void UseAbility()
    {
        if (_prefab != null)
        {
            if (GetCellUnderCursor(out Vector3 position))
            {
                var ability = Instantiate(_prefab, position, _prefab.transform.rotation);
            }
        }
    }

    private bool GetCellUnderCursor(out Vector3 position)
    {
        var cell = _grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));

        if(cell == null)
        {
            position = Vector3.zero;
            return false;
        }

        position = cell.transform.position;
        return true;
    }
}
