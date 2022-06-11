using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    [SerializeField] private float _flyDuration;
    [SerializeField] private float _travelDistance;
    [SerializeField] private float _flySpeed;
    [SerializeField] private float _groundSpeed;
    [SerializeField] private float _meteorSize;
    [SerializeField] private float _spinSize;
    [SerializeField] private float _spinSpeed;
    [SerializeField] private float _destroyDelay;
    [SerializeField] private LayerMask _groundMask;

    private Vector3 _moveDirection, _landLocation;
    private bool _flying = true;
    private Vector3 _spawnOffset = Vector3.one * 5;

    private void Start()
    {
        _moveDirection = transform.position - (transform.position + _spawnOffset);
        transform.position = transform.position + Vector3.one * 5;

        Vector3 relativePos = _moveDirection - transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        transform.rotation = rotation;
    }

    private void Update()
    {
        RaycastHit hit;

        if (_flying)
        {
            if(_flyDuration > 0)
            {
                _flyDuration -= Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }

            transform.position = transform.position + _moveDirection * _flySpeed * Time.deltaTime;

            if(Physics.Raycast(transform.position, Vector3.down, out hit, _meteorSize, _groundMask))
            {
                _flying = false;
                _moveDirection.y = 0;
                _landLocation = hit.point;

            }
        }
        else
        {
            if(Physics.Raycast(transform.position, Vector3.down, out hit, _meteorSize, _groundMask))
            {
                Vector3 nextPosition = new Vector3(transform.position.x, hit.point.y + _meteorSize, transform.position.z);
                transform.position = nextPosition + _moveDirection * _groundSpeed * Time.deltaTime;
            }

            float distance = Vector3.Distance(_landLocation, transform.position);

            if(distance > _travelDistance)
            {
                Destroy(gameObject);
            }

            float spin = _spinSpeed * Time.deltaTime;
            transform.Rotate(spin, 0, 0);
        }
    }

    public void Init(Vector3 position)
    {
        transform.position = position;
    }

    private void SetDamage()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, _meteorSize);

        foreach (var collider in objectsInRange)
        {
            if(collider.TryGetComponent<HexUnit>(out HexUnit unit))
            {
                unit.Die();
            }
        }
    }
}
