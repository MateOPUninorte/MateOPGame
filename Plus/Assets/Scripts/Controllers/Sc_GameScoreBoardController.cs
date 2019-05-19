using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Sc_GameScoreBoardController : MonoBehaviour
{
    public GameObject DataContainerPrefab;
    public GameObject Container;
    public GameObject SpinnerContainer;
    public Sprite BoyImage;
    public Sprite GirlImage;
    public Toggle otherGradesToogle;
    public Text ScoreBoarPageUI;
    private int ScoreBoarPage;
    private int PageSize;
    private int NumberOfPages;
    private bool enabelToPreview;
    private bool enableToNext;
    private List<string> endAtPreview;
    //private List<Mdl_UserScoreBoard> myUserScoresPage;
    private FirebaseOperations firebaseOperations;

    void Start(){
        ScoreBoarPage = 1;
        PageSize = 10;
        GameObject myManagerObject = GameObject.FindWithTag("Manager");
        if(myManagerObject != null){
            Sc_FirebaseOperations fop = myManagerObject.GetComponent<Sc_FirebaseOperations>();
            if (fop != null){
                firebaseOperations = myManagerObject.GetComponent<Sc_FirebaseOperations>().FirebaseOperations;
            }
        }
        enabelToPreview = false;
        enableToNext = false;
        SpinnerContainer.SetActive(false);
        NumberOfPages = -1;
        endAtPreview = new List<string>();
    }


    void OnEnable() {        
        StartCoroutine(InitializeNumberOfPages());
    }

    void OnDisable(){
        int childCount = Container.transform.childCount;
        for (var i =0;i<childCount;i++) {
            if (Container.transform.GetChild(i).tag.Equals("SBDataContainer")) {
                Destroy(Container.transform.GetChild(i).gameObject);
            }
        }
    }

    public void ButtonPressNextPage() {
        if(ScoreBoarPage==NumberOfPages) {
            enableToNext = false;
        }
        if (enableToNext) {
            ScoreBoarPage++;
            if (ScoreBoarPage > 1){
                enabelToPreview = true;
            }
            
            ScoreBoarPageUI.text = "" + ScoreBoarPage + " de " + NumberOfPages;
            StartCoroutine(InitializeUserData());
            
        }
    }

    public void ButtonPressPreviewPage() {
        if (enabelToPreview) {
            ScoreBoarPage--;
            if (ScoreBoarPage == 1){
                enabelToPreview = false;
            }
            if (ScoreBoarPage<NumberOfPages) {
                enableToNext = true;
            }
            ScoreBoarPageUI.text = "" + ScoreBoarPage + " de " + NumberOfPages;
            StartCoroutine(InitializeUserData());
        }
    }

    public void OnValueChangeToogleOtherGrades()
    {
        StartCoroutine(InitializeUserData());
    }

    IEnumerator InitializeUserData(){
        float waitTime = 1f;
        yield return new WaitForSeconds(waitTime);
        List<Mdl_UserScoreBoard> myUserScores=null;
        myUserScores = firebaseOperations.GetUsersDataScoreFromDatabase(endAtPreview[ScoreBoarPage], ScoreBoarPage, PageSize);
        SpinnerContainer.SetActive(true);
        while (waitTime<=3){
            yield return new WaitForSeconds(waitTime);
            if(myUserScores!=null && myUserScores.Count>0){
                //myUserScoresPage = myUserScores;
                SpinnerContainer.SetActive(false);
                AddNewChildrensToList(myUserScores);
                //endAtUserID = myUserScores[myUserScores.Count - 1].id;
                endAtPreview.Insert(ScoreBoarPage+1, myUserScores[myUserScores.Count - 1].id);
                if(myUserScores.Count == PageSize){
                    enableToNext = true;
                }else{
                    enableToNext = false;
                }
                break;
            }else{
                waitTime++;
            }
        }
        SpinnerContainer.SetActive(false);
    }

    IEnumerator InitializeNumberOfPages(){
        float waitTime = 1f;
        yield return new WaitForSeconds(waitTime);
        List<long>  numberOfPages = firebaseOperations.GetUsersPagesNumberOfUser(PageSize);
        while (waitTime <= 3){
            yield return new WaitForSeconds(waitTime);
            if (numberOfPages != null && numberOfPages.Count > 0){
                NumberOfPages = (int)numberOfPages[0];
                for (var i = 0; i<=NumberOfPages; i++) { 
                    endAtPreview.Add("");
                }
                ScoreBoarPageUI.text = "" + ScoreBoarPage + " de " + NumberOfPages;
                StartCoroutine(InitializeUserData());
                break;
            }
            else{
                waitTime++;
            }
        }
    }

    private void AddNewChildrensToList(List<Mdl_UserScoreBoard>  myUserScores) {
        int raiting = 1;
        foreach (Mdl_UserScoreBoard userScore in myUserScores) {
            GameObject NewRowUserScore;
            if(Container.transform.childCount>2 && raiting+1 < Container.transform.childCount) {
                NewRowUserScore = Container.transform.GetChild(raiting+1).gameObject;
            }else{
                NewRowUserScore = Instantiate(DataContainerPrefab);
                NewRowUserScore.transform.SetParent(Container.transform);
                NewRowUserScore.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            }
            GameObject gm;
            gm = GetChildObject(NewRowUserScore.transform, "SBUserName");
            gm.GetComponent<Text>().text = userScore.userName;
            gm = GetChildObject(NewRowUserScore.transform, "SBUserIcon");
            if (userScore.genero == 0) {
                gm.GetComponent<Image>().sprite = BoyImage;
            } else {
                gm.GetComponent<Image>().sprite = GirlImage;
            }
            gm = GetChildObject(NewRowUserScore.transform, "SBRaiting");
            gm.GetComponent<Text>().text = "" + (raiting + PageSize * (ScoreBoarPage-1));
            raiting++;
            gm = GetChildObject(NewRowUserScore.transform, "SBStars");
            gm.GetComponent<Text>().text = "" + userScore.estrellas;
            gm = GetChildObject(NewRowUserScore.transform, "SBLevel");
            gm.GetComponent<Text>().text = "" + userScore.nivel;
            gm = GetChildObject(NewRowUserScore.transform, "SBGrade");
            gm.GetComponent<Text>().text = "" + userScore.grado;
        }
    }


    public GameObject GetChildObject(Transform parent, string _tag){
        for(int i = 0; i < parent.childCount; i++){
            Transform child = parent.GetChild(i);
            if (child.tag == _tag){
                return child.gameObject;
            }if (child.childCount > 0){
                GameObject temp= GetChildObject(child, _tag);
                if (temp!=null) {
                    return temp;
                }
            }
        }
        return null;
    }
}


