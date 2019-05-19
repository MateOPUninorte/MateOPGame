using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mdl_UserScoreBoard
{

    public string userName;
    public string id;
    public int grado;
    public int genero;
    public int estrellas;
    public int nivel;
    public List<long> lo;

    public Mdl_UserScoreBoard(){
    }

    public Mdl_UserScoreBoard(string id,string userName, int grado, int genero, int estrellas, int nivel){
        this.id = id;
        this.userName = userName;
        this.grado = grado;
        this.genero = genero;
        this.estrellas = estrellas;
        this.nivel = nivel;
    }
}
