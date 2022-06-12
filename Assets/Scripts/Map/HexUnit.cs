using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using DG.Tweening;

public class HexUnit : MonoBehaviour
{
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private AudioClip _panicAudio;
    [SerializeField] private AudioSource _source;

    private float _orientation;
    private List<HexCell> _pathToTravel;
    private HexCell location, currentTravelLocation;
    private bool _isAttached;

    const float rotationSpeed = 180f;
    const float travelSpeed = 2f;

    public static HexUnit unitPrefab;

    public bool IsAtached => _isAttached;
    public HexCell CellToTravel { get; private set; }
    public bool InTravel { get; private set; }
    public HexGrid Grid { get; set; }

    public event Action<HexUnit, HexCell> Treveled;
    public event Action<HexUnit> Died;

    public HexCell Location
    {
        get
        {
            return location;
        }
        set
        {
            if (location)
            {
                Grid.DecreaseVisibility(location, VisionRange);
                location.Unit = null;
            }

            location = value;
            value.Unit = this;
            Grid.IncreaseVisibility(value, VisionRange);
            transform.localPosition = value.Position;
            Grid.MakeChildOfColumn(transform, value.ColumnIndex);
        }
    }

    public float Orientation
    {
        get
        {
            return _orientation;
        }
        set
        {
            _orientation = value;
            transform.localRotation = Quaternion.Euler(0f, value, 0f);
        }
    }

    public int Speed
    {
        get
        {
            return 24;
        }
    }

    public int VisionRange
    {
        get
        {
            return 3;
        }
    }

    private bool _turn = true;

    private void Start()
    {
        InTravel = false;
    }

    private void Update()
    {
        FindTravel();
    }

    private void OnDestroy()
    {
        DOTween.CompleteAll();
    }

    private void FindTravel()
    {
        if (InTravel == false)
        {
            Collider[] cells = Physics.OverlapSphere(transform.position, 100f, _groundLayer);
            int index;

            do
            {
                index = UnityEngine.Random.Range(0, cells.Length - 1);

                if (index > cells.Length - 1)
                {
                    _turn = true;
                    return;
                }

                CellToTravel = cells[index].GetComponent<HexCell>();

                if (CellToTravel == Location)
                {
                    _turn = true;
                    return;
                }

            } while (CellToTravel.IsUnderwater == true);

            Treveled?.Invoke(this, CellToTravel);

        }
    }

    public void ValidateLocation()
    {
        transform.localPosition = location.Position;
    }

    public bool IsValidDestination(HexCell cell)
    {
        return !cell.IsUnderwater;
    }

    public void SetAttached()
    {
        _isAttached = true;
    }

    public void Travel(List<HexCell> path)
    {
        _turn = true;
        InTravel = true;
        location.Unit = null;
        location = path[path.Count - 1];
        location.Unit = this;
        _pathToTravel = path;
        StopAllCoroutines();
        StartCoroutine(TravelPath());
    }

    IEnumerator TravelPath()
    {
        Vector3 a, b, c = _pathToTravel[0].Position;
        yield return LookAt(_pathToTravel[1].Position);

        if (!currentTravelLocation)
        {
            currentTravelLocation = _pathToTravel[0];
        }

        Grid.DecreaseVisibility(currentTravelLocation, VisionRange);
        int currentColumn = currentTravelLocation.ColumnIndex;

        float t = Time.deltaTime * travelSpeed;

        for (int i = 1; i < _pathToTravel.Count; i++)
        {
            currentTravelLocation = _pathToTravel[i];
            a = c;
            b = _pathToTravel[i - 1].Position;

            int nextColumn = currentTravelLocation.ColumnIndex;
            if (currentColumn != nextColumn)
            {
                if (nextColumn < currentColumn - 1)
                {
                    a.x -= HexMetrics.InnerDiameter * HexMetrics.wrapSize;
                    b.x -= HexMetrics.InnerDiameter * HexMetrics.wrapSize;
                }
                else if (nextColumn > currentColumn + 1)
                {
                    a.x += HexMetrics.InnerDiameter * HexMetrics.wrapSize;
                    b.x += HexMetrics.InnerDiameter * HexMetrics.wrapSize;
                }

                Grid.MakeChildOfColumn(transform, nextColumn);
                currentColumn = nextColumn;
            }

            c = (b + currentTravelLocation.Position) * 0.5f;
            Grid.IncreaseVisibility(_pathToTravel[i], VisionRange);

            for (; t < 1f; t += Time.deltaTime * travelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }

            Grid.DecreaseVisibility(_pathToTravel[i], VisionRange);
            t -= 1f;
        }

        currentTravelLocation = null;

        a = c;
        b = location.Position;
        c = b;
        Grid.IncreaseVisibility(location, VisionRange);

        for (; t < 1f; t += Time.deltaTime * travelSpeed)
        {
            transform.localPosition = Bezier.GetPoint(a, b, c, t);
            Vector3 d = Bezier.GetDerivative(a, b, c, t);
            d.y = 0f;
            transform.localRotation = Quaternion.LookRotation(d);
            yield return null;
        }

        transform.localPosition = location.Position;
        _orientation = transform.localRotation.eulerAngles.y;
        ListPool<HexCell>.Add(_pathToTravel);
        _pathToTravel = null;
        InTravel = false;
    }

