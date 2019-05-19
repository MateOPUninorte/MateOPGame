using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sc_TimerCounter : MonoBehaviour
{
    private Text timerText;
    private float secondsCount;
    private int minuteCount;
    public bool isActive;
    void Start(){
        timerText = GetComponent<Text>();
    }

    void Update(){
        if (isActive) {
            UpdateTimerUI();
        }        
    }


    private void UpdateTimerUI(){
        secondsCount += Time.deltaTime;

        //WriteText();

        if (secondsCount >= 60){
            minuteCount++;
            secondsCount = 0;
        }
    }

    private void WriteText() {
        if (minuteCount <= 9 && (int)secondsCount <= 9)
        {
            timerText.text = "0" + minuteCount + ":" + "0" + (int)secondsCount;
        }
        else if (minuteCount <= 9 && (int)secondsCount > 9)
        {
            timerText.text = "0" + minuteCount + ":" + (int)secondsCount;
        }
        else if (minuteCount > 9 && (int)secondsCount <= 9)
        {
            timerText.text = minuteCount + ":" + "0" + (int)secondsCount;
        }
        else
        {
            timerText.text = minuteCount + ":" + (int)secondsCount;
        }
    }

    public int GetMinutes() {
        return minuteCount;
    }

    public float GetSeconds(){
        return secondsCount;
    }

    public bool getActive(){
        return isActive;
    }

    public void SetText(Text text) {
        timerText = text;
        WriteText();
    }
    public void SetActive(bool state) {
        isActive = state;
    }

    public void SetSeconds(float seg) {
        secondsCount = seg;
    }

    public void SetMinutes(int min){
        minuteCount = min;
    }


}
