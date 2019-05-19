using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;




public class Sc_GameAdditionController : MonoBehaviour
{

    public GameObject panelExercise;
    public GameObject panelImageHelp;
    public Font numberFont;
    public Image ButtonSound;
    public Sprite mutedSound;
    public Sprite unMutedSound;
    public Sprite[] objectsAdd;

    public Sprite plusImage;
    public Sprite equalsImage;

    private Mdl_Exercise myExercise;
    private Sc_ExerciseManager myExerciseManager;

    private bool soundPause = false;
    public bool gameOver = true;

    void Start(){
        myExerciseManager = GameObject.FindGameObjectsWithTag("ExerciseManager")[0].GetComponent<Sc_ExerciseManager>();
        myExercise = myExerciseManager.GetCurrentExersise();
        ShowExercise();
    }

    private void ShowExercise() {
        DeleteAllChilds(panelExercise);
        DeleteAllChilds(panelImageHelp);
        List<string> op = DivideOperation(myExercise.Operation);
        InitializeExercise(op, panelExercise);
        if (panelImageHelp.activeSelf){
            InitializeImageHelp(op, panelImageHelp);
        }
    }

    private void DeleteAllChilds(GameObject gameObject){
        foreach (Transform child in gameObject.transform){
            Destroy(child.gameObject);
        }
    }

    private void NextExercise(){
        myExerciseManager.NextExercise();
        myExercise = myExerciseManager.GetCurrentExersise();
    }

    public void OnReadyButtonPress() {
        string answ = GameObject.FindGameObjectsWithTag("Answer")[0].GetComponent<Text>().text;
        if (!answ.Equals("?"))
        {
            myExerciseManager.GetCurrentExersise().PlayerAnswer = answ;
            NextExercise();
            ShowExercise();
        }

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
        newImage.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
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
        newTextComp.fontSize = 45;
        newTextComp.resizeTextForBestFit = true;
        newTextComp.alignment = TextAnchor.MiddleCenter;
        newText.transform.SetParent(panel.transform);
        newText.transform.localScale = new Vector3(1, 1, 1);

        if (elem.Equals("?")){
            newText.gameObject.tag = "Answer";
            Sc_ScaleEffect newEffect = newText.AddComponent<Sc_ScaleEffect>();
            newEffect.maxSize = 1.5f;
            newEffect.growFactor = 0.4f;
            newEffect.waitTime = 0.1f;
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
            this.GetComponent<AudioSource>().Play();
            ButtonSound.sprite = unMutedSound;
        }
        else{
            soundPause = true;
            ButtonSound.sprite = mutedSound;
            this.GetComponent<AudioSource>().Pause();
        }
            
    }

    public void ButtonKeyboardPress(Text childText) {
        ChangePanelAnswerText(childText.text, panelExercise);
        if (panelImageHelp.activeSelf){
            ChangePanelAnswerText(childText.text, panelImageHelp);
        }
    }

    private void ChangePanelAnswerText(string childText, GameObject panel){
        int count = panel.transform.childCount;
        GameObject childGame;
        for (var i = 0; i < count; i++)
        {
            childGame = panel.transform.GetChild(i).gameObject;
            if (childGame.tag.Equals("Answer"))
            {
                if (childGame.GetComponent<Text>().text == "?" || childText=="?") {
                    childGame.GetComponent<Text>().text = childText;
                }else {
                    if (childGame.GetComponent<Text>().text.Length+1<=myExercise.AnswerLength())
                    {
                        childGame.GetComponent<Text>().text += childText;
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
}
