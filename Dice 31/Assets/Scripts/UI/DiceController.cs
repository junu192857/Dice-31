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

    public void ResetDice() {
        
        maxFace = 0;
        alreadyRolled = false;
    }
    public void Start()
    {
        //plate 할당하는거 나중에 바꾸든가 해야지..
        plate = GameObject.Find("plate");
        DefaultPos = transform.position;
        var extents = plate.GetComponent<Renderer>().bounds.extents;
        boundX = extents.x - 0.1f;
        boundZ = extents.z - 0.1f;
        checkNum = GetComponent<CheckNum>();
        ResetDice();
    }


    //TODO: 같은 턴에서 한번 굴린 주사위는 다시 못 굴리게 하기
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
            transform.rotation = Random.rotation;
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
            point.x = Mathf.Clamp(point.x, -boundX, boundX);
            point.z = Mathf.Clamp(point.z, -boundZ, boundZ);

            var diff = point - transform.position;
            var velocity = diff * 10;
            GetComponent<Rigidbody>().velocity = velocity;
        }
    }

    public void ChangeStateToRolling() {
        State = DiceState.Rolling;
    }
    public bool CheckDiceState() {
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