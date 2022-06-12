using UnityEngine;

public class TornadoCaughter : MonoBehaviour
{
    private Tornado _tornadoReference;
    private SpringJoint _spring;
    private Rigidbody _rigidbody;

    public Rigidbody Rigidbody => _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_spring != null)
        {
            Vector3 newPosition = _spring.connectedAnchor;
            newPosition.y = transform.position.y;
            _spring.connectedAnchor = newPosition;
        }
    }

    private void FixedUpdate()
    {
        if (_tornadoReference == null)
            return;

        Vector3 direction = transform.position - _tornadoReference.transform.position;
        Vector3 projection = Vector3.ProjectOnPlane(direction, _tornadoReference.RotationAxis);
        projection.Normalize();

        Vector3 normal = Quaternion.AngleAxis(130, _tornadoReference.RotationAxis) * projection;
        normal = Quaternion.AngleAxis(_tornadoReference.Lift, projection) * normal;
        _rigidbody.AddForce(normal * _tornadoReference.RotationStrength, ForceMode.Force);

        Debug.DrawRay(transform.position, normal * 10, Color.red);
    }

    public void Init(Tornado tornadoRef, Rigidbody tornadoRigidbody, float springForce)
    {
        enabled = true;

        _tornadoReference = tornadoRef;

        _spring = gameObject.AddComponent<SpringJoint>();
        _spring.spring = springForce;
        _spring.connectedBody = tornadoRigidbody;

        _spring.autoConfigureConnectedAnchor = false;

        Vector3 initialPosition = Vector3.zero;
        initialPosition.y = transform.position.y;
        _spring.connectedAnchor = initialPosition;
    }

    public void Release()
    {
        enabled = false;
        Destroy(_spring);
    }
}