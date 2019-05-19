using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;




public class Sc_GameAdditionController : MonoBehaviour { 

    public GameObject panelExercise;
    public GameObject panelImageHelp;
    public GameObject keyBoard;
    public Text OptionA;
    public Text OptionB;
    public Text OptionC;
    public GameObject OptionAGO;
    public GameObject OptionBGO;
    public GameObject OptionCGO;
    public GameObject keyOptions;
    public Font numberFont;
    public Image ButtonSound;
    public Sprite mutedSound;
    public Sprite unMutedSound;
    public Sprite[] objectsAdd;

    public Sprite plusImage;
    public Sprite equalsImage;
    public Sc_TimerCounter myTimer;

    public ParticleSystem goodSplash;
    public ParticleSystem badSplash;
    public RectTransform loadingMovingBar;
    public RectTransform loadingFixedBar;
    public Image ReadyButton;

    private Mdl_Exercise myExercise;
    private Sc_ExerciseManager myExerciseManager;
    private Sc_QuickUserManager myQUManager;
    private Sc_AdditionTypeManager myAdditionTypeManager;
    private FirebaseOperations firebaseOperations;
    public GameObject AnswersContainer;
    private System.Random myRandom;
    private bool soundPause = true;
    private bool AnswerChanged = true;
    private float lastTime;
    private int lastNumber;
    private int numberAnwersChange;

    void Start() {
        GameObject myManagerObject = GameObject.FindWithTag("Manager");
        if (myManagerObject != null) {
            Sc_FirebaseOperations fop = myManagerObject.GetComponent<Sc_FirebaseOperations>();
            if (fop != null) {
                firebaseOperations = myManagerObject.GetComponent<Sc_FirebaseOperations>().FirebaseOperations;
            }
        } else {
            myManagerObject = new GameObject();
            Sc_FirebaseOperations myFirebaseManager = myManagerObject.AddComponent<Sc_FirebaseOperations>();
            firebaseOperations = new FirebaseOperations();
            myFirebaseManager.FirebaseOperations = firebaseOperations;
            myManagerObject.tag = "Manager";
            myManagerObject.name = "ManagerObject";
            DontDestroyOnLoad(myManagerObject);
        }

        if (soundPause) {
            ButtonSound.sprite = mutedSound;
            this.GetComponent<AudioSource>().Pause();
        }
        myRandom = new System.Random();
        myExerciseManager = myManagerObject.GetComponent<Sc_ExerciseManager>();
        myQUManager = myManagerObject.GetComponent<Sc_QuickUserManager>();
        myAdditionTypeManager = myManagerObject.GetComponent<Sc_AdditionTypeManager>();
        InitializeAnswerType();
        AnswerChanged = true;
        myExercise = myExerciseManager.GetCurrentExersise();
        ShowExercise();
        lastTime = 0;
        myTimer.SetActive(true);
        numberAnwersChange = 0;
    }

    private void InitializeAnswerType() {
        if (myAdditionTypeManager.QuestionsType == QuestionsType.Abiertas) {
            keyBoard.SetActive(true);
            keyOptions.SetActive(false);
        } else if (myAdditionTypeManager.QuestionsType == QuestionsType.Cerradas) {
            keyOptions.SetActive(true);
            keyBoard.SetActive(false);
        }
    }

    private void ShowExercise() {
        if (OptionAGO!=null && OptionBGO!=null && OptionCGO!=null) { 
            OptionAGO.transform.SetParent(AnswersContainer.transform);
            OptionBGO.transform.SetParent(AnswersContainer.transform);
            OptionCGO.transform.SetParent(AnswersContainer.transform);
            OptionAGO.SetActive(true);
            OptionBGO.SetActive(true);
            OptionCGO.SetActive(true);
        }
        DeleteAllChilds(panelExercise);
        DeleteAllChilds(panelImageHelp);
        List<string> op = DivideOperation(myExercise.problem);
        InitializeExercise(op, panelExercise);
        if (panelImageHelp.activeSelf) {
            //InitializeImageHelp(op, panelImageHelp);
        }
        if (myAdditionTypeManager.QuestionsType == QuestionsType.Cerradas) {
            GenerateAnswerOptions(myExercise.answer);
        }
    }

