using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;

    [SerializeField] private float _speed;

    [Tooltip("Distance after which the rotation physics starts")]
    [SerializeField] private float _maxDistance = 20;

    [Tooltip("The axis that the caught objects will rotate around")]
    [SerializeField] private Vector3 _rotationAxis = new Vector3(0, 1, 0);

    [Tooltip("Angle that is added to the object's velocity (higher lift -> quicker on top)")]
    [Range(0, 90)]
    [SerializeField] private float _lift = 45;

    [Tooltip("The force that will drive the caught objects around the tornado's center")]
    [SerializeField] private float _rotationStrength = 50;

    [Tooltip("Tornado pull force")]
    [SerializeField] private float _tornadoStrength = 2;

    private Vector3 _direction;
    private List<TornadoCaughter> _caughtObject = new List<TornadoCaughter>();
    private List<HexUnit> _units = new List<HexUnit>();

    public Vector3 RotationAxis => _rotationAxis;
    public float RotationStrength => _rotationStrength;
    public float Lift => _lift;

    private void Start()
    {
        _direction = GetRandomDirection();
        _rotationAxis.Normalize();
    }

    private void Update()
    {
        Move(_direction);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < _caughtObject.Count; i++)
        {
            if (_caughtObject[i] != null)
            {
                Vector3 pull = transform.position - _caughtObject[i].transform.position;

                if (pull.magnitude > _maxDistance)
                {
                    _caughtObject[i].Rigidbody.AddForce(pull.normalized * pull.magnitude, ForceMode.Force);
                    _caughtObject[i].enabled = false;
                }
                else
                {
                    _caughtObject[i].enabled = true;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out HexUnit unit))
        {
            if (!other.attachedRigidbody)
                return;

            unit.SetAttached();
            _units.Add(unit);

            if (other.attachedRigidbody.isKinematic)
                other.attachedRigidbody.isKinematic = false;

            TornadoCaughter caught = other.GetComponent<TornadoCaughter>();

            if (!caught)
            {
                caught = other.gameObject.AddComponent<TornadoCaughter>();
            }

            caught.Init(this, _rigidbody, _tornadoStrength);

            if (!_caughtObject.Contains(caught))
            {
                _caughtObject.Add(caught);
            }
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < _caughtObject.Count; i++)
        {
            if (_caughtObject[i] != null)
            {
                _caughtObject[i].enabled = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3[] positions = new Vector3[30];
        Vector3 centrePos = transform.position;

        for (int pointNum = 0; pointNum < positions.Length; pointNum++)
        {
            float i = (float)(pointNum * 2) / positions.Length;

            float angle = i * Mathf.PI * 2;

            float x = Mathf.Sin(angle) * _maxDistance;
            float z = Mathf.Cos(angle) * _maxDistance;

            Vector3 pos = new Vector3(x, 0, z) + centrePos;
            positions[pointNum] = pos;
        }

        Gizmos.color = Color.cyan;

        for (int i = 0; i < positions.Length; i++)
        {
            if (i == positions.Length - 1)
            {
                Gizmos.DrawLine(positions[0], positions[positions.Length - 1]);
            }
            else
            {
                Gizmos.DrawLine(positions[i], positions[i + 1]);
            }
        }
    }

    private void Move(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
        transform.position += direction * _speed * Time.deltaTime;
    }

    private Vector3 GetRandomDirection()
    {
        return Quaternion.Euler(0, Random.Range(-180f, 180f), 0) * transform.forward;
    }
}