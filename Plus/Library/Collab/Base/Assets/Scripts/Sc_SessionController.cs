using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Sc_SessionController : MonoBehaviour
{
    public InputField EmailSignIn;
    public InputField PasswordSignIn;


    public InputField EmailSignUp;
    public InputField PasswordSignUp;
    public InputField NamesSignUp;
    public InputField LastnamesSignUp;
    public InputField AgeSignUp;
    public Dropdown GenderSignUp;
    public Dropdown GradeSignUp;
    public InputField SchoolSignUp;

    private FirebaseOperations DBOperation;

    void Start() {
        DBOperation = new FirebaseOperations();
    }

    public void OnSignInButtonPress() {
        //Debug.Log(CorreoSignIn.text);
        //Debug.Log(PasswordSignIn.text);
        if(DBOperation.GetExercisesList() != null)
        {
            Debug.Log(DBOperation.GetExercisesList().Count);
        }
        DBOperation.RetrieveQuestionsFromDatabase();
        //DBOperation.SignIn(CorreoSignUp.text, PasswordSignUp.text);
    }

    public void OnRegisterButtonPress()
    {
        int grado = GradeSignUp.value + 1;
        int gender = GenderSignUp.value;
        if (DBOperation.CreateAccount( NamesSignUp.text, LastnamesSignUp.text, EmailSignUp.text, PasswordSignUp.text, SchoolSignUp.text, int.Parse(AgeSignUp.text), grado, gender)){       
            SceneManager.LoadScene("Menu");
        }

    }
}
