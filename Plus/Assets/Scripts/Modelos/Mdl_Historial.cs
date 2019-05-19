using UnityEngine;
using UnityEditor;

public class Mdl_Historial
{
    public string loID;
    public string problema;
    public int respuesta;
    public int respuestaJugador;
    public int tiempo;
    public string userID;
    public string histID;

    public Mdl_Historial()
    {

    }

    public Mdl_Historial(string loID, string problema, int respuesta, int respuestaJugador, int tiempo, string userID, string histID)
    {
        this.loID = loID;
        this.problema = problema;
        this.respuesta = respuesta;
        this.respuestaJugador = respuestaJugador;
        this.tiempo = tiempo;
        this.userID = userID;
        this.histID = histID;
    }

}