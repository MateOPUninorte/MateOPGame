using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestionsType
{
    Abiertas, Cerradas
}
public class Sc_AdditionTypeManager : MonoBehaviour
{
    private QuestionsType questionsType;

    public QuestionsType QuestionsType { get => questionsType; set => questionsType = value; }
}
