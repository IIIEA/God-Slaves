using UnityEngine;

public class Crater : MonoBehaviour
{
    [SerializeField] private float _damageRadius = 5f;

    private void OnEnable()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, _damageRadius);

        foreach (var collider in objectsInRange)
        {
            if(collider.TryGetComponent(out HexUnit unit))
            {
                unit.Die();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawSphere(transform.position, _damageRadius);
    }
}