    private void GenerateAnswerOptions(string ans) {
        int Answer = int.Parse(ans);
        int delta = (int)(Answer * 0.2);
        if (delta == 0) {
            delta = 1;
        }
        int extraOption1 = Answer - delta;
        int extraOption2 = Answer + delta;
        int[] allOptions = { Answer, extraOption1, extraOption2 };
        bool[] posTaken = new bool[allOptions.Length];

        int number = lastNumber;
        int a;
        while (number == lastNumber) {
            a = (int)(Random.value * 100);
            if (a >= 60) {
                number = 1;
            } else if (a >= 30 && a < 60) {
                number = 2;
            } else if (a < 30) {
                number = 3;
            }
        }
        lastNumber = number;

        if (lastNumber == 1) {
            OptionA.text = "" + allOptions[0];
            a = (int)(Random.value * 200);
            if (a > 100) {
                OptionC.text = "" + allOptions[2];
                OptionB.text = "" + allOptions[1];
            } else {
                OptionC.text = "" + allOptions[1];
                OptionB.text = "" + allOptions[2];
            }
        } else if (lastNumber == 2) {
            OptionB.text = "" + allOptions[0];
            a = (int)(Random.value * 200);
            if (a >= 100) {
                OptionC.text = "" + allOptions[2];
                OptionA.text = "" + allOptions[1];
            } else {
                OptionC.text = "" + allOptions[1];
                OptionA.text = "" + allOptions[2];
            }
        } else if (lastNumber == 3) {
            OptionC.text = "" + allOptions[0];
            a = (int)(Random.value * 200);
            if (a > 100) {
                OptionB.text = "" + allOptions[1];
                OptionA.text = "" + allOptions[2];
            } else {
                OptionB.text = "" + allOptions[2];
                OptionA.text = "" + allOptions[1];
            }
        }

    }


    int GetRandom(int min, int max) {
        int rand = Random.Range(min, max);
        while (rand == lastNumber)
            rand = Random.Range(min, max);
        lastNumber = rand;
        return rand;
    }

    private void DeleteAllChilds(GameObject gameObject) {
        foreach (Transform child in gameObject.transform) {
            Destroy(child.gameObject);
        }
    }

    private void NextExercise() {
        myExerciseManager.NextExercise();
        myExercise = myExerciseManager.GetCurrentExersise();
    }



    public void OnReadyButtonPress() {
        string answ = GameObject.FindGameObjectWithTag("Answer").GetComponent<Text>().text;
        if (!answ.Equals("?")) {
            ReadyButton.color = new Color(0.78f, 0.78f, 0.78f);
            float time = myTimer.GetMinutes() * 60 + myTimer.GetSeconds() - lastTime;
            myExerciseManager.GetCurrentExersise().playerAnswer = answ;
            myExerciseManager.GetCurrentExersise().time = time;
            lastTime += time;
            Mdl_Exercise e = myExerciseManager.GetCurrentExersise();
            int type;
            if (myAdditionTypeManager.QuestionsType == QuestionsType.Abiertas) {
                type = 0;
            } else {
                type = 1;
            }
            if (GameObject.FindGameObjectWithTag("Manager").GetComponent<Sc_OfflineSesion>()==null) {
                if (myQUManager != null) {
                    CreateSurveyAnswerLogOffline(myExerciseManager.CurrentExersise, myQUManager.Id, myQUManager.Edad, myQUManager.Genero, myQUManager.Grado, myQUManager.Escuela, e.problem, e.answer, e.playerAnswer, e.time, e.loID, type, numberAnwersChange);
                } else {
                    Mdl_User userData = firebaseOperations.GetCurrentUserData();
                    firebaseOperations.CreateSurveyAnswerLog(userData.userID, userData.edad, userData.genero, userData.grado, userData.escuela, e.problem, e.answer, e.playerAnswer, e.time, e.loID, type, numberAnwersChange, true);
                    if (!myExerciseManager.GetCurrentExersise().answer.Equals(myExerciseManager.GetCurrentExersise().playerAnswer)) {
                        Sc_ExerciseGeneratorController.WriteOnFileWrongExercise(myExerciseManager.GetCurrentExersise());
                    }
                }
            }


            if (myExerciseManager.GetCurrentExersise().answer.Equals(myExerciseManager.GetCurrentExersise().playerAnswer)) {
                goodSplash.Play();
                siguientePregunta = true;
            } else {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Sc_CameraShake>().shakeDuration = 0.5f;
            }

            if (myExerciseManager.IsTestFinished()) {
                myTimer.SetActive(false);
                Sc_TimerCounter mytimerManager = GameObject.FindGameObjectWithTag("Manager").AddComponent<Sc_TimerCounter>();
                mytimerManager.SetSeconds(myTimer.GetSeconds());
                mytimerManager.SetMinutes(myTimer.GetMinutes());
                mytimerManager.SetActive(myTimer.getActive());
                StartCoroutine(LoadScore());
            }

            loadingMovingBar.sizeDelta = new Vector2(loadingMovingBar.sizeDelta.x + (int)(loadingFixedBar.sizeDelta.x / myExerciseManager.AllExercises.Count), loadingMovingBar.sizeDelta.y);
            loadingMovingBar.anchoredPosition = new Vector2(loadingMovingBar.anchoredPosition.x + (int)(loadingFixedBar.sizeDelta.x / myExerciseManager.AllExercises.Count) / 2, loadingMovingBar.anchoredPosition.y);


            NextExercise();
            panelExercise.GetComponent<Sc_ScaleEffect>().StartEffect();
            ShowExercise();
            numberAnwersChange = 0;
            OnButtonPressedChangeColor(-1);

        }
    }

