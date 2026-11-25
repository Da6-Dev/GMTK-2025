using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Skill
{
    public string nome;
    public Button botao;
    public bool desbloqueada = false;
    public Skill prerequisito;
}