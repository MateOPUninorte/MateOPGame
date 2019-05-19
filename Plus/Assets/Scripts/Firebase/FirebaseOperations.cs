using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using System;
using System.Threading.Tasks;
//using Google;

public class FirebaseOperations
{
    private FirebaseApp appService;
    private FirebaseAuth authService;
    private FirebaseDatabase dbService;
    private DatabaseReference rootReference;
    private FirebaseUser user;
    private Mdl_User userData;
    private Credential credential;
    private List<Mdl_Exercise> exercisesList;
    //private string authO2ID = "216585056772-hjvn5nb0iroksapc6dulp9eno73nsv87.apps.googleusercontent.com";
    private const string FIREBASE_CONNECTION_STRING = "https://mateop-a3f96.firebaseio.com/";
    private int loCount;
    private string sessionID;

    
    public FirebaseOperations()
    {
        exercisesList = new List<Mdl_Exercise>();
        InitializeFirebase();
    }

    /***************************/
    /***** Init operations *****/
    /***************************/
    void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith((task) => {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                appService = FirebaseApp.DefaultInstance;
                //Editor only
                appService.SetEditorDatabaseUrl(FIREBASE_CONNECTION_STRING);
                authService = FirebaseAuth.DefaultInstance;
                dbService = FirebaseDatabase.DefaultInstance;
                rootReference = dbService.RootReference;
                //AssignUserIDForSession();
                CountLearningObjectives();
                authService.StateChanged += AuthStateChanged;
                AuthStateChanged(this, null);
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                "Could not resolve all Firebase dependencies: {0}", status));
            }
        });
        
    }

    public string AssignUserIDForSession()
    {
        sessionID = rootReference.Push().Key;
        return sessionID;
    }

    public string GetUserID()
    {
        return sessionID;
    }

    /***************************/
    /***** Auth operations *****/
    /***************************/
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (authService.CurrentUser != user){
            bool signedIn = user != authService.CurrentUser && authService.CurrentUser != null;
            if (!signedIn && user != null){
                //Log out
            }
            user = authService.CurrentUser;
            if (signedIn){
                GetCurrentUserDataFromDatabase();
            }
        }
    }

    public List<bool> CreateAccount(string correo,  string password){
        List<bool> taskCompleted = new List<bool>();
        authService.CreateUserWithEmailAndPasswordAsync(correo, password).ContinueWith((task) =>{
            if (task.IsCanceled){
                Debug.LogError("SignUpWithEmailAndPasswordAsync was canceled.");
                taskCompleted.Add(false);
                return null;
            }
            if (task.IsFaulted){
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                taskCompleted.Add(false);
                return null;
            }
            taskCompleted.Add(true);
            return task.Result;
        });
        //CreateUserReference(nombres, correo, userName, password, escuela, edad, grado, genero);
        return taskCompleted;
    }


    public void UpdateUserProfile(string username) {
        FirebaseUser user = authService.CurrentUser;
        if (user != null){
            UserProfile profile = new UserProfile{DisplayName = username};
            user.UpdateUserProfileAsync(profile).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("UpdateUserProfileAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("UpdateUserProfileAsync encountered an error: " + task.Exception);
                    return;
                }

                Debug.Log("User profile updated successfully.");
            });
        }
    }


    public async Task<FirebaseUser> SignIn(string email, string password){
        //email=email+"@correo.com";
        FirebaseUser x= await authService.SignInWithEmailAndPasswordAsync(email, password).ContinueWith((task) => {
            if (task.IsCanceled){
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return null;
            }
            if (task.IsFaulted){
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return null;
            }
            Debug.Log("Successfully Logged In");
            return task.Result;
        });
        return x;
    }



    public FirebaseUser GetCurrentUser() {
        return user;
    }

    public Mdl_User GetCurrentUserData()
    {
        return userData;
    }


    public void signInWithGoogle()
    {
       // Google
        //credential = GoogleAuthProvider.GetCredential(clientID,)
        //authService.SignInWithCredentialAsync
    }

    public void SignOut()
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

    public List<bool> CreateUserReference(string nombres, string correo, string userName, string escuela,int edad, int grado, int genero,int estrellas, int nivel, int sesion, int tipoEscuela){
        List<bool> taskResult = new List<bool>();
        string userID = authService.CurrentUser.UserId;
        Mdl_User newUser = new Mdl_User(userID, nombres, correo, userName.ToLower(), escuela, edad, grado, genero,estrellas,nivel,sesion,tipoEscuela);
        this.userData = newUser;
        string jsonifiedUser = JsonUtility.ToJson(newUser);
        DatabaseReference newUserReference = rootReference.Child("Usuarios").Child(userID);
        newUserReference.SetRawJsonValueAsync(jsonifiedUser).ContinueWith((task) =>{
                if (task.IsCanceled){
                    taskResult.Add(false);
                    Debug.LogError("Unsuccessfully created user on db");                    
                }else if (task.IsCompleted){
                    taskResult.Add(false);
                    Debug.Log("Successfully created user on db");
                }
            taskResult.Add(true);
            return task;
        });
        return taskResult;
    }

    public void UpdateUserSesion(int sesion){
        string userID = authService.CurrentUser.UserId;
        userData.sesion = sesion;
        DatabaseReference newUserReference = rootReference.Child("Usuarios").Child(userID).Child("sesion");
        try
        {
            newUserReference.SetValueAsync(sesion).ContinueWith((task) => {
                if (task.IsCanceled)
                {
                    Debug.LogError("Unsuccessfull sesion update");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Successfull sesion update");
                }
            });
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void UpdateUserJsonPredict(string jsonPredict){
        string userID = authService.CurrentUser.UserId;        
        DatabaseReference newUserReference = rootReference.Child("Usuarios").Child(userID).Child("jsonPredict").Child("Sesion"+userData.sesion);
        try
        {
            newUserReference.SetValueAsync(jsonPredict).ContinueWith((task) => {
                if (task.IsCanceled)
                {
                    Debug.LogError("Unsuccessfull jsonPredict update");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Successfull jsonPredict update");
                }
            });
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public List<string> GetUserJsonPredict(){
        string userID = authService.CurrentUser.UserId;
        List<string> result = new List<string>();
        DatabaseReference newUserReference = rootReference.Child("Usuarios").Child(userID).Child("jsonPredict").Child("Sesion" + userData.sesion);
            newUserReference.GetValueAsync().ContinueWith((task) => {
                if (task.IsCanceled){
                    Debug.LogError("Unsuccessfull jsonPredict update");
                }else if (task.IsCompleted){
                    DataSnapshot snapshot = task.Result;
                    //IDictionary dbData = (IDictionary)snapshot.Value;
                    result.Add(snapshot.Value.ToString());
                    Debug.Log("Successfull jsonPredict update");
                }
            });
        return result;
    }


    public void UpdateUserLevel(int level){
        string userID = authService.CurrentUser.UserId;
        userData.nivel = level;
        DatabaseReference newUserReference = rootReference.Child("Usuarios").Child(userID).Child("nivel");
        try
        {
            newUserReference.SetValueAsync(level).ContinueWith((task) => {
                if (task.IsCanceled)
                {
                    Debug.LogError("Unsuccessfull level update");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Successfull level update");
                }
            });
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void UpdateUserStars(int stars){
        string userID = authService.CurrentUser.UserId;
        userData.estrellas = stars;
        DatabaseReference newUserReference = rootReference.Child("Usuarios").Child(userID).Child("estrellas");
        try
        {
            newUserReference.SetValueAsync(stars).ContinueWith((task) => {
                if (task.IsCanceled)
                {
                    Debug.LogError("Unsuccessfull stars update");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Successfull stars update");
                }
            });
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private void CreateObjectivesReference(string nombre, string descripcion){
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
        currentUser.lo = newIntensity;
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
                Debug.LogError(e.Message);
            }
        }
        Debug.Log("Pretest-Ejercicios Cargados");
        return exercs;
    }

    public List<Mdl_College> ReturnCollegesNames(){
        List<Mdl_College> colleges = new List<Mdl_College>();
        DatabaseReference dataBaseReference = rootReference.Child("Escuelas");
        dataBaseReference.GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted){
                Debug.LogError("Error al obtener las escuelas");
            }else if (task.IsCompleted){
                DataSnapshot snapshot = task.Result;
                Dictionary<string, object> collegesData = (Dictionary<string, object>)snapshot.Value;
                foreach (var item in collegesData){
                    IDictionary dbData = (IDictionary)item.Value;
                    string escuelaId = dbData["escuelaID"] + "";
                    string nombre = dbData["nombre"] + "";
                    int tipo = int.Parse(dbData["tipo"] + "");
                    Mdl_College college = new Mdl_College(escuelaId, nombre, tipo);
                    colleges.Add(college);                    
                }
                Debug.Log("Escuelas Cargadas");
            }
        });

        return colleges;
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
    public void CreateSurveyAnswerLog(string userID, int edad, int genero, int grado, string escuela, string problema, string respuesta, string respuestaJ, float tiempo, int loID, int tipo, int answerChangedCount, bool output)
    {
        DatabaseReference surveyAnswerReference;
        if (output) {
            surveyAnswerReference = rootReference.Child("Historial");
        }else {
            surveyAnswerReference = rootReference.Child("SurveyCollection5");
        }
       
        string pushID = surveyAnswerReference.Push().Key;
        Mdl_Survey surveyAnswer = new Mdl_Survey(userID, edad, genero, grado, escuela, problema, respuesta, respuestaJ, tiempo, loID, tipo, pushID, answerChangedCount);
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

    public void SubmitQuestionToDatabase(int loID, string question, int ans, int type){
        DatabaseReference baseQuestionsRefernece = rootReference.Child("PreguntasBase");
        string pushID = baseQuestionsRefernece.Push().Key;
        Mdl_Exercise exercise = new Mdl_Exercise(loID, question, ans + "", type);
        string jsonifiedExercise = JsonUtility.ToJson(exercise);
        Debug.Log(jsonifiedExercise);
        baseQuestionsRefernece.Child(pushID).SetRawJsonValueAsync(jsonifiedExercise).ContinueWith((task) => {
            if (task.IsCompleted){
                Debug.Log("Question " + question + " was successfully submited");
            }
        });
    }

    public void GetCurrentUserDataFromDatabase(){
        string userID = authService.CurrentUser.UserId;
        DatabaseReference currentUserReference = rootReference.Child("Usuarios/" + userID);
        currentUserReference.GetValueAsync().ContinueWith((task) =>{
            if (task.IsCompleted){
                DataSnapshot dataSnapshot = task.Result;
                IDictionary dictUser = (IDictionary)dataSnapshot.Value;
                try{
                    userData = new Mdl_User(
                        dictUser["userID"]+"",
                        dictUser["nombres"]+"",
                        dictUser["correo"]+"",
                        dictUser["userName"]+"",
                        dictUser["escuela"]+"",
                        int.Parse(dictUser["edad"]+""),
                        int.Parse(dictUser["grado"]+""),
                        int.Parse(dictUser["genero"]+""),
                        int.Parse(dictUser["estrellas"] + ""),
                        int.Parse(dictUser["nivel"] + ""),
                        int.Parse(dictUser["sesion"] + ""),
                        int.Parse(dictUser["tipoEscuela"] + ""));
                }catch(Exception e){
                    Debug.LogError(e.Message);
                }
            }
        });
    }

    public List<Mdl_Exercise> GetExercisesList()
    {
        return this.exercisesList;
    }

    public List<Mdl_UserScoreBoard> GetUsersDataScoreFromDatabase(string endAt, int page, int pageSize){
        DatabaseReference dataBaseReference = rootReference.Child("Usuarios");
        List<Mdl_UserScoreBoard> usersScore = new List<Mdl_UserScoreBoard>();
        
        //.EqualTo(userData.grado, "grado")
            dataBaseReference.OrderByChild("estrellas").LimitToLast(pageSize * page).GetValueAsync().ContinueWith(task => {
                if (task.IsFaulted){
                    Debug.Log("Error al obtener los datos de la tabla de puntuacion");
                }else if (task.IsCompleted){
                    DataSnapshot snapshot = task.Result;
                    bool addItem=true;
                    foreach (var item in snapshot.Children) {
                        IDictionary dictUser = (IDictionary)item.Value;
                        if (addItem) {
                            if (endAt.Equals(dictUser["userID"] + "")) {
                                addItem = false;
                            }
                        }

                        if (addItem) {
                            Mdl_UserScoreBoard userData = new Mdl_UserScoreBoard(
                                dictUser["userID"] + "",
                                dictUser["userName"] + "",
                                int.Parse(dictUser["grado"] + ""),
                                int.Parse(dictUser["genero"] + ""),
                                int.Parse(dictUser["estrellas"] + ""),
                                int.Parse(dictUser["nivel"] + ""));
                            usersScore.Add(userData);
                        }
                    }
                    usersScore.Reverse();
                    Debug.Log("Datos del la tabla de puntuacion");
                }
            });
            
        return usersScore;
    }

    public List<long> GetUsersPagesNumberOfUser(int pageSize)
    {
        DatabaseReference dataBaseReference = rootReference.Child("Usuarios");
        List<long> pagesNumber = new List<long>();
        dataBaseReference.GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted){
                Debug.Log("Error al obtener el numero de paginas");
            }else if (task.IsCompleted){
                if(task.Result.ChildrenCount < pageSize) {
                    pagesNumber.Add(1);
                }else{
                    pagesNumber.Add(task.Result.ChildrenCount / pageSize);
                }
                Debug.Log("Numero de paginas obtenidas ");
            }
        });
        return pagesNumber;
    }

    public List<bool> IsUserNameAvailable(string username) {
        DatabaseReference dataBaseReference = rootReference.Child("Usuarios");
        List<bool> checkResult = new List<bool>();
        //dataBaseReference.OrderByChild("userName").EqualTo(username.ToLower()).GetValueAsync().ContinueWith(task => {
        dataBaseReference.OrderByChild("userName").GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted){
                Debug.Log("Error al validar si el nombre de usuario esta disponible");
            }else if (task.IsCompleted){
                DataSnapshot snapshot = task.Result;
                bool notfound = true;
                foreach(var item in snapshot.Children){
                    IDictionary dictUser = (IDictionary)item.Value;
                    if((dictUser["userName"] + "").Equals(username)) {
                        notfound = false;
                    }
                }
                checkResult.Add(notfound);
                /*
                if (!task.Result.HasChildren){
                    checkResult.Add(true);
                    Debug.Log("Nombre de usuario disponible");
                } else{
                    checkResult.Add(false);
                    Debug.Log("Nombre de usuario NO disponible");
                }*/

            }
        });
        return checkResult;
    }

    public List<string> GetEmailByUserNameAvailable(string username) {
        DatabaseReference dataBaseReference = rootReference.Child("Usuarios");
        List<string> checkResult = new List<string>();
        dataBaseReference.OrderByChild("userName").EqualTo(username.ToLower()).GetValueAsync().ContinueWith(task => {
            if(task.IsFaulted){
                Debug.Log("Error al obtener el correo con nombre de usuario");
            }else if (task.IsCompleted){
                if(!task.Result.HasChildren){
                    checkResult.Add("404");
                    Debug.Log("Correo NO encontrado con nombre de usuario");
                }else{
                    DataSnapshot snapshot = task.Result;                    
                    foreach (var item in snapshot.Children) {
                        IDictionary dictUser = (IDictionary)item.Value;
                        checkResult.Add(dictUser["correo"] + "");
                    }
                    Debug.Log("Correo encontrado con nombre de usuario");
                }
                
            }
        });
        return checkResult;
    }


    public void SetExerciseGeneratorURL() {
        DatabaseReference dataBaseReference = rootReference.Child("ModelAppUrl");
        dataBaseReference.GetValueAsync().ContinueWith(task => {
            if(task.IsFaulted){
                Debug.Log("Error al buscar la url");
            }else if (task.IsCompleted){
                DataSnapshot snapshot = task.Result;
                IDictionary dictUser = (IDictionary)snapshot.Value;
                Sc_ExerciseGeneratorController.MethodModelOne = dictUser["ModelOne"] + "";
                Sc_ExerciseGeneratorController.MethodModelTwo = dictUser["ModelTwo"] + "";
                Sc_ExerciseGeneratorController.UrlARN = dictUser["Url"] + "";
                Debug.Log("Url de moleos establecida");
            }
        });
    }

    

}
