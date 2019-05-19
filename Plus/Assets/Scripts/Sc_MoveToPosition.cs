using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_MoveToPosition : MonoBehaviour
{
    public Vector2 target;
    public float speed;
    public bool canMove;
    public float sumy;
    public float sumx;
    private RectTransform rectTransform;
    void Start(){
        speed = 100;
        Initialize();
    }

    public void Initialize() {
        rectTransform = GetComponent<RectTransform>();
        target = new Vector2(rectTransform.anchoredPosition.x+sumx, rectTransform.anchoredPosition.y+sumy);
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove) {
            float step = speed * Time.deltaTime;
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, target,step);
        }

        if (rectTransform.anchoredPosition == target && canMove){
            canMove = false;
        }    
    }
}
