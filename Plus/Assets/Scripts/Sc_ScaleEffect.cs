using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_ScaleEffect : MonoBehaviour
{
    public float maxSize;
    public float growFactor;
    public float waitTime;

    public bool notReapit;
    private bool state;
    IEnumerator Scale()
    {
        float timer = 0;

        while (state){
            // we scale all axis, so they will have the same value, 
            // so we can work with a float instead of comparing vectors
            while (maxSize > transform.localScale.x)
            {
                timer += Time.deltaTime;
                if (transform.localScale.x<1) {
                    transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor*4;
                }
                else {
                    transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
                }
                
                yield return null;
            }
            // reset the timer

            yield return new WaitForSeconds(waitTime);

            timer = 0;
            while (1 < transform.localScale.x)
            {
                timer += Time.deltaTime;
                transform.localScale -= new Vector3(1, 1, 1) * Time.deltaTime * growFactor;
                yield return null;
            }

            timer = 0;
            yield return new WaitForSeconds(waitTime);
            if (notReapit) {
                state = false;
            }
        }
    }

    void Start() {
        state = true;
    }

    public void StartEffect() {
        state = true;
        StartCoroutine(Scale());
    }

    void OnEnable(){
        state = true;
        StartCoroutine(Scale());
    }
    void OnDisable(){
        state = false;
        StopCoroutine(Scale());
    }
}
