using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_ExerciseManager : MonoBehaviour
{
    List<Mdl_Exercise> allExercises;
    int currentExersise;
    float finalTime;

    public static Sc_ExerciseManager myExerciseManager;

    public List<Mdl_Exercise> AllExercises { get => allExercises; set => allExercises = value; }
    public int CurrentExersise { get => currentExersise; set => currentExersise = value; }
    public float FinalTime { get => finalTime; set => finalTime = value; }


    public void AnswerCurrentExercise(string ans) {
        GetCurrentExersise().playerAnswer = ans;
    }


    public void NextExercise(){
        if (currentExersise+1 < AllExercises.Count) {
            currentExersise++;
        }
    }

    public bool IsTestFinished(){
        if (currentExersise == AllExercises.Count-1) {
            return true;
        }
        return false;
    }

    public Mdl_Exercise GetCurrentExersise(){
        return allExercises[currentExersise];
    }

    public float GetPorcentGoodAnswers() {
        float num, den;
        num = 0;
        den = allExercises.Count;
        foreach (Mdl_Exercise exer in allExercises) {
            if (exer.answer==exer.playerAnswer) {
                num++;
            }
        }
        return (num/den);
    }

    public int GetGoodAnswers() {
        int num;
        num = 0;
        foreach (Mdl_Exercise exer in allExercises){
            if (exer.answer == exer.playerAnswer){
                num++;
            }
        }
        return num;
    }




}
