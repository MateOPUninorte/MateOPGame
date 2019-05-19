using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Mdl_PerformanceVectors
{
    public int[] LO;
    public int grado;
    public int sesion;
    public int tipoEscuela;
    public float[] tiempos;
    public int[] correcto;
    public int[] dificultad;
    public int[] titubeo;

    public float[][] binLO0;
    public float[][] binLO1;
    public float[][] binLO2;
    public float[][] binLO3;
    public float[][] binLO4;
    public float[][] binPer;
    public float[][] mulLO0;
    public float[][] mulLO1;
    public float[][] mulLO2;
    public float[][] mulLO3;
    public float[][] mulLO4;
    public float[][] mulPer;

    public string JsonIntensities;

    public Mdl_PerformanceVectors(){
        binLO0 = new float[0][];
        binLO1 = new float[0][];
        binLO2 = new float[0][];
        binLO3 = new float[0][];
        binLO4 = new float[0][];
        binPer = new float[5][];
        mulLO0 = new float[0][];
        mulLO1 = new float[0][];
        mulLO2 = new float[0][];
        mulLO3 = new float[0][];
        mulLO4 = new float[0][];
        mulPer = new float[5][];
        for (var i = 0; i < 5; i++) {
            binPer[i] = new float[0];
            mulPer[i] = new float[0];
        }
        JsonIntensities = "{}";
    }

    public void  UpdatePerformanceVectorsSet(List<Mdl_Exercise> exercises,int grado, int sesion){
        LO = new int[exercises.Count];
        tiempos = new float[exercises.Count];
        correcto = new int[exercises.Count];
        dificultad = new int[exercises.Count];
        titubeo = new int[exercises.Count];
        this.grado = grado;
        this.sesion = sesion;
        int i = 0;
        foreach (Mdl_Exercise ex in exercises){
            LO[i] = ex.loID;
            tiempos[i] = ex.time;
            dificultad[i] = ex.dificultad;
            titubeo[i] = ex.answerChangedNumber;

            if(ex.answer.Equals(ex.playerAnswer)){
                correcto[i] = 1;
            }else{
                correcto[i] = 0;
            }

            i++;
        }
    }

    public static string GetJnonOf2DArray(float[][] array2D,string name) {
        string myjson= ",\""+name+"\":[";
        for (var i = 0; i< array2D.Length;i++) {
            if (array2D.Length!= 1 || array2D[i].Length!=0) { 
                myjson = myjson + "[";
                for (var j = 0; j < array2D[i].Length; j++){
                    if (j!= array2D[i].Length-1) {
                        myjson = myjson + array2D[i][j] + ",";
                    }else{
                        myjson = myjson + array2D[i][j] + "";
                    }               
                }
                if (i==array2D.Length-1) {
                    myjson = myjson + "]";
                }else {
                    myjson = myjson + "],";
                }
            }
        }
        myjson = myjson + "]";
        return myjson;
    }

    public static float[][]  Get2DArrayOfJson(string jsonRead,string name) {
        float[][] array2D=null;
        jsonRead = jsonRead.Trim();
        List<float[]> arrays = new List<float[]>();
        int startIndex = jsonRead.IndexOf(name);
        if (startIndex!=-1) { 
            startIndex = startIndex + name.Length+1;
            int index = startIndex;
            char a= jsonRead[index];
            while (a!='"' && index!=jsonRead.Length-1){           
                if (a=='['){
                    string tempArray="";
                    index++;
                    a = jsonRead[index];
                    while(a!=']'){
                        if (a!='[') {
                            tempArray = tempArray + a;
                        }                        
                        index++;
                        a = jsonRead[index];
                    }
                    float[] newArray = new float[0];
                    if (!tempArray.Equals("")) {
                        string[] splitString = tempArray.Split(',');
                        newArray = new float[splitString.Length];
                        int j = 0;
                        foreach (string elem in splitString) {
                            newArray[j] = float.Parse(elem);
                            j++;
                        }
                    }
                    arrays.Add(newArray);
                }
                index++;
                a = jsonRead[index];
            }
            array2D = new float[arrays.Count][];
            int i=0;
            foreach (float[] arrayTemp in arrays) {
                array2D[i] = arrayTemp;
                i++;
            }
        }
        return array2D;
    }

    public void SetBinloPerformanceVectors(float[][] binLO0, float[][] binLO1, float[][] binLO2, float[][] binLO3, float[][] binLO4) {
        this.binLO0 = binLO0;
        this.binLO1 = binLO1;
        this.binLO2 = binLO2;
        this.binLO3 = binLO3;
        this.binLO4 = binLO4;
    }
    public void SetBinperPerformanceVectors(float[] binLO0, float[] binLO1, float[] binLO2, float[] binLO3, float[] binLO4){
        this.binPer[0] = binLO0;
        this.binPer[1] = binLO1;
        this.binPer[2] = binLO2;
        this.binPer[3] = binLO3;
        this.binPer[4] = binLO4;
    }

    public void SetMulPerformanceVectors(float[][] mulLO0, float[][] mulLO1, float[][] mulLO2, float[][] mulLO3, float[][] mulLO4) {
        this.mulLO0 = mulLO0;
        this.mulLO1 = mulLO1;
        this.mulLO2 = mulLO2;
        this.mulLO3 = mulLO3;
        this.mulLO4 = mulLO4;
    }

    public static void WriteObjectInFile(Mdl_PerformanceVectors objectToWrite) {
        string jsonifiedExercise = JsonUtility.ToJson(objectToWrite);
        jsonifiedExercise = jsonifiedExercise.Remove(jsonifiedExercise.Length - 1);
        jsonifiedExercise = jsonifiedExercise + GetJnonOf2DArray(objectToWrite.binLO0, "binLO0");
        jsonifiedExercise = jsonifiedExercise + GetJnonOf2DArray(objectToWrite.binLO1, "binLO1");
        jsonifiedExercise = jsonifiedExercise + GetJnonOf2DArray(objectToWrite.binLO2, "binLO2");
        jsonifiedExercise = jsonifiedExercise + GetJnonOf2DArray(objectToWrite.binLO3, "binLO3");
        jsonifiedExercise = jsonifiedExercise + GetJnonOf2DArray(objectToWrite.binLO4, "binLO4");
        jsonifiedExercise = jsonifiedExercise + GetJnonOf2DArray(objectToWrite.binPer, "binPer");
        jsonifiedExercise = jsonifiedExercise + GetJnonOf2DArray(objectToWrite.mulLO0, "mulLO0");
        jsonifiedExercise = jsonifiedExercise + GetJnonOf2DArray(objectToWrite.mulLO1, "mulLO1");
        jsonifiedExercise = jsonifiedExercise + GetJnonOf2DArray(objectToWrite.mulLO2, "mulLO2");
        jsonifiedExercise = jsonifiedExercise + GetJnonOf2DArray(objectToWrite.mulLO3, "mulLO3");
        jsonifiedExercise = jsonifiedExercise + GetJnonOf2DArray(objectToWrite.mulLO4, "mulLO4");
        jsonifiedExercise = jsonifiedExercise + GetJnonOf2DArray(objectToWrite.mulPer, "mulPer");
        jsonifiedExercise = jsonifiedExercise + ",\"Intensities\":" + objectToWrite.JsonIntensities;
        jsonifiedExercise = jsonifiedExercise + "}";
        TextWriter file;
        if(Application.isEditor){
            file = new StreamWriter(Application.dataPath + "/PerformanceVectors.txt");
        }else{
            file = new StreamWriter(Application.persistentDataPath + "/PerformanceVectors.txt");
        }
        file.WriteLine(jsonifiedExercise);
        file.Close();
    }

    public static Mdl_PerformanceVectors ReadObjectFromFile(){
        Mdl_PerformanceVectors myPerformanceVector = new Mdl_PerformanceVectors();
        string path;
        if (Application.isEditor){
            path = Application.dataPath;
        }else{
            path = Application.persistentDataPath;
        }
        if (File.Exists(path + "/PerformanceVectors.txt")){
            using(StreamReader reader = new StreamReader(path+"/PerformanceVectors.txt")){
                string content = reader.ReadToEnd();
                myPerformanceVector = JsonUtility.FromJson<Mdl_PerformanceVectors>(content);
                myPerformanceVector.binLO0 = Get2DArrayOfJson(content, "binLO0");
                myPerformanceVector.binLO1 = Get2DArrayOfJson(content, "binLO1");
                myPerformanceVector.binLO2 = Get2DArrayOfJson(content, "binLO2");
                myPerformanceVector.binLO3 = Get2DArrayOfJson(content, "binLO3");
                myPerformanceVector.binLO4 = Get2DArrayOfJson(content, "binLO4");
                myPerformanceVector.binPer = Get2DArrayOfJson(content, "binPer");
                myPerformanceVector.mulLO0 = Get2DArrayOfJson(content, "mulLO0");
                myPerformanceVector.mulLO1 = Get2DArrayOfJson(content, "mulLO1");
                myPerformanceVector.mulLO2 = Get2DArrayOfJson(content, "mulLO2");
                myPerformanceVector.mulLO3 = Get2DArrayOfJson(content, "mulLO3");
                myPerformanceVector.mulLO4 = Get2DArrayOfJson(content, "mulLO4");
                myPerformanceVector.mulPer = Get2DArrayOfJson(content, "mulPer");
                myPerformanceVector.JsonIntensities = GetIntensitiesOfJson(content);
            }
        }else {
            return null;
        }
        return myPerformanceVector;
    }

     public static string ReadJsonFromFile(){
        string content = "";
        string path;
        if (Application.isEditor){
            path = Application.dataPath;
        }else{
            path = Application.persistentDataPath;
        }
        if (File.Exists(path + "/PerformanceVectors.txt")){
            using(StreamReader reader = new StreamReader(path+"/PerformanceVectors.txt")){
                content = reader.ReadToEnd();
            }
        }else {
            return null;
        }
        return content;
    }


     public static void WriteJsonInFile(string jsonifiedExercise){
        TextWriter file;
        if(Application.isEditor){
            file = new StreamWriter(Application.dataPath + "/PerformanceVectors.txt");
        }else{
            file = new StreamWriter(Application.persistentDataPath + "/PerformanceVectors.txt");
        }
        file.WriteLine(jsonifiedExercise);
        file.Close();
    }

    public static string GetIntensitiesOfJson(string jsonRead){
        string name = "Intensities";
        string content = "";
        jsonRead = jsonRead.Trim();
        int startIndex = jsonRead.IndexOf(name);
        if (startIndex != -1){
            startIndex = startIndex + name.Length + 1;
            int index = startIndex;
            char a = jsonRead[index];
            while (index != jsonRead.Length - 1) {
                if (a=='{') {
                    while (a!='}'){
                        content=content+a;
                        index++;
                        a = jsonRead[index];
                    }
                    content = content + a;
                    break;
                }
                index++;
                a = jsonRead[index];
            }
            
        }
        return content;
    }

}
