using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsHolder : MonoBehaviour
{
    [SerializeField] private List<HexUnit> _unitsPrefabs = new List<HexUnit>();

    private static List<HexUnit> _units = new List<HexUnit>();

    private void Awake()
    {
        _units = _unitsPrefabs;
    }

    public static HexUnit GetRandomUnit()
    {
        if (_units.Count > 0)
        {
            var index = Random.Range(0, _units.Count);

            return _units[index];
        }
        else
        {
            return null;
        }
    }
}