    private bool primeraOpcionTomada = false;
    private bool siguientePregunta = false;

    public bool IsAnswerAGoodOne(string nextAnwsere) {
        if (myExerciseManager.GetCurrentExersise().answer.Equals(nextAnwsere)) {
            return true;
        }
        return false;
    }

    public void OnReadyAnswerDrop(GameObject answer) {
        ButtonKeyboardPress(answer.transform.GetChild(0).GetComponent<Text>());
        string answ = GameObject.FindGameObjectWithTag("Answer").GetComponent<Text>().text;
        if (!answ.Equals("?")){
            ReadyButton.color = new Color(0.78f, 0.78f, 0.78f);
            float time = myTimer.GetMinutes()*60 + myTimer.GetSeconds()-lastTime;
            myExerciseManager.GetCurrentExersise().time = time;
            lastTime += time;
            Mdl_Exercise e = myExerciseManager.GetCurrentExersise();
            int type;
            if (myAdditionTypeManager.QuestionsType == QuestionsType.Abiertas) {
                type = 0;
            }else{
                type = 1;
            }
             
            if (!primeraOpcionTomada) {
                myExerciseManager.GetCurrentExersise().playerAnswer = answ;
                if (GameObject.FindGameObjectWithTag("Manager").GetComponent<Sc_OfflineSesion>() == null){
                    if (myQUManager != null) {
                        CreateSurveyAnswerLogOffline(myExerciseManager.CurrentExersise, myQUManager.Id, myQUManager.Edad, myQUManager.Genero, myQUManager.Grado, myQUManager.Escuela, e.problem, e.answer, e.playerAnswer, e.time, e.loID, type, numberAnwersChange);
                    } else {
                        Mdl_User userData = firebaseOperations.GetCurrentUserData();
                        myExerciseManager.GetCurrentExersise().answerChangedNumber = numberAnwersChange;
                        firebaseOperations.CreateSurveyAnswerLog(userData.userID, userData.edad, userData.genero, userData.grado, userData.escuela, e.problem, e.answer, e.playerAnswer, e.time, e.loID, type, numberAnwersChange, true);
                        if(!myExerciseManager.GetCurrentExersise().answer.Equals(myExerciseManager.GetCurrentExersise().playerAnswer)){
                            Sc_ExerciseGeneratorController.WriteOnFileWrongExercise(myExerciseManager.GetCurrentExersise());
                        }
                    }                   
                }
                primeraOpcionTomada = true;
            }

            if (myExerciseManager.GetCurrentExersise().answer.Equals(answer.transform.GetChild(0).GetComponent<Text>().text)) {
                goodSplash.Play();
                siguientePregunta = true;                
            }else{
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Sc_CameraShake>().shakeDuration = 0.5f;
            }
            
            if (siguientePregunta) {
                if (GameObject.FindGameObjectWithTag("AnswerDropContainer") != null){
                    GameObject answerCont = GameObject.FindGameObjectWithTag("AnswerDropContainer");
                    //answerCont.SetActive(false);
                    answerCont.transform.SetParent(keyOptions.transform);
                    answerCont.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                    answerCont.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
                }
                StartCoroutine(YieldForNextQuestion());
            }
        }
    }

