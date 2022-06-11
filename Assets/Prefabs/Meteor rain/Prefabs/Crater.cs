using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crater : MonoBehaviour
{
    private void OnEnable()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, 25f);

        foreach (var collider in objectsInRange)
        {
            if(collider.TryGetComponent<HexUnit>(out HexUnit unit))
            {
                unit.Die();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(transform.position, 25f);
    }
}
