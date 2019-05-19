using UnityEngine;
using System.Collections;

public class Mdl_Survey {

    public int edad;
    public int genero;
    public int grado;
    public string escuela;
    public string problema;
    public string respuesta;
    public string respuestaJ;
    public float tiempo;
    public int loID;
    public int tipo;
    public int answerChangedCount;
    public string answerID;
    public string userID;

    public Mdl_Survey()
    {

    }

    public Mdl_Survey(string userID, int edad, int genero, int grado, string escuela, string problema, string respuesta, string respuestaJ, float tiempo, int loID, int tipo, string answerID, int answerChangedCount)
    {
        this.edad = edad;
        this.genero = genero;
        this.grado = grado;
        this.escuela = escuela;
        this.problema = problema;
        this.respuesta = respuesta;
        this.respuestaJ = respuestaJ;
        this.tiempo = tiempo;
        this.loID = loID;
        this.tipo = tipo;
        this.answerID = answerID;
        this.userID = userID;
        this.answerChangedCount = answerChangedCount;
    }


}
