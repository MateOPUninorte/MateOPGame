using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_RepeatingBackground : MonoBehaviour
{
    private BoxCollider2D groundCollider;       
    private float groundHorizontalLength;      
    private float speedSign;
    private float y;
    private float z;
    private void Awake()
    {
        y = transform.position.y;
        z = transform.position.z;
        groundCollider = GetComponent<BoxCollider2D>();
        groundHorizontalLength = groundCollider.size.x*transform.localScale.x;
        speedSign = Mathf.Sign(GetComponent<Sc_ScrollingObject>().bkgScrollSpeed);
    }


    private void Update()
    {
        if (speedSign>0) {
            if (transform.position.x > groundHorizontalLength)
            {
                RepositionBackground();
            }
        }
        else {
            if (transform.position.x < -groundHorizontalLength)
            {
                RepositionBackground();
            }
        }

    }

    private void RepositionBackground()
    {
        Vector3 groundOffSet = new Vector3((-1*speedSign)*groundHorizontalLength * 2f, 0,0);
        transform.position = transform.position + groundOffSet;
    }
}
