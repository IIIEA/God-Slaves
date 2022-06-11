using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellHolder : MonoBehaviour
{
    [SerializeField] private HexGrid _grid;
    [SerializeField] private GameObject _prefab;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UseAbility();
        }
    }

    private void UseAbility()
    {
        var position = GetCellUnderCursor();

        var ability = Instantiate(_prefab, position, _prefab.transform.rotation);
    }

    private Vector3 GetCellUnderCursor()
    {
        return _grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition)).transform.position;
    }
}