    IEnumerator YieldForNextQuestion() { 
        yield return new WaitForSeconds(0.5f);
        if (myExerciseManager.IsTestFinished()){
            myTimer.SetActive(false);
            Sc_TimerCounter mytimerManager = GameObject.FindGameObjectWithTag("Manager").AddComponent<Sc_TimerCounter>();
            mytimerManager.SetSeconds(myTimer.GetSeconds());
            mytimerManager.SetMinutes(myTimer.GetMinutes());
            mytimerManager.SetActive(myTimer.getActive());
            StartCoroutine(LoadScore());
        }

        loadingMovingBar.sizeDelta = new Vector2(loadingMovingBar.sizeDelta.x + (int)(loadingFixedBar.sizeDelta.x / myExerciseManager.AllExercises.Count), loadingMovingBar.sizeDelta.y);
        loadingMovingBar.anchoredPosition = new Vector2(loadingMovingBar.anchoredPosition.x + (int)(loadingFixedBar.sizeDelta.x / myExerciseManager.AllExercises.Count) / 2, loadingMovingBar.anchoredPosition.y);


        NextExercise();
        panelExercise.GetComponent<Sc_ScaleEffect>().StartEffect();
        ShowExercise();
        numberAnwersChange = 0;
        OnButtonPressedChangeColor(-1);



        siguientePregunta = false;
        primeraOpcionTomada = false;
    }



    IEnumerator LoadScore(){
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Score");
    }

    public void CreateSurveyAnswerLogOffline(int count, string userID, int edad, int genero, int grado, string escuela, string problema, string respuesta, string respuestaJ, float tiempo, int loID, int tipo, int answerChangedCount)
    {
        Mdl_Survey surveyAnswer = new Mdl_Survey(userID, edad, genero, grado, escuela, problema, respuesta, respuestaJ, tiempo, loID, tipo, "pushID", answerChangedCount);
        string jsonifiedSurveyAnswer = JsonUtility.ToJson(surveyAnswer);
        TextWriter file;
        if (Application.isEditor){
            file = new StreamWriter(Application.dataPath + "/StudentsData.txt", true);
        }else{
            file = new StreamWriter(Application.persistentDataPath + "/StudentsData.txt", true);
        }

        file.Write(count+"|"+jsonifiedSurveyAnswer+";\n");//El saltp de linea depende si el sistema operativo es o no de uNix tener en cuenta para IOS

        file.Close();
    }

    

    private List<string> DivideOperation(string operation) {
        List<string> myList = new List<string>();
        string elem = "";
        string subi = "";
        for (var i = 0; i < operation.Length; i++) {
            subi = operation.Substring(i,1);
            if (IsCharANumber(subi)){
                elem = elem + subi;
            }else{
                myList.Add(elem);
                myList.Add(subi);
                elem = "";
            }
        }

        if (subi.Equals("?")){
            myList.Add(subi);
        }
        else{
            myList.Add(elem);
        }

        return myList;
    }

    private void InitializeImageHelp(List<string> op, GameObject panel){
        foreach (string elem in op){
            if(elem.Equals("?")) {
                AddTextElement(elem, panel);
            }else if(IsCharANumber(elem)){
                AddRadialLayoutElement(elem,panel);
            }else{
                AddImageSimbolElement(elem, panel);
            }
        }
    }

    private void InitializeExercise(List<string> op, GameObject panel) {
        foreach (string elem in op){
            if(IsCharANumber(elem)){
                AddTextElement(elem, panel);
            }else{
                AddImageSimbolElement(elem, panel);
            }
        }
    }

    private void AddImageSimbolElement(string elem, GameObject panel) {
        GameObject newImage = new GameObject("ImageSimbol", typeof(RectTransform));
        Image newImageComp = newImage.AddComponent<Image>();
        newImage.transform.SetParent(panel.transform);
        newImage.GetComponent<RectTransform>().sizeDelta = new Vector2(50, 50);
        newImage.transform.localScale = new Vector3(1, 1, 1);
        switch (elem){
            case "+":
                newImageComp.sprite = plusImage;
                break;
            case "=":
                newImageComp.sprite = equalsImage;
                break;
            default:
                break;
        }
    }

    private void AddTextElement(string elem,GameObject panel){
        GameObject newText = new GameObject("TextNumber", typeof(RectTransform));
        Text newTextComp = newText.AddComponent<Text>();
        newTextComp.text = elem.ToString();
        newTextComp.font = numberFont;
        newTextComp.resizeTextForBestFit = true;
        newTextComp.fontSize = 55;
        newTextComp.resizeTextMaxSize = newTextComp.fontSize;
        newTextComp.alignment = TextAnchor.MiddleCenter;
        newText.transform.SetParent(panel.transform);
        newText.transform.localScale = new Vector3(1, 1, 1);

        if (elem.Equals("?")){
            newText.gameObject.tag = "Answer";
            Sc_ScaleEffect newEffect = newText.AddComponent<Sc_ScaleEffect>();
            newEffect.maxSize = 1.2f;
            newEffect.growFactor = 0.2f;
            newEffect.waitTime = 0;

            if(GameObject.FindGameObjectWithTag("AnswerDropContainer")!=null) {
                GameObject answerCont = GameObject.FindGameObjectWithTag("AnswerDropContainer");
                answerCont.SetActive(true);
                answerCont.transform.SetParent(newText.transform);
                answerCont.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                answerCont.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }

        }
    }

