using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    Vector3 mousePosition;
    public Vector3 DefaultPos;

    public void Start()
    {
        DefaultPos = transform.position;
    }

    private Vector3 GetMousePosition()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }
    private void OnMouseDown()
    {
        mousePosition = Input.mousePosition - GetMousePosition();
    }
    private void OnMouseDrag()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
    }

    // only if dice collision.transform.tag == "plate" is true can roll dice
    // When dice roll end, If come back to default position
    // When dragging, it can't go through the table

}