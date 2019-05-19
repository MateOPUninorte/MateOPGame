using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class Sc_ExerciseGeneratorController : MonoBehaviour
{

    public static string UrlARN = "";
    public static string MethodModelOne = "";
    public static string MethodModelTwo = "";
    public static string LOID = "LOIN";
    public static string LOIDDF = "LOINDF";
    public static string LOIDDD = "LOINDD";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static List<Mdl_Exercise> GenerateRandomExercises(int numberOfExercises,int minNumber,int maxNumber) {
        List<Mdl_Exercise> myExercises = new List<Mdl_Exercise>();
        for (var i = 1; i <= numberOfExercises; i++){
            int sum1 = Random.Range(minNumber, maxNumber);
            int sum2 = Random.Range(minNumber, maxNumber);
            string op = sum1 + "+" + sum2 + "=?";
            string ans = (sum1 + sum2) + "";
            myExercises.Add(new Mdl_Exercise(op, ans));
        }
        return myExercises;
    }

    //crear un metodo que se conecte al modelo mande los datos y regrese el performance esperado del niño
    public static List<Mdl_Exercise> GenerateExercisesFromLOPerformance(Mdl_Intensities myPerformance, int numberOfExercises) {
        List<Mdl_Exercise> myExercises = new List<Mdl_Exercise>();
        Mdl_Intensities intensities = new Mdl_Intensities();
        float totalPerformance = GetTotalPerformance(myPerformance);
        for(var i = 0; i < myPerformance.GetNumebrOfLO(); i++){
            Debug.Log(myPerformance.LoIntensities[LOID + i]);
        }
        int chosenLO=0;
        int totalEx = 0;
        float comparator = -1;
        List<int> numberPerLo = new List<int>();
         for (var i = 0; i < myPerformance.GetNumebrOfLO(); i++){
            int numberExercPerLo = (int)(numberOfExercises * myPerformance.LoIntensities[LOID + i]);
            numberPerLo.Add(numberExercPerLo);
            totalEx = totalEx + numberExercPerLo;
            if (comparator < myPerformance.LoIntensities[LOID + i]) {
                comparator = myPerformance.LoIntensities[LOID + i];
                chosenLO = i;
            }
        }

        if (numberOfExercises > totalEx){
            numberPerLo[chosenLO] = numberPerLo[chosenLO] + Mathf.Abs(numberOfExercises - totalEx);
        }else if (numberOfExercises < totalEx){
            numberPerLo[chosenLO] = numberPerLo[chosenLO] - Mathf.Abs(numberOfExercises - totalEx);
        }

        for(var i = 0; i < myPerformance.GetNumebrOfLO(); i++){
            int numberOfDF = (int)(numberPerLo[i] * myPerformance.LoIntensities[LOIDDF + i]);
            int numberOfDD = (int)(numberPerLo[i] * myPerformance.LoIntensities[LOIDDD + i]);
            if (numberOfDD + numberOfDF != numberPerLo[i]) {
                if(myPerformance.LoIntensities[LOIDDF + i]> myPerformance.LoIntensities[LOIDDD + i]) {
                    numberOfDF = numberOfDF + Mathf.Abs(numberOfDD + numberOfDF - numberPerLo[i]);
                } else {
                    numberOfDD = numberOfDD + Mathf.Abs(numberOfDD + numberOfDF - numberPerLo[i]);
                }
            }
            
            List<Mdl_Exercise> myExerciseslo = GenerateExercisesPerLO(numberOfDF, LOID + i,0);
            foreach(Mdl_Exercise ex in myExerciseslo){
                myExercises.Add(ex);
            }
            myExerciseslo = GenerateExercisesPerLO(numberOfDD, LOID + i,1);
            foreach(Mdl_Exercise ex in myExerciseslo){
                myExercises.Add(ex);
            }
        }


        return myExercises;
    }

    private static bool CanBeAddedToList(List<int> list, int newNum, int comparator) {
        int count = 0;
        foreach (int a in list) {
            if (a==newNum) {
                count++;
            }
        }
        if (count>= comparator){
            return false;
        }
        return true;
    }

    private static List<Mdl_Exercise> GenerateExercisesPerLO(int numberExerc,string loIDString, int dificulty) {
        List<Mdl_Exercise> myExercises = new List<Mdl_Exercise>();
        List<int> choosenNumbersSum1 = new List<int>();
        List<int> choosenNumbersSum2 = new List<int>();
        myExercises = GetPastExercisesFromFile(numberExerc, loIDString, dificulty);
        numberExerc = numberExerc - myExercises.Count;
        for (var i = 0; i < numberExerc; i++) {       
            int loId;
            int sumOp1=0, sumOp2=0;
            if (loIDString.Equals(LOID+"0")){
                loId = 0;
                if (dificulty == 1) {
                    do{
                        sumOp1 = Random.Range(1, 10);
                        sumOp2 = Random.Range(0, 10);
                    }while(sumOp1+sumOp2<10 || !CanBeAddedToList(choosenNumbersSum1,sumOp1,2) || !CanBeAddedToList(choosenNumbersSum2, sumOp2,2) || !CanExerciseBeAdded(myExercises,sumOp1,sumOp2));
                }else{
                    do{ 
                        sumOp1 = Random.Range(1, 10);
                        sumOp2 = Random.Range(0, 10);
                    } while ((sumOp1 + sumOp2) >= 10 || !CanBeAddedToList(choosenNumbersSum1, sumOp1,2) || !CanBeAddedToList(choosenNumbersSum2, sumOp2,2) || !CanExerciseBeAdded(myExercises, sumOp1, sumOp2));
                }
            }
            else if(loIDString.Equals(LOID+"1")){
                loId = 1;
                if (i<10) {
                    do{
                        sumOp1 = Random.Range(0, 10);
                    } while (!CanBeAddedToList(choosenNumbersSum1, sumOp1, 1));
                    sumOp2 = 10;
                }
                else{
                    do{
                        sumOp2 = Random.Range(0, 10);
                    } while (!CanBeAddedToList(choosenNumbersSum2, sumOp2, 1));
                    sumOp1 = 10;
                }

            }else if(loIDString.Equals(LOID+"2")){
                loId = 2;
                if(dificulty == 1) {
                    do{
                        sumOp1 = Random.Range(1, 10);
                        sumOp2 = Random.Range(10, 100);
                    }while(sumOp1+sumOp2<100 || !CanBeAddedToList(choosenNumbersSum1, sumOp1,2) || !CanBeAddedToList(choosenNumbersSum2, sumOp2,2) || !CanExerciseBeAdded(myExercises, sumOp1, sumOp2));
                }else{
                    do{ 
                        sumOp1 = Random.Range(1, 10);
                        sumOp2 = Random.Range(10, 100);
                    } while (sumOp1 + sumOp2 >= 100 || !CanBeAddedToList(choosenNumbersSum1, sumOp1,2) || !CanBeAddedToList(choosenNumbersSum2, sumOp2,2) || !CanExerciseBeAdded(myExercises, sumOp1, sumOp2));
                }
            }else if(loIDString.Equals(LOID+"3")){
                loId = 3;
                do{
                    sumOp1 = Random.Range(10, 100);
                } while (!CanBeAddedToList(choosenNumbersSum1, sumOp1,1));
                sumOp2 = 10;
            }
            else{
                loId = 4;
                if (dificulty == 1){
                    do{
                        sumOp1 = Random.Range(10, 100);
                        sumOp2 = Random.Range(10, 100);
                    }while(sumOp1 + sumOp2 < 100 || !CanBeAddedToList(choosenNumbersSum1, sumOp1,2) || !CanBeAddedToList(choosenNumbersSum2, sumOp2,2) || !CanExerciseBeAdded(myExercises, sumOp1, sumOp2));
                }else{
                    do{
                        sumOp1 = Random.Range(10, 100);
                        sumOp2 = Random.Range(10, 100);
                    } while (sumOp1 + sumOp2 > 100 || !CanBeAddedToList(choosenNumbersSum1, sumOp1,2) || !CanBeAddedToList(choosenNumbersSum2, sumOp2,2) || !CanExerciseBeAdded(myExercises, sumOp1, sumOp2));
                }
            }
            choosenNumbersSum1.Add(sumOp1);
            choosenNumbersSum2.Add(sumOp2);            
            string problem = sumOp1 + "+"+ sumOp2 + "=?";
            string answer = ""+(sumOp1 + sumOp2);
            int type=0;
            Mdl_Exercise oneExc = new Mdl_Exercise(loId,problem,answer,type);
            oneExc.dificultad = dificulty;
            myExercises.Add(oneExc);
        }
        return myExercises;
    }

    private static List<Mdl_Exercise> GetPastExercisesFromFile(int numberExerc, string loIDString, int dificulty) {
        List<Mdl_Exercise> myExercises = new List<Mdl_Exercise>();
        string path;
        if (Application.isEditor){
            path = Application.dataPath;
        }else {
            path = Application.persistentDataPath;
        }
        string line = null;
        if (File.Exists(path + "/WrongExercises.txt")) {        
            using (StreamReader reader = new StreamReader(path+ "/WrongExercises.txt")){
                using (StreamWriter writer = new StreamWriter(path + "/WrongExercisesTemp.txt")){
                    while((line = reader.ReadLine()) != null){
                        string[] elems = line.Split('|');
                        if (elems[0].Equals(loIDString) && elems[1].Equals(""+dificulty) && myExercises.Count < numberExerc) {
                            Mdl_Exercise previewExercise = JsonUtility.FromJson<Mdl_Exercise>(elems[2]);
                            myExercises.Add(previewExercise);
                        }else {
                            writer.WriteLine(line);
                        }                    
                    }
                }
            }
            File.Delete(path + "/WrongExercises.txt");
            File.Move(path + "/WrongExercisesTemp.txt", path + "/WrongExercises.txt");
        }
        return myExercises;
    }

    private static bool CanExerciseBeAdded(List<Mdl_Exercise> currents, int a, int b) {
        string problem = a + "+" + b + "=?";
        foreach (Mdl_Exercise ex in currents) {
            if (ex.problem.Equals(problem)) {
                return false;
            }
        }
        return true;
    }

    private static float GetTotalPerformance(Mdl_Intensities myPerformance) {
        float result = 0;
        for (var i = 0;i<myPerformance.GetNumebrOfLO(); i++) {
            result = result + myPerformance.LoIntensities[LOID + i];
        }
        return result;
    }

    public static void WriteOnFileWrongExercise(Mdl_Exercise myWrongExercise) {
        string jsonifiedExercise = JsonUtility.ToJson(myWrongExercise);
        TextWriter file;
        if(Application.isEditor){
            file = new StreamWriter(Application.dataPath + "/WrongExercises.txt", true);
        }else{
            file = new StreamWriter(Application.persistentDataPath + "/WrongExercises.txt", true);
        }

        int dificulty= GetDificultyOfExercise(myWrongExercise);
        file.WriteLine(LOID+myWrongExercise.loID+ "|"+ dificulty + "|" + jsonifiedExercise);//El salto de linea depende si el sistema operativo es o no de Unix tener en cuenta para IOS
        file.Close();
    }

    private static int GetDificultyOfExercise(Mdl_Exercise myExercise) {
        string[] problem = myExercise.problem.Split('+');
        problem[1]=problem[1].Remove(problem[1].Length - 2);
        int sumOp1 = int.Parse(problem[0]);
        int sumOp2 = int.Parse(problem[1]);
        if (myExercise.loID==0){
            if (sumOp1 + sumOp2 >= 10) {
                return 1;
            } else {
                return 0;
            }
        }else if(myExercise.loID == 1){
            return 0;
        }else if(myExercise.loID==2){
            if (sumOp1 + sumOp2 >= 100) {
                return 1;
            } else {
                return 0;
            }
        }else if(myExercise.loID == 3){
            return 0;
        }else{
            if (sumOp1 + sumOp2 >= 100) {
                return 1;
            } else {
                return 0;
            }
        }
    }


}



