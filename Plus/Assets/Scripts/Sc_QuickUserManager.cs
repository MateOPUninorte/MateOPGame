using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_QuickUserManager : MonoBehaviour
{
    public static Sc_QuickUserManager myQuickUserManager;
    private string id;
    private int edad;
    private int grado;
    private string escuela;
    private int genero;


    public int Edad { get => edad; set => edad = value; }
    public int Grado { get => grado; set => grado = value; }
    public string Escuela { get => escuela; set => escuela = value; }
    public int Genero { get => genero; set => genero = value; }
    public string Id { get => id; set => id = value; }

    void Awake()
    {
        if (myQuickUserManager == null)
        {
            DontDestroyOnLoad(this);
            myQuickUserManager = this;
        }
        else if (myQuickUserManager == this)
        {
            Destroy(gameObject);
        }
    }
}
