using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class DiceController : MonoBehaviour
{
    public Vector3 DefaultPos;
    public GameObject plate;
    private float boundX, boundZ;
    private float defaultDrag;
    private Vector3 torque;
    private Coroutine updateTorqueCoroutine;

    public void Start()
    {
        DefaultPos = transform.position;
        var extents = plate.GetComponent<Renderer>().bounds.extents;
        boundX = extents.x - 0.1f;
        boundZ = extents.z - 0.1f;
    }

    private void OnMouseDrag()
    {
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

    // only if dice collision.transform.tag == "plate" is true can roll dice
    // When dice roll end, If come back to default position
    // When dragging, it can't go through the table or outside screen and table
}