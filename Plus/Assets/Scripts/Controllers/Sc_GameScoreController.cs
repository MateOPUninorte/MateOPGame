using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Sc_GameScoreController : MonoBehaviour
{

    public Text timerText;
    public Text scoreText;
    public Text message;
    private Sc_TimerCounter timerCounter;
    private Sc_ExerciseManager exerciseManager;
    public ParticleSystem particles;
    public Image ButtonSound;
    public Sprite mutedSound;
    public Sprite unMutedSound;

    public Image starsImage;
    public Sprite star1img;
    public Sprite star2img;
    public Sprite star3img;

    private bool soundPause = true;
    private int score;
    private int tempScore;
    private float growthFactor;
    private bool onGrowth;
    private Sc_QuickUserManager sc_QuickUserManager;
    private FirebaseOperations firebaseOperations;
    private bool star1, star2, star3;

    void Start(){
        GameObject myManagerObject = GameObject.FindWithTag("Manager");
        if(myManagerObject != null){
            Sc_FirebaseOperations fop = myManagerObject.GetComponent<Sc_FirebaseOperations>();
            if (fop != null){
                firebaseOperations = myManagerObject.GetComponent<Sc_FirebaseOperations>().FirebaseOperations;
            }
            sc_QuickUserManager = myManagerObject.GetComponent<Sc_QuickUserManager>();
        }

        if(AudioListener.volume == 0){
            soundPause = true;
            ButtonSound.sprite = mutedSound;
        }else{
            soundPause = false;
            ButtonSound.sprite = unMutedSound;
        }
        timerCounter = GameObject.FindGameObjectWithTag("Manager").GetComponent<Sc_TimerCounter>();
        exerciseManager= GameObject.FindGameObjectWithTag("Manager").GetComponent<Sc_ExerciseManager>();
        timerCounter.SetText(timerText);
        score = (int)(100 * exerciseManager.GetPorcentGoodAnswers());
        message.text = exerciseManager.GetGoodAnswers() + " preguntas correctas de " + exerciseManager.AllExercises.Count;
        onGrowth = true;
        growthFactor = 100;
        tempScore = 0;
        star1 = true;
        star2 = true;
        star3 = true;

        UpdateLevelAndStars();
        UpdateSesion();
        UpdateFileWithVectorPerformace();
    }

    void Update(){
        if(onGrowth){
            UpdateScore();
            WriteScore();

            if (tempScore < 50 && star1) {
                star1 = false;
                particles.Play();
                starsImage.sprite = star1img;
            } else if (tempScore >= 50 && star2) {
                star2 = false;
                particles.Play();
                starsImage.sprite = star2img;
            } else if (tempScore >= 80 && star3) {
                star3 = false;
                starsImage.sprite = star3img;
                particles.Play();
            }
        }
    }

    private void UpdateFileWithVectorPerformace() {
        if (GameObject.FindGameObjectWithTag("Manager").GetComponent<Sc_OfflineSesion>() == null) {
            Mdl_PerformanceVectors performanceVectors = Mdl_PerformanceVectors.ReadObjectFromFile();
            performanceVectors.UpdatePerformanceVectorsSet(exerciseManager.AllExercises, firebaseOperations.GetCurrentUserData().grado, firebaseOperations.GetCurrentUserData().sesion);
            Mdl_PerformanceVectors.WriteObjectInFile(performanceVectors);
            firebaseOperations.UpdateUserJsonPredict(Mdl_PerformanceVectors.ReadJsonFromFile());
        }
        //mandar a escribir el objeto a la bd, por si continua en otro divice
    }

    private void UpdateSesion() {
        if (GameObject.FindGameObjectWithTag("Manager").GetComponent<Sc_OfflineSesion>() == null) {
            int currentSesion = firebaseOperations.GetCurrentUserData().sesion;
            firebaseOperations.UpdateUserSesion(currentSesion + 1);
        }
    }

    private void UpdateLevelAndStars(){
        if (sc_QuickUserManager==null && GameObject.FindGameObjectWithTag("Manager").GetComponent<Sc_OfflineSesion>()==null){
            int level = firebaseOperations.GetCurrentUserData().nivel;
            int newStars = firebaseOperations.GetCurrentUserData().estrellas;
            if(score>0 && score < 50){
                newStars = newStars + 1;                
            }else if(score >=50 && score < 80){
                newStars = newStars + 2;
            }else if(score >= 80) {
                newStars = newStars + 3;
            }
            int newLevel = level;
            if(level <= 3){
                if(newStars>=((level+1)*(level+1))){
                    newLevel = level + 1;
                }
            }else{
                if(newStars>=(3*(level))){
                    newLevel = level + 1;
                }
            }
            if(newLevel != level){ 
                firebaseOperations.UpdateUserLevel(newLevel);
            }
            firebaseOperations.UpdateUserStars(newStars);
        }
    }

    private void WriteScore() {
        string text="";
        string score = tempScore + "";
        for (var i = 1;i<=3-score.Length;i++){
            text = text+"0";
        }
        text = text + score;
        scoreText.text = text;
    }

    private void UpdateScore(){
        if (tempScore < score){
            tempScore += (int)(growthFactor * Time.deltaTime);
        }else{
            tempScore = score;
            onGrowth = false;
        }
    }

    public void OnSoundButtonClick(){
        if (soundPause){
            soundPause = false;
            AudioListener.volume = Sc_GameOptionsController.VolumeSetUP;
            ButtonSound.sprite = unMutedSound;
        }else{
            soundPause = true;
            ButtonSound.sprite = mutedSound;
            AudioListener.volume = 0;
        }
    }

    public void OnContinueButton() {
        if (GameObject.FindGameObjectWithTag("Manager").GetComponent<Sc_OfflineSesion>()==null){
            if (sc_QuickUserManager!=null) {
                SceneManager.LoadScene("QuickSession");
            }else{
                GameObject.FindGameObjectWithTag("Manager").AddComponent<Sc_OpenScoreBoard>();
                SceneManager.LoadScene("Menu");
            }
        }else{
            SceneManager.LoadScene("Sesion");
        }            
    }

    
}
