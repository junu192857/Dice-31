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
    public int maxFace;
    public bool alreadyRolled;

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

    public void ResetDice()
    {
        maxFace = 0;
        alreadyRolled = false;
    }

    public void Start()
    {
        //plate 할당하는거 나중에 바꾸든가 해야지..
        plate = GameObject.Find("plate");
        DefaultPos = transform.position;
        var extents = plate.GetComponent<Renderer>().bounds.extents;
        boundX = extents.x - 0.3f;
        boundZ = extents.z - 0.2f;
        checkNum = GetComponent<CheckNum>();
        ResetDice();
    }


    private void OnMouseDown()
    {
        if (GameManager.Inst.gsm.State == GameState.DiceRolling && !alreadyRolled)
        {
            if (State != DiceState.Idle) return;
            State = DiceState.Dragging;
        }
    }

    private void OnMouseUp()
    {
        if (GameManager.Inst.gsm.State == GameState.DiceRolling && !alreadyRolled)
        {
            if (State != DiceState.Dragging) return;
            var rigidbody = GetComponent<Rigidbody>();
            rigidbody.velocity += Vector3.up * 6;
            rigidbody.angularVelocity += Random.onUnitSphere * 15;
            // transform.rotation = Random.rotation;
            State = DiceState.Rolling;
        }
    }

    private void OnMouseDrag()
    {
        if (GameManager.Inst.gsm.State == GameState.DiceRolling && !alreadyRolled)
        {
            if (State != DiceState.Dragging) return;
            float planeY = 1.2f;

            Plane plane = new Plane(Vector3.up, Vector3.up * planeY); // ground plane

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!plane.Raycast(ray, out var distance)) return;
            var point = ray.GetPoint(distance); // distance along the ray
            var position = plate.transform.position;
            point.x = Mathf.Clamp(point.x, position.x - boundX, position.x + boundX);
            point.z = Mathf.Clamp(point.z, position.z - boundZ, position.z + boundZ);

            var diff = point - transform.position;
            var velocity = diff * 10;
            var rigidbody = GetComponent<Rigidbody>();
            var oldVelocity = rigidbody.velocity;
            rigidbody.velocity = velocity;
            var targetAngularVelocity = Vector3.Cross(
                velocity - oldVelocity,
                velocity
            ) * 15;
            var angularVelocity = rigidbody.angularVelocity;
            rigidbody.angularVelocity = Vector3.Lerp(
                angularVelocity,
                targetAngularVelocity,
                0.1f
            );
        }
    }

    public void ChangeStateToRolling()
    {
        State = DiceState.Rolling;
    }

    public bool CheckDiceState()
    {
        return State == DiceState.Idle;
    }

    private void Update()
    {
        if (State != DiceState.Rolling) return;
        if (GetComponent<Rigidbody>().velocity.magnitude < 0.001f &&
            GetComponent<Rigidbody>().angularVelocity.magnitude < 0.001f)
        {
            State = DiceState.Idle;
            maxFace = checkNum.GetResultNum();
            Debug.Log($"dice face: {maxFace}");
            alreadyRolled = true;
            onDiceSettle.Invoke(maxFace);
        }
    }
}