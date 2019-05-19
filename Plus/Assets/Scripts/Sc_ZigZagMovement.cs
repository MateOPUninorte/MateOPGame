using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_ZigZagMovement : MonoBehaviour
{
    public float MoveSpeed = 0.5f;
    public float frequency = 2f; // Speed of sine movement
    public float magnitude = 1.5f; // Size of sine movement

    private Vector3 axis;
    private Vector3 pos;

    void Start()
    {
        pos = transform.position;
        axis = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        pos += transform.up * Time.deltaTime * MoveSpeed;
        transform.position = pos + axis * Mathf.Sin(Time.time * frequency) * magnitude;
    }
}