    private IEnumerator LookAt(Vector3 point)
    {
        if (HexMetrics.Wrapping)
        {
            float xDistance = point.x - transform.localPosition.x;
            if (xDistance < -HexMetrics.InnerRadius * HexMetrics.wrapSize)
            {
                point.x += HexMetrics.InnerDiameter * HexMetrics.wrapSize;
            }
            else if (xDistance > HexMetrics.InnerRadius * HexMetrics.wrapSize)
            {
                point.x -= HexMetrics.InnerDiameter * HexMetrics.wrapSize;
            }
        }

        point.y = transform.localPosition.y;
        Quaternion fromRotation = transform.localRotation;
        Quaternion toRotation = Quaternion.LookRotation(point - transform.localPosition);
        float angle = Quaternion.Angle(fromRotation, toRotation);

        if (angle > 0f)
        {
            float speed = rotationSpeed / angle;
            for (
                float t = Time.deltaTime * speed;
                t < 1f;
                t += Time.deltaTime * speed
            )
            {
                transform.localRotation =
                    Quaternion.Slerp(fromRotation, toRotation, t);
                yield return null;
            }
        }

        transform.LookAt(point);
        _orientation = transform.localRotation.eulerAngles.y;
    }

    public int GetMoveCost(HexCell fromCell, HexCell toCell, HexDirection direction)
    {
        if (!IsValidDestination(toCell))
        {
            return -1;
        }

        HexEdgeType edgeType = fromCell.GetEdgeType(toCell);

        if (edgeType == HexEdgeType.Cliff)
        {
            return -1;
        }

        int moveCost;

        if (fromCell.HasRoadThroughEdge(direction))
        {
            moveCost = 1;
        }
        else if (fromCell.Walled != toCell.Walled)
        {
            return -1;
        }
        else
        {
            moveCost = edgeType == HexEdgeType.Flat ? 5 : 10;
            moveCost += toCell.UrbanLevel + toCell.FarmLevel + toCell.PlantLevel;
        }

        return moveCost;
    }

    public void Die()
    {
        _source.PlayOneShot(_panicAudio);

        AnimateDie();

        if (location)
        {
            Grid.DecreaseVisibility(location, VisionRange);
        }

        Died?.Invoke(this);
        location.Unit = null;
        Destroy(gameObject, 0.6f);
    }

    public void DieWithDelay()
    {
        _source.PlayOneShot(_panicAudio);

        AnimateDie();

        if (location)
        {
            Grid.DecreaseVisibility(location, VisionRange);
        }

        Died?.Invoke(this);
        location.Unit = null;
        Destroy(gameObject, 5f);
    }

    private void AnimateDie()
    {
        _isAttached = true;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(1.3f, 0.2f));
        sequence.Append(transform.DOScale(0f, 0.3f));
    }

    public void Save(BinaryWriter writer)
    {
        location.coordinates.Save(writer);
        writer.Write(_orientation);
    }

    public static void Load(BinaryReader reader, HexGrid grid)
    {
        HexCoordinates coordinates = HexCoordinates.Load(reader);
        float orientation = reader.ReadSingle();
        grid.AddUnit(Instantiate(unitPrefab), grid.GetCell(coordinates), orientation);
    }

    private void OnEnable()
    {
        if (location)
        {
            transform.localPosition = location.Position;

            if (currentTravelLocation)
            {
                Grid.IncreaseVisibility(location, VisionRange);
                Grid.DecreaseVisibility(currentTravelLocation, VisionRange);
                currentTravelLocation = null;
            }
        }
    }

    //	void OnDrawGizmos () {
    //		if (pathToTravel == null || pathToTravel.Count == 0) {
    //			return;
    //		}
    //
    //		Vector3 a, b, c = pathToTravel[0].Position;
    //
    //		for (int i = 1; i < pathToTravel.Count; i++) {
    //			a = c;
    //			b = pathToTravel[i - 1].Position;
    //			c = (b + pathToTravel[i].Position) * 0.5f;
    //			for (float t = 0f; t < 1f; t += 0.1f) {
    //				Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
    //			}
    //		}
    //
    //		a = c;
    //		b = pathToTravel[pathToTravel.Count - 1].Position;
    //		c = b;
    //		for (float t = 0f; t < 1f; t += 0.1f) {
    //			Gizmos.DrawSphere(Bezier.GetPoint(a, b, c, t), 2f);
    //		}
    //	}
}