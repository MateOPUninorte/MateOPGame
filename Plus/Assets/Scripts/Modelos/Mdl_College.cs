using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mdl_College{
    public string escuelaID;
    public string nombre;
    public int tipo;

    public Mdl_College(string escuelaID, string nombre,int tipo)
    {
        this.escuelaID = escuelaID;
        this.nombre = nombre;
        this.tipo = tipo;
    }

    public Mdl_College()
    {

    }
}
