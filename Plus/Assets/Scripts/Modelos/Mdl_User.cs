using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mdl_User 
{
    public string userID;
    public string nombres;
    public string correo;
    public string userName;
    public int edad;
    public int grado;
    public string escuela;
    public int tipoEscuela;
    public int genero;
    public int estrellas;
    public int nivel;
    public int sesion;
    public List<long> lo;

    public Mdl_User(){
    }

    public Mdl_User(string userID, string nombres, string correo, string userName, string escuela, int edad, int grado, int genero, int estrellas, int nivel, int sesion, int tipoEscuela) {
        this.userID = userID;
        this.nombres = nombres;
        this.correo = correo;
        this.userName = userName;
        this.escuela = escuela;
        this.edad = edad;
        this.grado = grado;
        this.genero = genero;
        this.estrellas = estrellas;
        this.nivel = nivel;
        this.sesion = sesion;
        this.tipoEscuela = tipoEscuela;
    }
    

}
