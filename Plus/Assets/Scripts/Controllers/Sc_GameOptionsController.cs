using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class Sc_GameOptionsController : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject optionsMenuPanel;
    public GameObject infoMenuPanel;
    public GameObject scoreBoardPanel;
    public GameObject PanelTopMenu;
    public GameObject PanelBottomMenu;
    public GameObject PanelBanner;
    public GameObject PlayButton;
    public GameObject WelcomePanel;
    public Image buttonSound;
    public Image buttonMainSound;
    public Slider volumeSlider;
    public Sprite mutedSound;
    public Sprite unMutedSound;
    public Sprite mutedSoundMain;
    public Sprite unMutedSoundMain;
    private bool volumeActive=true;
    private FirebaseOperations firebaseOperations;


    void Start(){
        GameObject myManagerObject = GameObject.FindWithTag("Manager");
        if(myManagerObject != null){
            Sc_FirebaseOperations fop = myManagerObject.GetComponent<Sc_FirebaseOperations>();
            if (fop != null){
                firebaseOperations = myManagerObject.GetComponent<Sc_FirebaseOperations>().FirebaseOperations;
            }
        }else{
            myManagerObject = new GameObject();
            Sc_FirebaseOperations myFirebaseManager = myManagerObject.AddComponent<Sc_FirebaseOperations>();
            firebaseOperations = new FirebaseOperations();
            myFirebaseManager.FirebaseOperations = firebaseOperations;
            myManagerObject.tag = "Manager";
            myManagerObject.name = "ManagerObject";
            DontDestroyOnLoad(myManagerObject);
        }        
        mainMenuPanel.SetActive(true);
        optionsMenuPanel.SetActive(false);
        infoMenuPanel.SetActive(false);
        scoreBoardPanel.SetActive(false);
        Sc_OpenScoreBoard sc_OpenScoreBoard = myManagerObject.GetComponent<Sc_OpenScoreBoard>();
        if (sc_OpenScoreBoard != null) {
            OnScoreBoardButtonPress();
            Destroy(sc_OpenScoreBoard);
        }
        Sc_SessionInitialize welcome = myManagerObject.GetComponent<Sc_SessionInitialize>();
        if (welcome != null){
            StartCoroutine(WelcomePanelFadeout());
            Destroy(welcome);
        }else {
            Destroy(WelcomePanel);
        }
    }

    IEnumerator WelcomePanelFadeout(){
        Text text=null;
        Image image=null;
        for (var i=0;i<WelcomePanel.transform.childCount;i++) {
            GameObject child = WelcomePanel.transform.GetChild(i).gameObject;
            if (child.GetComponent<Image>()!=null) {
                image = child.GetComponent<Image>();
            }
            if (child.GetComponent<Text>()!=null) {
                text = child.GetComponent<Text>();
            }
        }
        text.text ="Bienvenido "+ firebaseOperations.GetCurrentUser().DisplayName;
        float colorState = 1;
        while(colorState>0) {
            colorState = colorState - 0.05f;
            image.color = new Color(1, 1, 1, colorState);
            text.color = new Color(0.2f, 0.2f, 0.2f, colorState); 
            yield return new WaitForSeconds(0.5f);
        }
        Destroy(WelcomePanel);
    }

    public void OnStartButtonPress(){
        GameObject myManagerObject = GameObject.FindWithTag("Manager");
        if (myManagerObject.GetComponent<Sc_ExerciseManager>()!=null && myManagerObject.GetComponent<Sc_AdditionTypeManager>()!=null ) {
            Destroy(myManagerObject.GetComponent<Sc_ExerciseManager>());
            Destroy(myManagerObject.GetComponent<Sc_AdditionTypeManager>());
        }
        Sc_ExerciseManager myExerciseManager = myManagerObject.AddComponent<Sc_ExerciseManager>();
        Sc_AdditionTypeManager myAdditionTypeManager = myManagerObject.AddComponent<Sc_AdditionTypeManager>();
        Destroy(myManagerObject.GetComponent<Sc_QuickUserManager>());
        myAdditionTypeManager.QuestionsType = QuestionsType.Cerradas;


        //esto no
        //Mdl_Intensities myPerformance = new Mdl_Intensities(new float[]{ 0.23f, 1, 0 }, new float[] { 0.27f, 1, 0 }, new float[] { 0.3f, 1, 0 }, new float[] { 0.1f, 1, 0 }, new float[] { 0.1f, 1, 0 });
        //myExerciseManager.AllExercises = Sc_ExerciseGeneratorController.GenerateExercisesFromLOPerformance(myPerformance, 15);
        //SceneManager.LoadScene("NivelSumas");
        //esto si/**/
        if (firebaseOperations.GetCurrentUserData().sesion>0) {
            //verificar si el archivo existe, si no mandar a perdir enla base de datos y escribir en ela rchivo
            //obtener el objet leyendo el archio o mediante base de datos
            //hacer el http postrequest
            StartCoroutine(GetPredictedPerformanceFromPlayerData(myExerciseManager));
        }else{
            //Mdl_Intensities myPerformance = new Mdl_Intensities(new float[] { 0.23f, 1, 0 }, new float[] { 0.27f, 1, 0 }, new float[] { 0.3f, 1, 0 }, new float[] { 0.1f, 1, 0 }, new float[] { 0.1f, 1, 0 });
            //myExerciseManager.AllExercises = Sc_ExerciseGeneratorController.GenerateExercisesFromLOPerformance(myPerformance, 15);
            //Mdl_PerformanceVectors myInitialPerformance = new Mdl_PerformanceVectors();
            //myInitialPerformance.grado = firebaseOperations.GetCurrentUserData().grado;
            //myInitialPerformance.sesion = firebaseOperations.GetCurrentUserData().sesion;
            //myInitialPerformance.SetBinPerformanceVectors(new float[][] { new float[] { 1, 0 } }, new float[][] { new float[] { 1, 0 } }, new float[][] { new float[] { 1, 0 } }, new float[][] { new float[] { 1, 0 } }, new float[][] { new float[] { 1, 0 } });
            //Mdl_PerformanceVectors.WriteObjectInFile(myInitialPerformance);
            //SceneManager.LoadScene("NivelSumasDragAndDrop");
            StartCoroutine(GetInitialPredictedPerformanceFromPlayerData(firebaseOperations.GetCurrentUserData(), myExerciseManager));
        }
    }

    IEnumerator GetPredictedPerformanceFromPlayerData(Sc_ExerciseManager myExerciseManager){
        string jsonStringTrial = "";
        //jsonStringTrial = Mdl_PerformanceVectors.ReadJsonFromFile();
        //if (jsonStringTrial==null) {
            List<string> resultJson = null;
            resultJson = firebaseOperations.GetUserJsonPredict();
            float waitTime = 1f;
            while (waitTime <= 3f) { 
                yield return new WaitForSeconds(waitTime);
                if (resultJson != null && resultJson.Count > 0) {
                    Mdl_PerformanceVectors.WriteJsonInFile(resultJson[0]);
                    jsonStringTrial = Mdl_PerformanceVectors.ReadJsonFromFile();
                    break;
                }
                waitTime++;
            }
       // }
        
        UnityWebRequest www = UnityWebRequest.Put(Sc_ExerciseGeneratorController.UrlARN+Sc_ExerciseGeneratorController.MethodModelTwo, jsonStringTrial);
        www.method = "POST";
        www.SetRequestHeader("Content-Type","application/json");
        www.SetRequestHeader("Accept","application/json");
        PlayButton.GetComponent<Button>().interactable = false;
        PlayButton.GetComponent<Sc_ScaleEffect>().enabled = false;
        yield return www.SendWebRequest();
        PlayButton.GetComponent<Button>().interactable = true;
        PlayButton.GetComponent<Sc_ScaleEffect>().enabled = true;
        if (www.isNetworkError){
            Debug.Log("Network Error sending player data" + www.error);
        }else if(www.isHttpError){
            Debug.Log("Http error: "+www.error);
        }else{
            Debug.Log("POST successful!");
            string body = www.downloadHandler.text;

            Debug.Log(body);
            //Escribir body en el archivo
            //obtener string jsonDeIntensidades
            Mdl_Intensities.IntensitiesFormJSon myPerJSon = JsonUtility.FromJson<Mdl_Intensities.IntensitiesFormJSon>(Mdl_PerformanceVectors.GetIntensitiesOfJson(body));
            Mdl_Intensities myPerformance = new Mdl_Intensities(myPerJSon.LOIN0, myPerJSon.LOIN1, myPerJSon.LOIN2, myPerJSon.LOIN3, myPerJSon.LOIN4);
            myExerciseManager.AllExercises = Sc_ExerciseGeneratorController.GenerateExercisesFromLOPerformance(myPerformance, 15);
            Mdl_PerformanceVectors.WriteJsonInFile(body);

            AudioListener.volume = 0;
            SceneManager.LoadScene("NivelSumas");            
        }
    }

    IEnumerator GetInitialPredictedPerformanceFromPlayerData(Mdl_User myUser, Sc_ExerciseManager myExerciseManager){
        string jsonStringTrial = JsonUtility.ToJson(myUser);
        //Empanada porque yes
        //Debug.Log(myUser.edad);
        //Debug.Log(jsonStringTrial);
        //jsonStringTrial = "{\"edad\":" + myUser.edad+ ",\"tipoEscuela\":1,\"genero\":" + myUser.genero+ ",\"grado\":" + myUser.grado+"}";

        UnityWebRequest www = UnityWebRequest.Put(Sc_ExerciseGeneratorController.UrlARN+Sc_ExerciseGeneratorController.MethodModelOne, jsonStringTrial);
        www.method = "POST";
        www.SetRequestHeader("Content-Type","application/json");
        www.SetRequestHeader("Accept","application/json");
        PlayButton.GetComponent<Button>().interactable = false;
        PlayButton.GetComponent<Sc_ScaleEffect>().enabled = false;
        yield return www.SendWebRequest();
        PlayButton.GetComponent<Button>().interactable = true;
        PlayButton.GetComponent<Sc_ScaleEffect>().enabled = true;
        if (www.isNetworkError){
            Debug.Log("Network Error sending player data" + www.error);
        }else if(www.isHttpError){
            Debug.Log("Http error: "+www.error);
        }else{
            Debug.Log("POST successful!");
            string body = www.downloadHandler.text;
            Mdl_Intensities.IntensitiesFormJSon myPerJSon = JsonUtility.FromJson<Mdl_Intensities.IntensitiesFormJSon>(body);
            Mdl_Intensities myPerformance = new Mdl_Intensities(myPerJSon.LOIN0, myPerJSon.LOIN1,myPerJSon.LOIN2, myPerJSon.LOIN3,myPerJSon.LOIN4);
            myExerciseManager.AllExercises = Sc_ExerciseGeneratorController.GenerateExercisesFromLOPerformance(myPerformance, 15);
            AudioListener.volume = 0;
            Mdl_PerformanceVectors myInitialPerformance = new Mdl_PerformanceVectors();
            myInitialPerformance.grado = firebaseOperations.GetCurrentUserData().grado;
            myInitialPerformance.sesion = firebaseOperations.GetCurrentUserData().sesion;
            myInitialPerformance.SetBinloPerformanceVectors(new float[][]{new float[]{1,0}},new float[][]{new float[]{1,0}},new float[][]{new float[]{1,0}},new float[][]{new float[]{1,0}},new float[][]{new float[]{1,0}});
            myInitialPerformance.SetBinperPerformanceVectors( new float[] { 1, 0 },new float[] { 1, 0 },new float[] { 1, 0 },new float[] { 1, 0 }, new float[] { 1, 0 });
            Mdl_PerformanceVectors.WriteObjectInFile(myInitialPerformance);
            SceneManager.LoadScene("NivelSumas");            
        }
    }

    public void OnOptionsPress() {
        if (!IsBannerMoving(PanelTopMenu) && !IsBannerMoving(PanelBottomMenu) && !IsBannerMoving(PanelBanner)){
            MovePanelToPosition(PanelTopMenu, false, 100,0,100);
            MovePanelToPosition(PanelBottomMenu, false, -100,0,100);
            MovePanelToPosition(PanelBanner, false,0, -200,200);
            PlayButton.SetActive(false);
            optionsMenuPanel.SetActive(true);
        }
    }

    public void OnInfoPress(){
        if (!IsBannerMoving(PanelTopMenu) && !IsBannerMoving(PanelBottomMenu) && !IsBannerMoving(PanelBanner)){
            MovePanelToPosition(PanelTopMenu, false, 100,0,100);
            MovePanelToPosition(PanelBottomMenu, false, -100,0,100);
            MovePanelToPosition(PanelBanner, false, 0, -200,200);
            PlayButton.SetActive(false);
            infoMenuPanel.SetActive(true);
        }
    }

    public void OnScoreBoardButtonPress()
    {
        if (!IsBannerMoving(PanelTopMenu) && !IsBannerMoving(PanelBottomMenu) && !IsBannerMoving(PanelBanner)){
            MovePanelToPosition(PanelTopMenu, false, 100,0,100);
            MovePanelToPosition(PanelBottomMenu, false, -100,0,100);
            MovePanelToPosition(PanelBanner, false, 0, -200,200);
            PlayButton.SetActive(false);
            scoreBoardPanel.SetActive(true);
        }
    }

    public void OnBackOptionsPress()
    {
        if (!IsBannerMoving(PanelTopMenu) && !IsBannerMoving(PanelBottomMenu) && !IsBannerMoving(PanelBanner)) {
            MovePanelToPosition(PanelTopMenu, true, -100,0,100);
            MovePanelToPosition(PanelBottomMenu, true, 100,0,100);
            MovePanelToPosition(PanelBanner, false, 0, 200,200);
            PlayButton.SetActive(true);
            infoMenuPanel.SetActive(false);
            optionsMenuPanel.SetActive(false);
            scoreBoardPanel.SetActive(false);
        }
    }

    private bool IsBannerMoving(GameObject panel) {
        Sc_MoveToPosition moveToPosition = panel.GetComponent<Sc_MoveToPosition>();
        if (moveToPosition.canMove){
            return true;
        }else{
            return false;
        }
    }

    public static float VolumeSetUP=1; 
    public void AdjustVolume(Slider newVolume)
    {
        if (newVolume.value == 0){
            AudioListener.volume = 0;
            volumeActive = false;
            buttonMainSound.sprite = mutedSoundMain;
            buttonSound.sprite = mutedSound;
        }else{
            volumeActive = true;
            buttonSound.sprite = unMutedSound;
            buttonMainSound.sprite = unMutedSoundMain;
        }

        if (volumeActive){
            AudioListener.volume = newVolume.value;
            VolumeSetUP = newVolume.value;
        }
        
    }

    public void OnVolumeButtonPress() {
        if (AudioListener.volume==0) {
            AudioListener.volume = 1;
            volumeSlider.value = 1;
            buttonSound.sprite = unMutedSound;
            buttonMainSound.sprite = unMutedSoundMain;
            volumeActive = false;
        }
        else {
            volumeActive = true;
            AudioListener.volume = 0;
            volumeSlider.value = 0;
            buttonSound.sprite = mutedSound;
            buttonMainSound.sprite = mutedSoundMain;
        }
    }

    public void MovePanelToPosition(GameObject panel, bool childState, float sumy, float sumx, float speed) {
        Sc_MoveToPosition mover = panel.GetComponent<Sc_MoveToPosition>();
        mover.canMove = true;
        mover.sumy = sumy;
        mover.sumx = sumx;
        mover.speed = speed;
        mover.Initialize();
        for (int i = 0; i < panel.transform.childCount; i++)
        {
            var child = panel.transform.GetChild(i).gameObject;
            if (child != null) { 
                Button childButton = child.GetComponent<Button>();
                childButton.interactable = childState;
            }
        }

    }

    public void OnLogoutButtonPress() {
        firebaseOperations.SignOut();
        SceneManager.LoadScene("Sesion");
    }



}