    private void AddRadialLayoutElement(string elem, GameObject panel) {
        GameObject newPanel = new GameObject("PanelObjects", typeof(RectTransform));
        Sc_RadialLayout newRadialLayoutComp = newPanel.AddComponent<Sc_RadialLayout>();
        newRadialLayoutComp.MinAngle = 0;
        newRadialLayoutComp.StartAngle = 0;
        newRadialLayoutComp.fDistance = 20;
        int number = int.Parse(elem.ToString());
        if (number <= 2){
            newRadialLayoutComp.MaxAngle = 180;
        }else{
            newRadialLayoutComp.MaxAngle = 280;
        }

        for (var i = 0; i < number; i++)
        {
            GameObject newImage = new GameObject("ImageObject", typeof(RectTransform));
            Image newImageComp = newImage.AddComponent<Image>();
            int index = Random.Range(0, objectsAdd.Length);
            newImageComp.sprite = objectsAdd[index];
            newImage.transform.SetParent(newPanel.transform);
            newImage.GetComponent<RectTransform>().sizeDelta = new Vector2(32, 32);
        }
        newPanel.transform.SetParent(panel.transform);
        newPanel.transform.localScale = new Vector3(1, 1, 1);
    }

    private bool IsCharANumber(string elem) {
        switch (elem){
            case "+":
                return false;
            case "=":
                return false;
            default:
                return true;
        }     
    }

    public void OnSoundButtonClick() {
        if (soundPause){
            soundPause = false;
            AudioListener.volume = Sc_GameOptionsController.VolumeSetUP;
            ButtonSound.sprite = unMutedSound;
        }else{
            soundPause = true;
            AudioListener.volume = 0;
            ButtonSound.sprite = mutedSound;
        }
            
    }
    
    public void ButtonKeyboardPress(Text childText) {
        ChangePanelAnswerText(childText.text, panelExercise);
        ReadyButton.color = new Color(1f, 1f, 1f);
        if (AnswerChanged) {
            numberAnwersChange++;
        }
        if (panelImageHelp.activeSelf){
            ChangePanelAnswerText(childText.text, panelImageHelp);
        }
    }

    private void ChangePanelAnswerText(string childText, GameObject panel){
        int count = panel.transform.childCount;
        GameObject childGame;
        for (var i = 0; i < count; i++){
            childGame = panel.transform.GetChild(i).gameObject;
            if (childGame.tag.Equals("Answer")){
                if (childGame.GetComponent<Text>().text == "?" || childText=="?"){
                    childGame.GetComponent<Text>().text = childText;
                    AnswerChanged = true;
                }
                else{
                    if (myAdditionTypeManager.QuestionsType == QuestionsType.Abiertas){
                        if (childGame.GetComponent<Text>().text.Length + 1 <= myExercise.AnswerLength()){
                            childGame.GetComponent<Text>().text += childText;
                        }
                    }else if(myAdditionTypeManager.QuestionsType == QuestionsType.Cerradas){
                        if (childGame.GetComponent<Text>().text.Equals(childText)){
                            AnswerChanged = false;
                        }else{
                            AnswerChanged = true;
                        }
                        childGame.GetComponent<Text>().text = childText;                        
                    }
                }
                
            }
        }

    }

    public void ButtonDeleteKeyboardPress(Text childText) {
        ChangePanelAnswerText("?", panelExercise);
        if (panelImageHelp.activeSelf){
            ChangePanelAnswerText("?", panelImageHelp);
        }
    }

    public void OnBackButtonClick() {
        SceneManager.LoadScene("Menu");
        this.GetComponent<AudioSource>().Pause();
    }

    public Image[] myOptionsImages;
    public void OnButtonPressedChangeColor(int index) {
        for (var i=0; i< myOptionsImages.Length;i++) {
            if (i==index) {
                myOptionsImages[i].color = new Color(0.13f, 0.59f, 0.953f);
            }
            else{
               myOptionsImages[i].color = Color.white;               
            }
        }      
    }
}
