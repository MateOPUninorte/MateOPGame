using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Firebase.Auth;
using System.Text.RegularExpressions;

public class Sc_SessionController : MonoBehaviour
{
    public GameObject panelSignIn;
    public GameObject panelSignUp;
    public InputField UserNameSignIn;
    public InputField PasswordSignIn;


    public InputField UserNameSignUp;
    public InputField PasswordSignUp;
    public InputField NamesSignUp;
    public InputField EmailSignUp;
    public InputField AgeSignUp;
    public Dropdown GenderSignUp;
    public Dropdown GradeSignUp;
    public Dropdown SchoolSignUp;
    private FirebaseOperations firebaseOperations;
    private bool RegisterReady;
    public RectTransform verticalLayout;
    public GameObject buttonPretest;
    public GameObject SpinnerSignInContainer;
    public GameObject SpinnerSignUpContainer;
    public GameObject errorSignInContainer;
    public GameObject errorSignUpContainer;
    public Button confirmSignIn;
    public Button confirmSignUp;
    public Text errorSignInText;
    public Text errorSignUpText;
    public GameObject[] contanier;
    public GameObject BackButton;
    public GameObject ContinueButton;
    public GameObject BoyButton;
    public GameObject GirlButton;
    private int indexSelected;
    private bool keyBStateOn = true;
    private bool keyBStateOff = true;
    List<Mdl_College> myCollegesOptions;
    public const string MatchEmailPattern =
            @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
            + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
              + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
            + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    void Start(){
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = false;
        Screen.orientation = ScreenOrientation.AutoRotation;

        GameObject myManagerObject = GameObject.FindWithTag("Manager");
        if (myManagerObject != null){
            Sc_FirebaseOperations fop = myManagerObject.GetComponent<Sc_FirebaseOperations>();
            if (fop != null) {
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
        myManagerObject.AddComponent<Sc_SessionInitialize>();
        StartCoroutine(InitializeDropdowsCollegeOptions());
        RegisterReady = true;
        if (contanier.Length>0) {
            indexSelected = 0;
            contanier[0].SetActive(true);
        } else {
            Debug.Log("Objeto Nulo");
        }
    }

    void OnDestroy(){
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
    
    void Update() {
        TouchScreenKeyboardOperations();
    }

    private void TouchScreenKeyboardOperations() {
        int factor;
        if (Screen.orientation == ScreenOrientation.Portrait){
            factor = 0;
            panelSignUp.GetComponent<RectTransform>().sizeDelta = new Vector2(480, 440);
        }else{
            factor = 1;
            panelSignUp.GetComponent<RectTransform>().sizeDelta = new Vector2(650, 500);
        }

        if (TouchScreenKeyboard.visible && keyBStateOn){
            if (panelSignIn.activeSelf){
                RectTransform position = panelSignIn.GetComponent<RectTransform>();
                if (UserNameSignIn.isFocused){
                    position.anchoredPosition = new Vector2(0, Mathf.Abs(GetKeyboardHeightRatio() - (position.sizeDelta.y * factor)-20));
                }
                if (PasswordSignIn.isFocused){
                    position.anchoredPosition = new Vector2(0, Mathf.Abs(GetKeyboardHeightRatio() - (position.sizeDelta.y * factor) + 20));
                }

            }

            if (panelSignUp.activeSelf){
                RectTransform position = panelSignUp.GetComponent<RectTransform>();
                //if (NamesSignUp.isFocused || AgeSignUp.isFocused){
                  //  position.anchoredPosition = new Vector2(0, Mathf.Abs(GetKeyboardHeightRatio() - (position.sizeDelta.y * factor)));
                //}
                if (NamesSignUp.isFocused || AgeSignUp.isFocused || EmailSignUp.isFocused || PasswordSignUp.isFocused || UserNameSignUp.isFocused){
                    position.anchoredPosition = new Vector2(0, Mathf.Abs(GetKeyboardHeightRatio() - (position.sizeDelta.y * factor) + 100));
                }
                //if (EmailSignUp.isFocused){
                  //  position.anchoredPosition = new Vector2(0, Mathf.Abs(GetKeyboardHeightRatio() - (position.sizeDelta.y * factor) + 2*verticalLayout.sizeDelta.y / 4));
                //}
                //if (PasswordSignUp.isFocused){
                  //  position.anchoredPosition = new Vector2(0, Mathf.Abs(GetKeyboardHeightRatio() - (position.sizeDelta.y * factor) + 3*verticalLayout.sizeDelta.y / 4));
                //}
            }
            keyBStateOn = false;
            keyBStateOff = true;
        }

        if (!TouchScreenKeyboard.visible && keyBStateOff){
            if (panelSignIn.activeSelf){
                RectTransform position = panelSignIn.GetComponent<RectTransform>();
                position.anchoredPosition = new Vector2(0, 0);
            }

            if (panelSignUp.activeSelf){
                RectTransform position = panelSignUp.GetComponent<RectTransform>();
                position.anchoredPosition = new Vector2(0, 0);
            }
            keyBStateOn = true;
            keyBStateOff = false;
        }

    }

    private float GetKeyboardHeightRatio(){
        if (Screen.orientation == ScreenOrientation.Portrait){
            return 0;
        }
        if (Application.platform == RuntimePlatform.Android){
            using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")){
                AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");
                using (AndroidJavaObject rect = new AndroidJavaObject("android.graphics.Rect")){
                    View.Call("getWindowVisibleDisplayFrame", rect);
                    return Mathf.Abs((Screen.height - rect.Call<int>("height")) - Screen.height/2);
                }
            }
        }else{
            return Mathf.Abs(TouchScreenKeyboard.area.height - Screen.height/2);
        }
    }

    IEnumerator InitializeDropdowsCollegeOptions(){
        float waitTime = 1f;
        yield return new WaitForSeconds(waitTime);
        myCollegesOptions=null;
        myCollegesOptions = firebaseOperations.ReturnCollegesNames();        
        while (waitTime<=4) {
            yield return new WaitForSeconds(waitTime);
            SchoolSignUp.options.Clear();
            if (myCollegesOptions!=null && myCollegesOptions.Count>0){
                foreach (Mdl_College college in myCollegesOptions){
                    Dropdown.OptionData newCollegeOption = new Dropdown.OptionData{text = college.nombre};
                    SchoolSignUp.options.Add(newCollegeOption);
                }
                break;
            }else{
                waitTime++;
            }
        }
        yield return new WaitForSeconds(0.5f);
        RegisterReady = true;
    }

    public void OnDropDownCollegesValueChanged() {
        if (SchoolSignUp.options.Count==0) {
            StartCoroutine(InitializeDropdowsCollegeOptions());
        }        
    }

    public void OnSignInButtonPress(){
        errorSignInContainer.SetActive(false);
        if (!UserNameSignIn.text.Equals("") && !PasswordSignIn.text.Equals("")){
            if (Regex.IsMatch(UserNameSignIn.text, MatchEmailPattern)){
                if (!IsAdmin()) { 
                    StartCoroutine(SignInResult(UserNameSignIn.text));
                } else     {
                    buttonPretest.SetActive(true);
                    UserNameSignIn.text = "";
                    PasswordSignIn.text = "";
                }
            } else {
                StartCoroutine(SearchEmailByUsername());
            }
        }else {
            errorSignInContainer.SetActive(true);
            errorSignInText.text = "*Digite sus datos correctamente";
        }
    }

    private bool IsAdmin() {
        if (UserNameSignIn.text.Equals("admin") && PasswordSignIn.text.Equals("012019")){
            return true;
        }
        return false;
    }
    
    IEnumerator SearchEmailByUsername(){
        float waitTime = 1f;
        List<string> emailByUserName = firebaseOperations.GetEmailByUserNameAvailable(UserNameSignIn.text);
        bool stateFinish = false;
        SpinnerSignInContainer.SetActive(true);
        while (waitTime <= 3f){
            yield return new WaitForSeconds(waitTime);
            if (emailByUserName != null && emailByUserName.Count > 0) {
                if (!emailByUserName[0].Equals("404")) {
                    StartCoroutine(SignInResult(emailByUserName[0]));
                    stateFinish = true;
                }
                break;
            }
            waitTime++;
        }
        if (!stateFinish){
            errorSignInContainer.SetActive(true);
            errorSignInText.text = "*Compruebe sus credenciales y su conexión a internet";
        }
        SpinnerSignInContainer.SetActive(false);
    }

    IEnumerator SignInResult(string email){
        float waitTime = 2f;
        bool stateFinish=false;
        SpinnerSignInContainer.SetActive(true);
        confirmSignIn.interactable = false;
        Task<FirebaseUser> signed = firebaseOperations.SignIn(email, PasswordSignIn.text);
        firebaseOperations.SetExerciseGeneratorURL();
        while (waitTime<=3f){            
            yield return new WaitForSeconds(waitTime);
            if (signed.IsCanceled || signed.IsFaulted) {
                Debug.Log("Sign in cancel");
                break;
            }if (signed.Result != null){
                stateFinish = true;
                break;
            }
            waitTime++;
        }
        if (stateFinish){
            SceneManager.LoadScene("Menu");
        }else{
            errorSignInContainer.SetActive(true);
            errorSignInText.text = "*Compruebe sus credenciales y su conexión a internet";
        }
        confirmSignIn.interactable = true;
        SpinnerSignInContainer.SetActive(false);
    }

    public void OnRegisterButtonPress(){
        errorSignUpContainer.SetActive(false);
        if (!UserNameSignUp.text.Equals("") && !PasswordSignUp.text.Equals("") && !EmailSignUp.text.Equals("") && !NamesSignUp.text.Equals("")&& !AgeSignUp.text.Equals("") && !SchoolSignUp.options[SchoolSignUp.value].text.Equals(""))
        {
            //verificar que el nombre de usuario no esta tomado, preferiblemente en el valuechanged
            if (PasswordSignUp.text.Length>=6) {
                if (Regex.IsMatch(EmailSignUp.text, MatchEmailPattern)){
                    StartCoroutine(SignUpResult());
                } else {
                    errorSignUpContainer.SetActive(true);
                    errorSignUpText.text = "*Dirección de correo electrónico incorrecta";
                }
            }else {
                errorSignUpContainer.SetActive(true);
                errorSignUpText.text = "*La contraseña debe contener 6 caracteres o mas";
            }
        }else{
            errorSignUpContainer.SetActive(true);
            errorSignUpText.text = "*Digite sus datos correctamente";
        }        
    }


    public void OnValueChangedUserName(InputField text) {
        text.text = text.text.ToLower();
    }

    public void PlayButtonOffline() {
        GameObject myManagerObject = GameObject.FindWithTag("Manager");
        if (myManagerObject.GetComponent<Sc_ExerciseManager>() != null && myManagerObject.GetComponent<Sc_AdditionTypeManager>() != null){
            Destroy(myManagerObject.GetComponent<Sc_ExerciseManager>());
            Destroy(myManagerObject.GetComponent<Sc_AdditionTypeManager>());
        }
        myManagerObject.AddComponent<Sc_OfflineSesion>();
        Sc_ExerciseManager myExerciseManager = myManagerObject.AddComponent<Sc_ExerciseManager>();
        Sc_AdditionTypeManager myAdditionTypeManager = myManagerObject.AddComponent<Sc_AdditionTypeManager>();
        Destroy(myManagerObject.GetComponent<Sc_QuickUserManager>());
        myAdditionTypeManager.QuestionsType = QuestionsType.Cerradas;
        Mdl_Intensities myPerformance = new Mdl_Intensities(new float[] { 0.5f, 1, 0 }, new float[] { 0.3f, 1, 0 }, new float[] { 0.2f, 1, 0 }, new float[] { 0f, 1, 0 }, new float[] { 0f, 1, 0 });
        myExerciseManager.AllExercises = Sc_ExerciseGeneratorController.GenerateExercisesFromLOPerformance(myPerformance, 15);
        AudioListener.volume = 0;
        SceneManager.LoadScene("NivelSumas");
    }

    IEnumerator SignUpResult(){
        confirmSignUp.interactable = false;
        SpinnerSignUpContainer.SetActive(true);
        int grado = GradeSignUp.value + 1;
        int gender = GenderSignUp.value;
        string school = SchoolSignUp.options[SchoolSignUp.value].text;
        float waitTime0 = 1f;
        bool stateFinish = false;
        bool userNameError = false;
        List<bool> checkNotTakenUserName = firebaseOperations.IsUserNameAvailable(UserNameSignUp.text);
        while (waitTime0 <= 3f){
            yield return new WaitForSeconds(waitTime0);
            if (checkNotTakenUserName != null && checkNotTakenUserName.Count > 0) {
                if(checkNotTakenUserName[0]==true) {
                    float waitTime = 1f;
                    List<bool> signed = firebaseOperations.CreateAccount(EmailSignUp.text, PasswordSignUp.text);
                    firebaseOperations.SetExerciseGeneratorURL();
                    while (waitTime <= 3f){
                        yield return new WaitForSeconds(waitTime);
                        if (signed != null && signed.Count > 0) {
                            if (signed[0]==true) {
                                float waitTime2 = 1f;
                                int tipoEscuela = 0;
                                firebaseOperations.UpdateUserProfile(UserNameSignUp.text);
                                foreach (Mdl_College college in myCollegesOptions) {
                                    if (school.Equals(college.nombre)) {
                                        tipoEscuela = college.tipo;
                                    }
                                }
                                List<bool> writeDB = firebaseOperations.CreateUserReference(NamesSignUp.text, EmailSignUp.text, UserNameSignUp.text, school, int.Parse(AgeSignUp.text), grado, gender, 0, 0, 0, tipoEscuela);
                                while (waitTime2 <= 3f){
                                    yield return new WaitForSeconds(waitTime2);
                                    if (writeDB != null && writeDB.Count > 0){
                                        if (signed[0] == true){
                                            stateFinish = true;
                                            break;
                                        }else{
                                            errorSignUpContainer.SetActive(true);
                                            errorSignUpText.text = "*Error al crear la cuenta DB";
                                        }
                                    }
                                    waitTime2++;
                                }
                            } else{
                                errorSignUpContainer.SetActive(true);
                                errorSignUpText.text = "*Error al crear la cuenta AUTH";
                            }
                            break;
                        }
                        waitTime++;
                    }
                }else{
                    userNameError = true;                                        
                }
                break;
            }
           // errorSignUpContainer.SetActive(true);
            //errorSignUpText.text = "*Error "+ waitTime0;
            waitTime0++;
        }

        if (stateFinish){
            SceneManager.LoadScene("Menu");
        }else{
            if (userNameError){
                errorSignUpContainer.SetActive(true);
                errorSignUpText.text = "*El nombre de usuario ya está en uso";
            }else{
                errorSignUpContainer.SetActive(true);
                errorSignUpText.text = "*Compruebe su conexión a internet";
            }
        }
        SpinnerSignUpContainer.SetActive(false);
        confirmSignUp.interactable = true;
    }

    public void OnOpenRegisterButtonPress() {
        if (RegisterReady){
            panelSignIn.SetActive(false);
            panelSignUp.SetActive(true);
        }
    }

    public void OnPretestButtonPress() {
        SceneManager.LoadScene("QuickSession");
    }

    public void OnBackButtonPress(){
        if (indexSelected - 1 >= 0){
            contanier[indexSelected].SetActive(false);
            indexSelected--;
            contanier[indexSelected].SetActive(true);
            ContinueButton.SetActive(true);
            confirmSignUp.gameObject.SetActive(false);
            errorSignUpContainer.SetActive(false);
            if (indexSelected == 0){
                BackButton.SetActive(false);
            }
        }

    }

    public void OnContinueButtonPress(){
        if (indexSelected + 1 <= contanier.Length){
            contanier[indexSelected].SetActive(false);
            indexSelected++;
            contanier[indexSelected].SetActive(true);
            BackButton.SetActive(true);
            if (indexSelected == contanier.Length - 1){
                ContinueButton.SetActive(false);
                confirmSignUp.gameObject.SetActive(true);
            }
        }
    }

    public void OnCloseButtonPress() {
        contanier[indexSelected].SetActive(false);
        indexSelected = 0;
        contanier[indexSelected].SetActive(true);
        confirmSignUp.gameObject.SetActive(false);
        ContinueButton.SetActive(true);
        BackButton.SetActive(false);
        errorSignUpContainer.SetActive(false);
    }

    public void SetGenderBoyOption() {
        GenderSignUp.value = 0;
        BoyButton.GetComponent<Sc_ScaleEffect>().enabled = true;
        GirlButton.GetComponent<Sc_ScaleEffect>().enabled = false;

        BoyButton.transform.GetChild(0).gameObject.SetActive(true);
        GirlButton.transform.GetChild(0).gameObject.SetActive(false);

    }

    public void SetGenderGirlOption() { 
        GenderSignUp.value = 1;
        GirlButton.GetComponent<Sc_ScaleEffect>().enabled = true;
        BoyButton.GetComponent<Sc_ScaleEffect>().enabled = false;

        GirlButton.transform.GetChild(0).gameObject.SetActive(true);
        BoyButton.transform.GetChild(0).gameObject.SetActive(false);

    }



}
