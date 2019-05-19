using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Sc_GameQuickSessionController : MonoBehaviour
{
    public GameObject[] contanier;
    public InputField AgeSign;
    public Dropdown GenderSign;
    public Dropdown GradeSign;
    public InputField SchoolSign;
    public Text buttonText;
    public GameObject BackButton;

    private FirebaseOperations firebaseOperations;
    private int indexSelected;

    void Start() {
        firebaseOperations = new FirebaseOperations();
        if (contanier[0] != null) {
            indexSelected = 0;
            contanier[0].SetActive(true);
        } else {
            Debug.Log("Objeto Nulo");
        }
        
    }




    public void OnBackButtonPress(){
        if (indexSelected-1 >= 0) {
            contanier[indexSelected].SetActive(false);
            indexSelected--;
            contanier[indexSelected].SetActive(true);
            if (indexSelected==0) {
                BackButton.SetActive(false);
            }
        }  
    }

    public void OnContinueButtonPress() {
        if (indexSelected + 1 < contanier.Length) {
            contanier[indexSelected].SetActive(false);
            indexSelected++;
            contanier[indexSelected].SetActive(true);
            BackButton.SetActive(true);
        } else if(indexSelected + 1 == contanier.Length){
            firebaseOperations.RetrieveQuestionsFromDatabase();
            BackButton.SetActive(false);
            buttonText.text = "EMPEZAR!";
            contanier[indexSelected].SetActive(false);
            indexSelected++;
        }
        else{
            bool[] myStates = new bool[firebaseOperations.GetExercisesList().Count];
            GameObject myExercisesObject = new GameObject();
            Sc_ExerciseManager myExerciseManager = myExercisesObject.AddComponent<Sc_ExerciseManager>();
            Sc_QuickUserManager myQuickUserManager = myExercisesObject.AddComponent<Sc_QuickUserManager>();

            myQuickUserManager.Edad = int.Parse(AgeSign.text);
            myQuickUserManager.Grado = GradeSign.value+1;
            myQuickUserManager.Genero = GradeSign.value;
            myQuickUserManager.Escuela = SchoolSign.text;
            
            myExerciseManager.AllExercises = firebaseOperations.GetExercisesList(); 
            myExerciseManager.ExercisesSolved = myStates;

            myExercisesObject.name = "ManagerObject";
            myExercisesObject.tag = "Manager";

           SceneManager.LoadScene("NivelSumas");
        }
    }

    public List<Mdl_Exercise> randomExercises() {
        List<Mdl_Exercise> myExercises = new List<Mdl_Exercise>();  
        for (var i = 1; i <= 10; i++)
        {
            int sum1 = Random.Range(0, 10);
            int sum2 = Random.Range(0, 10);
            string op = sum1 + "+" + sum2 + "=?";
            string ans = (sum1 + sum2) + "";
            myExercises.Add(new Mdl_Exercise(op, ans));
        }
        return myExercises;
    }
}
