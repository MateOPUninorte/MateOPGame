using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Sc_GameSplashController : MonoBehaviour
{
    public GameObject button;
    public GameObject text;
    public AudioClip bkgMusic;

    private FirebaseOperations firebaseOperations;
    void Awake(){
        GameObject myManagerObject = GameObject.FindWithTag("Manager");
        if (myManagerObject != null){
            Sc_FirebaseOperations fop = myManagerObject.GetComponent<Sc_FirebaseOperations>();
            if (fop != null){
                firebaseOperations = myManagerObject.GetComponent<Sc_FirebaseOperations>().FirebaseOperations;
            }
        }else{
            myManagerObject = new GameObject();
            AudioSource audioSource = myManagerObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = true;
            audioSource.loop = true;
            audioSource.clip = bkgMusic;
            audioSource.Play();
            Sc_FirebaseOperations myFirebaseManager = myManagerObject.AddComponent<Sc_FirebaseOperations>();
            firebaseOperations = new FirebaseOperations();
            myFirebaseManager.FirebaseOperations = firebaseOperations;
            myManagerObject.tag = "Manager";
            myManagerObject.name = "ManagerObject";
            DontDestroyOnLoad(myManagerObject);
        }
    }

    IEnumerator InitializeData(){
        float waitTime = 1f;
        bool result = false;
        while (waitTime < 3){
            yield return new WaitForSeconds(waitTime);
            if (firebaseOperations.GetCurrentUser() != null){
                firebaseOperations.SetExerciseGeneratorURL();
                yield return new WaitForSeconds(waitTime);
                result = true;
                break;
            }
            waitTime++;
        }
        if (result) {
            GameObject.FindWithTag("Manager").AddComponent<Sc_SessionInitialize>();
            SceneManager.LoadScene("Menu");
        } else{
            SceneManager.LoadScene("Sesion");
        }
    }

    public void OnBannerPress(){
        button.GetComponent<Sc_ScaleEffect>().growFactor = 0;
        text.GetComponent<Sc_ScaleEffect>().growFactor = 0;
        button.GetComponent<Button>().interactable=false;
        StartCoroutine("InitializeData");
    }

}
