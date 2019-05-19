using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mdl_Exercise 
{

    public string problem;
    public string answer;
    public string playerAnswer;
    public float time;
    public int answerChangedNumber;
    public int dificultad;
    public int loID;
    public int type;

    public Mdl_Exercise()
    {
    }

    public Mdl_Exercise(string problem, string answer)
    {
        this.problem = problem;
        this.answer = answer;
    }

    public Mdl_Exercise(int loID, string problem, string answer, int type) {
        this.loID = loID;
        this.problem = problem;
        this.answer = answer;
    }




    public int AnswerLength() {
        return answer.Length;
    }


}
