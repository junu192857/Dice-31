using UnityEngine;
using UnityEngine.Events;

enum DiceState
{
    Dragging,
    Rolling,
    Idle
}

public class DiceController : MonoBehaviour
{
    public Vector3 DefaultPos;
    public GameObject plate;
    private float boundX, boundZ;
    private DiceState state = DiceState.Idle;
    private CheckNum checkNum;
    DiceState State
    {
        get => state;
        set
        {
            Debug.Log($"dice state changed: {state} -> {value}");
            state = value;
        }
    }
    public UnityEvent<int> onDiceSettle;

    public void Start()
    {
        DefaultPos = transform.position;
        var extents = plate.GetComponent<Renderer>().bounds.extents;
        boundX = extents.x - 0.1f;
        boundZ = extents.z - 0.1f;
        checkNum = GetComponent<CheckNum>();
    }

    private void OnMouseDown()
    {
        if (State != DiceState.Idle) return;
        State = DiceState.Dragging;
    }
    
    private void OnMouseUp()
    {
        if (State != DiceState.Dragging) return;
        State = DiceState.Rolling;
    }

    private void OnMouseDrag()
    {
        if (State != DiceState.Dragging) return;
        float planeY = 0.8f;

        Plane plane = new Plane(Vector3.up, Vector3.up * planeY); // ground plane

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!plane.Raycast(ray, out var distance)) return;
        var point = ray.GetPoint(distance); // distance along the ray
        point.x = Mathf.Clamp(point.x, -boundX, boundX);
        point.z = Mathf.Clamp(point.z, -boundZ, boundZ);
        
        var diff = point - transform.position;
        var velocity = diff * 10;
        GetComponent<Rigidbody>().velocity = velocity;
    }

    private void Update()
    {
        if (State != DiceState.Rolling) return;
        if (GetComponent<Rigidbody>().velocity.magnitude < 0.001f && 
            GetComponent<Rigidbody>().angularVelocity.magnitude < 0.001f)
        {
            State = DiceState.Idle;
            var maxFace = checkNum.GetResultNum();
            Debug.Log($"dice face: {maxFace}");
            onDiceSettle.Invoke(maxFace);
        }
    }
}