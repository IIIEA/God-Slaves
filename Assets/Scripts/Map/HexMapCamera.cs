using UnityEngine;

public class HexMapCamera : MonoBehaviour
{
	[SerializeField] private HexGrid _grid;
	[SerializeField] private float _stickMinZoom, _stickMaxZoom;
	[SerializeField] private float _swivelMinZoom, _swivelMaxZoom;
	[SerializeField] private float _moveSpeedMinZoom, _moveSpeedMaxZoom;
	[SerializeField] private float _rotationSpeed;

	private float _rotationAngle;
	private Transform _swivel, _stick;
	private float _zoom = 1f;

	void Awake()
	{
		_swivel = transform.GetChild(0);
		_stick = _swivel.GetChild(0);
	}

	private void Update()
	{
		float zoomDelta = Input.GetAxis("Mouse ScrollWheel");

		if (zoomDelta != 0f)
		{
			AdjustZoom(zoomDelta);
		}

		float rotationDelta = Input.GetAxis("Rotation");

		if (rotationDelta != 0f)
		{
			AdjustRotation(rotationDelta);
		}

		float xDelta = Input.GetAxis("Horizontal");
		float zDelta = Input.GetAxis("Vertical");

		if (xDelta != 0f || zDelta != 0f)
		{
			AdjustPosition(xDelta, zDelta);
		}
	}

	private void AdjustZoom(float delta)
	{
		_zoom = Mathf.Clamp01(_zoom + delta);


		float distance = Mathf.Lerp(_stickMinZoom, _stickMaxZoom, _zoom);
		_stick.localPosition = new Vector3(0f, 0f, distance);

		float angle = Mathf.Lerp(_swivelMinZoom, _swivelMaxZoom, _zoom);
		_swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
	}

	private void AdjustPosition(float xDelta, float zDelta)
	{
		Vector3 direction = transform.localRotation * new Vector3(xDelta, 0f, zDelta).normalized;
		float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
		float distance = Mathf.Lerp(_moveSpeedMinZoom, _moveSpeedMaxZoom, _zoom) * damping * Time.deltaTime;

		Vector3 position = transform.localPosition;
		position += direction * distance;
		transform.localPosition = ClampPosition(position);
	}

	private Vector3 ClampPosition(Vector3 position)
	{
		float xMax = (_grid.ChunkCountX * HexMetrics.ChunkSizeX - 0.5f) * (2f * HexMetrics.InnerRadius);
		position.x = Mathf.Clamp(position.x, 0f, xMax);

		float zMax = (_grid.ChunkCountZ * HexMetrics.ChunkSizeZ - 1) * (1.5f * HexMetrics.OuterRadius);
		position.z = Mathf.Clamp(position.z, 0f, zMax);

		return position;
	}

	private void AdjustRotation(float delta)
	{
		_rotationAngle += delta * _rotationSpeed * Time.deltaTime;

		if (_rotationAngle < 0f)
		{
			_rotationAngle += 360f;
		}
		else if (_rotationAngle >= 360f)
		{
			_rotationAngle -= 360f;
		}

		transform.localRotation = Quaternion.Euler(0f, _rotationAngle, 0f);
	}
}