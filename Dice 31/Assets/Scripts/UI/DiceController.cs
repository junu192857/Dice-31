using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceController : MonoBehaviour
{
    Vector3 mousePosition;
    public Vector3 DefaultPos;
    public GameObject Table;
    private Vector3 TablePos;

    public void Start()
    {
        DefaultPos = transform.position;
        TablePos = Table.transform.position;
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
        Vector3 objectPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition - mousePosition);
        if (objectPosition.y >= 0) {
            transform.position = objectPosition;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
        
        //transform.position = new Vector3(transform.position.x, 0.5f, transform.position.y);

    }

    // only if dice collision.transform.tag == "plate" is true can roll dice
    // When dice roll end, If come back to default position
    // When dragging, it can't go through the table or outside screen and table

}