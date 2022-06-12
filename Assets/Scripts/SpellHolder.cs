using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SpellHolder : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    [SerializeField] private HexGrid _grid;
    [SerializeField] private GameObject[] _prefabs;
    [SerializeField] private float _cooldown;

    private bool _isCooldown;
    private GameObject _prefab = null;
    private float _timer;

    void Update()
    {
        if (_isCooldown)
        {
            _slider.value += 1 / _cooldown * Time.deltaTime;

            if(_slider.value >= 0.99f)
            {
                _isCooldown = false;
            }
        }

        if (_isCooldown == false)
        {
            if (Input.GetMouseButtonDown(0))
            {
                UseAbility();
            }
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
                _isCooldown = true;
                _slider.value = 0;
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
