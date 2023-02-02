using System;
using System.Collections;
using System.Collections.Generic;
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

    private Vector3 GetMousePosition()
    {
        return Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseDown()
    {
        var rigidBody = GetComponent<Rigidbody>();
        rigidBody.useGravity = false;
        defaultDrag = rigidBody.drag;
        rigidBody.drag = 2;
        updateTorqueCoroutine = StartCoroutine(UpdateTorque());
    }

    private IEnumerator UpdateTorque()
    {
        while (true)
        {
            torque = Random.insideUnitSphere * 3.0f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnMouseUp()
    {
        var rigidBody = GetComponent<Rigidbody>();
        rigidBody.useGravity = true;
        rigidBody.drag = defaultDrag;
        StopCoroutine(updateTorqueCoroutine);
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
        if (diff.magnitude > 0.1f)
        {
            var force = diff.normalized * Mathf.Clamp(diff.magnitude, 0, 3f);
            GetComponent<Rigidbody>().AddForce(force);
        }
        
        //transform.position = new Vector3(transform.position.x, 0.5f, transform.position.y);

        // GetComponent<Rigidbody>().AddTorque(torque);
    }

    // only if dice collision.transform.tag == "plate" is true can roll dice
    // When dice roll end, If come back to default position
    // When dragging, it can't go through the table or outside screen and table
}