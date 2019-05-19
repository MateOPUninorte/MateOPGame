using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
//using Google;

public class FirebaseOperations
{
    private FirebaseApp appService;
    private FirebaseAuth authService;
    private FirebaseDatabase dbService;
    private DatabaseReference rootReference;
    private FirebaseUser user;
    private Credential credential;
    private List<Mdl_Exercise> exercisesList;
    private string authO2ID = "216585056772-hjvn5nb0iroksapc6dulp9eno73nsv87.apps.googleusercontent.com";
    private const string FIREBASE_CONNECTION_STRING = "https://pfsumas-9ffd6.firebaseio.com/";
    private int loCount;

    
    public FirebaseOperations()
    {
        InitializeFirebase();
    }

    /***************************/
    /***** Init operations *****/
    /***************************/
    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith((task) =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                appService = FirebaseApp.DefaultInstance;
                appService.SetEditorDatabaseUrl(FIREBASE_CONNECTION_STRING);
                authService = FirebaseAuth.DefaultInstance;
                dbService = FirebaseDatabase.DefaultInstance;
                rootReference = dbService.RootReference;
                CountLearningObjectives();
                authService.StateChanged += AuthStateChanged;
                AuthStateChanged(this, null);
            }
            else
            {
                //output error
            }
        });
        
    }


    /***************************/
    /***** Auth operations *****/
    /***************************/
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (authService.CurrentUser != user)
        {
            bool signedIn = user != authService.CurrentUser && authService.CurrentUser != null;
            if (!signedIn && user != null)
            {
                //Log out
            }
            user = authService.CurrentUser;
            if (signedIn)
            {
                //Log in
            }
        }
    }

    public bool CreateAccount(string nombres, string apellidos, string correo, string password, string escuela, int edad, int grado, int genero)
    {
        authService.CreateUserWithEmailAndPasswordAsync(correo, password).ContinueWith((task) =>
        {
            if (task.IsCanceled)
            {
                //mensaje
                return false;
            }
            if (task.IsFaulted)
            {
                //mensaje
                return false;
            }
            CreateUserReference(nombres, apellidos, correo, password, escuela, edad, grado, genero);
            return true;
        });
        return true;
    }

    public void SignIn(string email, string password)
    {
        authService.SignInWithEmailAndPasswordAsync(email, password).ContinueWith((task) => 
        {
            if (task.IsCanceled)
            {
                //mensaje
                return;
            }
            if (task.IsFaulted)
            {
                //mensaje
                return;
            }
            //Login listo
        });
    }

    public void signInWithGoogle()
    {
       // Google
        //credential = GoogleAuthProvider.GetCredential(clientID,)
        //authService.SignInWithCredentialAsync
    }

    public void signOut()
    {
        authService.SignOut();
    }

    /********************************/
    /***** Database operations ******/
    /********************************/

    public void CountLearningObjectives()
    {
        DatabaseReference loReference = rootReference.Child("Objetivos");
        loReference.GetValueAsync().ContinueWith((task) => 
        {
            if (task.IsCompleted)
            {
                loCount = 0;
                DataSnapshot dataSnapshot = task.Result;
                loCount = (int) dataSnapshot.ChildrenCount;
            }
        });
    }

    public void CreateUserReference(string nombres, string apellidos, string correo, string password, string escuela,
        int edad, int grado, int genero)
    {
        string userID = authService.CurrentUser.UserId;
        List<long> LO = new List<long>();
        for (int i = 0; i < loCount; i++)
        {
            LO.Add(0);
        }
        Mdl_User newUser = new Mdl_User(userID, nombres, apellidos, correo, password, escuela, edad, grado, genero, LO);
        string jsonifiedUser = JsonUtility.ToJson(newUser);
        Debug.Log(jsonifiedUser);
        DatabaseReference newUserReference = rootReference.Child("Usuarios").Child(userID);
        try
        {
            newUserReference.SetRawJsonValueAsync(jsonifiedUser).ContinueWith((task) =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("NO");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Successfully created user on db");
                }

                //Que continúe a la siguiente vista
            });
        }
        catch(System.Exception e)
        {
            Debug.Log(e.Message);
        }
        
    }

    private void CreateObjectivesReference(string nombre, string descripcion)
    {
        DatabaseReference newObjectiveReference = rootReference.Child("Objetivos");
        string pushKey = newObjectiveReference.Push().Key;
        Mdl_Objective newObjective = new Mdl_Objective(pushKey, nombre, descripcion);
        string jsonifiedObjective = JsonUtility.ToJson(newObjective);
        newObjectiveReference.Child(pushKey).SetRawJsonValueAsync(jsonifiedObjective).ContinueWith((task) => 
        {
            if(task.IsCanceled || task.IsFaulted)
            {
                //Mensaje de error
            }
            else if(task.IsCompleted)
            {
                Debug.Log("Successfully written on db");
                loCount++;
            }
        });
    }

    public void UpdateLOIntensityFromUser(Mdl_User currentUser, List<long> newIntensity)
    {
        currentUser.LO = newIntensity;
        string jsonifiedUser = JsonUtility.ToJson(currentUser);
        DatabaseReference currentUserReference = rootReference.Child("Usuarios").Child(currentUser.userID);
        currentUserReference.SetRawJsonValueAsync(jsonifiedUser).ContinueWith((task) =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Successfully written on db");
            }
        });
    }

    public void CreateProblemSolvedLog(string loID, string problema, int respuesta1, int respuesta2, int tiempo)
    {
//        string userID = authService.CurrentUser.UserId;
        string userID = "TESTID";
        DatabaseReference newProblemLogReference = rootReference.Child("Historial").Child(userID);
        string pushID = newProblemLogReference.Push().Key;
        Mdl_Historial newLog = new Mdl_Historial(loID, problema, respuesta1, respuesta2, tiempo, userID, pushID);
        string jsonifiedLog = JsonUtility.ToJson(newLog);
        newProblemLogReference.Child(pushID).SetRawJsonValueAsync(jsonifiedLog).ContinueWith((task) =>
        {
            if (task.IsCanceled)
            {
                //Error message 1
            }
            if (task.IsFaulted)
            {
                //Error message 2 
            }
            if (task.IsCompleted)
            {
                Debug.Log("Successfully written on db");
            }
        });

    }

    private void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        Dictionary<string, object> data = (Dictionary<string,object>)args.Snapshot.Value;
        exercisesList = ReturnQuestionsToUser(data);
        // Do something with the data in args.Snapshot
    }

    public void RetrieveQuestionsFromDatabase()
    {
        DatabaseReference baseQuestionsReference = rootReference.Child("PreguntasBase");
        baseQuestionsReference.ValueChanged += HandleValueChanged;
    }

    private List<Mdl_Exercise> ReturnQuestionsToUser(Dictionary<string, object> data)
    {
        List<Mdl_Exercise> exercs = new List<Mdl_Exercise>();
        foreach (var item in data)
        {
            try
            {
                IDictionary dbData = (IDictionary)item.Value;
                int loID = Convert.ToInt32(dbData["loID"]);
                string question = dbData["problem"] + "";
                string answer = dbData["answer"] + "";
                int type = Convert.ToInt32(dbData["tipo"]);
                Mdl_Exercise exerciseI = new Mdl_Exercise(loID, question, answer,type);
                exercs.Add(exerciseI);
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        }
        return exercs;
    }


    /// <summary>
    /// Función para enviar datos de la encuesta a la base de datos
    /// </summary>
    /// <param name="edad">Edad</param>
    /// <param name="genero">Genero: 0 (hombre) y 1 (mujer)</param>
    /// <param name="grado">Grado (0 a 11) siendo 0 kinder</param>
    /// <param name="escuela">Colegio</param>
    /// <param name="problema">Ejercicio de la forma a+b</param>
    /// <param name="respuesta">Respuesta del ejercicio</param>
    /// <param name="respuestaJ">Respuesta del jugador</param>
    /// <param name="tiempo">Tiempo tomado para contestar la pregunta</param>
    /// <param name="loID">ID del objetivo de aprendizaje asociado a la pregunta</param>
    /// <param name="tipo">Tipo de pregunta por como se mostró (0: abierta y 1 cerrada)</param>
    public void CreateSurveyAnswerLog(int edad, int genero, int grado, string escuela, string problema, string respuesta, string respuestaJ, float tiempo, int loID, int tipo)
    {
        DatabaseReference surveyAnswerReference = rootReference.Child("SurveyCollection");
        string pushID = surveyAnswerReference.Push().Key;
        Mdl_Survey surveyAnswer = new Mdl_Survey(edad, genero, grado, escuela, problema, respuesta, respuestaJ, tiempo, loID, tipo, pushID);
        string jsonifiedSurveyAnswer = JsonUtility.ToJson(surveyAnswer);
        surveyAnswerReference.Child(pushID).SetRawJsonValueAsync(jsonifiedSurveyAnswer).ContinueWith((task) =>
        {
            if (task.IsCanceled)
            {
                //Cancel
            }else if (task.IsFaulted)
            {
                //Faulted
            }else if (task.IsCompleted)
            {
                Debug.Log("Success!");
            }
        });
    }

    public void SubmitQuestionToDatabase(int loID, string question, int ans, int type)
    {
        DatabaseReference baseQuestionsRefernece = rootReference.Child("PreguntasBase");
        string pushID = baseQuestionsRefernece.Push().Key;
        Mdl_Exercise exercise = new Mdl_Exercise(loID, question, ans + "", type);
        string jsonifiedExercise = JsonUtility.ToJson(exercise);
        Debug.Log(jsonifiedExercise);
        baseQuestionsRefernece.Child(pushID).SetRawJsonValueAsync(jsonifiedExercise).ContinueWith((task) =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Question " + question + " was successfully submited");
            }
        });
    }

    public void GetCurrentUserDataFromDatabase()
    {
        //string userID = authService.CurrentUser.UserId;
        string userID = "HvMq7UMBHKThaE9l5H2zflMlXtp1";
        DatabaseReference currentUserReference = rootReference.Child("Usuarios/" + userID);
        Mdl_User currentUser = null;
        currentUserReference.GetValueAsync().ContinueWith((task) =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot dataSnapshot = task.Result;
                IDictionary dictUser = (IDictionary)dataSnapshot.Value;
                try
                {
                    //System.Convert.ChangeType(dictUser["lo"])
                    //string a = dictUser["LO"];
                    Debug.Log((int[])dictUser["lo"]);
                    Debug.Log("me cago en tu puta madre, unity");
                        /*currentUser = new Mdl_User(
                        dictUser["userID"].ToString(),
                        dictUser["nombres"].ToString(),
                        dictUser["apellidos"].ToString(),
                        dictUser["correo"].ToString(),
                        dictUser["password"].ToString(),
                        dictUser["escuela"].ToString(),
                        (int)dictUser["edad"],
                        (int)dictUser["grado"],
                        (int)dictUser["genero"],
                        dictUser["LO"]);*/
                }
                catch(System.Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
        });
    }

    public List<Mdl_Exercise> GetExercisesList()
    {
        return this.exercisesList;
    }

}
