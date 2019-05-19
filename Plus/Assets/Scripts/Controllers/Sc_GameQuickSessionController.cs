using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Sc_GameQuickSessionController : MonoBehaviour
{
    public GameObject[] contanier;
    public GameObject panelKeyBoard;
    public InputField AgeSign;
    public Dropdown GenderSign;
    public Dropdown GradeSign;
    public Dropdown SchoolSign;
    public Text buttonText;
    public GameObject BackButton;
    public GameObject ContinueButton;
    private GameObject PretestTypeContainer;
    public Image ButtonSound;
    public Sprite mutedSound;
    public Sprite unMutedSound;
    public Scrollbar verticalScrollbar;
    public Button UploadButton;
    private FirebaseOperations firebaseOperations;
    private List<Mdl_College> myCollegesOptions;
    private int indexSelected;
    private bool soundPause = true;

    void Start() {
        //firebaseOperations = new FirebaseOperations();//Esto se necesita para el modo online
        //StartCoroutine("InitializeExercises");
        if (contanier[0] != null) {
            indexSelected = 0;
            contanier[0].SetActive(true);
        } else {
            Debug.Log("Objeto Nulo");
        }
        if (soundPause){
            ButtonSound.sprite = mutedSound;
            this.GetComponent<AudioSource>().Pause();
        }        
    }

    IEnumerator InitializeExercises() {
        yield return new WaitForSeconds(1f);
        //Offline Code
        //OfflineLoad();-esto no se llama aca
        //Online Code
        //OnlineQuestions();
    }
    IEnumerator UploadExercises()
    {
        firebaseOperations = new FirebaseOperations();
        yield return new WaitForSeconds(5f);
        UploadAllDataColeccted();
    }

    public void OnlineQuestions(){
        firebaseOperations.RetrieveQuestionsFromDatabase();
        myCollegesOptions=firebaseOperations.ReturnCollegesNames();
        StartCoroutine("InitializeDropdowsCollegeOptions");
    }

    public List<Mdl_Exercise> OfflineLoad() {
        string questions = "{\"answer\" : \"4\",\"loID\" : 0,\"problem\" : \"1+3=?\", \"tipo\" : 0};{\"answer\" : \"6\",\"loID\" : 0,\"problem\" : \"4+2=?\",\"tipo\" : 0};{ \"answer\" : \"7\",\"loID\" : 0,\"problem\" : \"4+3=?\",\"tipo\" : 0};{\"answer\" : \"4\",\"loID\" : 0,\"problem\" : \"3+1=?\",\"tipo\" : 0};"+
        "{\"answer\" : \"10\",\"loID\" : 0,\"problem\" : \"6+4=?\",\"tipo\" : 0};{\"answer\" : \"11\",\"loID\" : 1,\"problem\" : \"1+10=?\",\"tipo\" : 0};{\"answer\" : \"13\",\"loID\" : 1,\"problem\" : \"3+10=?\",\"tipo\" : 0};{\"answer\" : \"15\",\"loID\" : 1,\"problem\" : \"5+10=?\",\"tipo\" : 0};{\"answer\" : \"17\",\"loID\" : 1,\"problem\" : \"7+10=?\",\"tipo\" : 0};"+
        "{\"answer\" : \"10\", \"loID\" : 1,\"problem\" : \"0+10=?\", \"tipo\" : 0};{\"answer\" : \"14\",\"loID\" : 2,\"problem\" : \"2+12=?\",\"tipo\" : 0};{\"answer\" : \"17\",\"loID\" : 2,\"problem\" : \"6+11=?\",\"tipo\" : 0};{\"answer\" : \"24\",\"loID\" : 2,\"problem\" : \"7+17=?\",\"tipo\" : 0};{\"answer\" : \"29\",\"loID\" : 2,\"problem\" : \"8+21=?\",\"tipo\" : 0};"+
        "{\"answer\" : \"33\",\"loID\" : 2,\"problem\" : \"3+30=?\",\"tipo\" : 0};{\"answer\" : \"23\",\"loID\" : 3,\"problem\" : \"13+10=?\",\"tipo\" : 0};{\"answer\" : \"32\",\"loID\" : 3,\"problem\" : \"22+10=?\",\"tipo\" : 0};{\"answer\" : \"27\",\"loID\" : 3, \"problem\" : \"17+10=?\",\"tipo\" : 0};{\"answer\" : \"43\",\"loID\" : 3,\"problem\" : \"33+10=?\",\"tipo\" : 0};"+
        "{\"answer\" : \"50\",\"loID\" : 3,\"problem\" : \"40+10=?\",\"tipo\" : 0};{\"answer\" : \"42\",\"loID\" : 4,\"problem\" : \"17+25=?\",\"tipo\" : 0};{\"answer\" : \"50\",\"loID\" : 4,\"problem\" : \"25+25=?\",\"tipo\" : 0};{\"answer\" : \"77\",\"loID\" : 4,\"problem\" : \"42+35=?\",\"tipo\" : 0};{\"answer\" : \"83\",\"loID\" : 4,\"problem\" : \"46+37=?\",\"tipo\" : 0}"; //;{\"answer\" : \"100\",\"loID\" : 4,\"problem\" : \"61+39=?\",\"tipo\" : 0}
        string[] eachQ = questions.Split(';');
        
        List<Mdl_Exercise> exercisesList = new List<Mdl_Exercise>();
        for (var i = 0; i < eachQ.Length; i++) {
            Mdl_Exercise MyExercise  = JsonUtility.FromJson<Mdl_Exercise>(eachQ[i]);
            exercisesList.Add(MyExercise);
        }

        string lastE = "{\"answer\" : \"100\",\"loID\" : 4,\"problem\" : \"61+39=?\",\"tipo\" : 0}";
        Mdl_Exercise MyExercis = JsonUtility.FromJson<Mdl_Exercise>(lastE);
        exercisesList.Add(MyExercis);
        return exercisesList;
    }

    public void OnUploadButtonPress() {
        StartCoroutine("UploadExercises");
    }

    private void UploadAllDataColeccted(){
        StreamReader reader;
        if (Application.isEditor){
            reader = new StreamReader(Application.dataPath + "/StudentsData.txt");
        }else {
            reader = new StreamReader(Application.persistentDataPath + "/StudentsData.txt");
        }
        
        string allData = reader.ReadToEnd();
        string[] eachJsonE = allData.Split(';');
        int countE = 0;
        string userTempID = firebaseOperations.AssignUserIDForSession();
        for (var i = 0; i < eachJsonE.Length - 1; i++)
        {
            string[] Exc = eachJsonE[i].Split('|');
            Mdl_Survey MyExercise = JsonUtility.FromJson<Mdl_Survey>(Exc[1]);
            if ((countE + "").Equals(Exc[0]) || ("\n"+countE).Equals(Exc[0])){
                firebaseOperations.CreateSurveyAnswerLog(userTempID, MyExercise.edad, MyExercise.genero, MyExercise.grado, MyExercise.escuela, MyExercise.problema, MyExercise.respuesta, MyExercise.respuestaJ, MyExercise.tiempo, MyExercise.loID, MyExercise.tipo, MyExercise.answerChangedCount,false);
                countE++;
            }else {
                userTempID = firebaseOperations.AssignUserIDForSession();
                countE = 0;
                firebaseOperations.CreateSurveyAnswerLog(userTempID, MyExercise.edad, MyExercise.genero, MyExercise.grado, MyExercise.escuela, MyExercise.problema, MyExercise.respuesta, MyExercise.respuestaJ, MyExercise.tiempo, MyExercise.loID, MyExercise.tipo, MyExercise.answerChangedCount,false);
                countE++;
            }

        }
        UploadButton.interactable = false;
        reader.Close();
        if (Application.isEditor){
            File.Move(Application.dataPath + "/StudentsData.txt", Application.dataPath + "/UselessStudentsData.txt");
        }else {
            File.Move(Application.persistentDataPath + "/StudentsData.txt", Application.persistentDataPath + "/UselessStudentsData.txt");
        }
    }

    public void InputTextValueChanged() {
        if (AgeSign.text == "2580"){
            UploadButton.interactable = true;
        } else {
            if (UploadButton.interactable) {
                UploadButton.interactable = false;
            }
        }
    }


    IEnumerator InitializeDropdowsCollegeOptions() {
        yield return new WaitForSeconds(1f);
        foreach (Mdl_College college in myCollegesOptions){
            Dropdown.OptionData newCollegeOption = new Dropdown.OptionData();
            newCollegeOption.text = college.nombre;
            SchoolSign.options.Add(newCollegeOption);
        }
    }




    public void OnBackButtonPress(){
        if (indexSelected-1 >= 0) {
             contanier[indexSelected].SetActive(false);
             indexSelected--;
             contanier[indexSelected].SetActive(true);
             ContinueButton.SetActive(true);
             if (indexSelected==0) {
                 BackButton.SetActive(false);
             }
        }
        if (indexSelected == 0){
            panelKeyBoard.SetActive(true);
        }
        /* for (var i=0;i<contanier.Length-1;i++) {
             contanier[i].SetActive(true);
         }
         contanier[contanier.Length - 1].SetActive(false);
         BackButton.SetActive(false);
         ContinueButton.SetActive(true);*/
    }

    public void OnContinueButtonPress() {
        if (indexSelected == 0){
            if (AgeSign.text == "") {
                StartCoroutine("HigthlightedInput");
                return;
            }
            panelKeyBoard.SetActive(false);
        }

        if (indexSelected + 1 <= contanier.Length) {
            contanier[indexSelected].SetActive(false);
            indexSelected++;
            contanier[indexSelected].SetActive(true);
            BackButton.SetActive(true);
            if (indexSelected== contanier.Length-1) {
                ContinueButton.SetActive(false);
            }
        }


        /*for (var i = 0; i < contanier.Length - 1; i++){
            contanier[i].SetActive(false);
        }
        contanier[contanier.Length - 1].SetActive(true);
        ContinueButton.SetActive(false);
        BackButton.SetActive(true);*/
        //else if(indexSelected + 1 == contanier.Length){
        //Hacer visible los dos botones
        // PretestTypeContainer.SetActive(true);
        //ContinueButton.SetActive(false);
        // buttonText.text = "EMPEZAR!";
        //contanier[indexSelected].SetActive(false);
        //indexSelected++;
        // }
    }


    public void LoadAdditionScene(QuestionsType myType) {
        if (AgeSign.text != "")
        {
            GameObject myManagerObject = GameObject.FindWithTag("Manager");
            if (myManagerObject != null)
            {
                Destroy(myManagerObject.GetComponent<Sc_ExerciseManager>());
                Destroy(myManagerObject.GetComponent<Sc_QuickUserManager>());
                Destroy(myManagerObject.GetComponent<Sc_AdditionTypeManager>());
                Destroy(myManagerObject.GetComponent<Sc_TimerCounter>());
            }
            else
            {
                myManagerObject = new GameObject();
            }
            
            Sc_ExerciseManager  myExerciseManager = myManagerObject.AddComponent<Sc_ExerciseManager>();
            Sc_QuickUserManager myQuickUserManager = myManagerObject.AddComponent<Sc_QuickUserManager>();
            Sc_AdditionTypeManager myAdditionTypeManager = myManagerObject.AddComponent<Sc_AdditionTypeManager>();

            myQuickUserManager.Edad = int.Parse(AgeSign.text);
            myQuickUserManager.Grado = GradeSign.value + 1;
            myQuickUserManager.Genero = GenderSign.value;
            myQuickUserManager.Escuela = SchoolSign.captionText.text;
            myQuickUserManager.Id = "UserID";
            //online
            //myQuickUserManager.Id = firebaseOperations.AssignUserIDForSession();
            //myExerciseManager.AllExercises = firebaseOperations.GetExercisesList();
            //offline
            myExerciseManager.AllExercises = OfflineLoad();


            myAdditionTypeManager.QuestionsType = myType;

            myManagerObject.name = "ManagerObject";
            myManagerObject.tag = "Manager";

            SceneManager.LoadScene("NivelSumas");
        } else {
            //verticalScrollbar.value = 1;
            //StartCoroutine("HigthlightedInput");
        }
    }

    public void OnSoundButtonClick()
    {
        if (soundPause)
        {
            soundPause = false;
            this.GetComponent<AudioSource>().Play();
            ButtonSound.sprite = unMutedSound;
        }
        else
        {
            soundPause = true;
            ButtonSound.sprite = mutedSound;
            this.GetComponent<AudioSource>().Pause();
        }

    }

    IEnumerator HigthlightedInput()
    {
        AgeSign.gameObject.GetComponent<Image>().color = new Color(1, 0, 0);
        float Green=0;
        while (Green <= 0.7f) {
            AgeSign.gameObject.GetComponent<Image>().color = new Color(1, Green, 0);
            Green += 0.08f;
            yield return new WaitForSeconds(0.1f);
        }
        AgeSign.gameObject.GetComponent<Image>().color = new Color(0.902f, 0.74f, 0.43f);
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

    public void OnPretestAbiertoButtonPress(){
        LoadAdditionScene(QuestionsType.Abiertas);
    }

    public void OnPretestCerradoButtonPress(){
        LoadAdditionScene(QuestionsType.Cerradas);
    }

    public void ValueOfInputChanged(){
        StartCoroutine(ScrollbarMovement(0.8f));   
    }


    IEnumerator ScrollbarMovement(float compare){
        while (verticalScrollbar.value> compare) {
            verticalScrollbar.value -= 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public void SetTextOfInputAge(Text inputText) {
        if (inputText.text.Equals("X"))
        {
            AgeSign.text = "";
        }
        else
        {
            AgeSign.text += inputText.text;
        }
    }

}
