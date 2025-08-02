using UnityEngine;

public class TowerData : MonoBehaviour
{
    [Header("Configurações da Torre")]
    [Tooltip("O nome da torre que será exibido na UI.")]
    public string nomeDaTorre = "Torre Padrão";
    [Tooltip("O custo em dinheiro para construir esta torre.")]
    public int custoDeConstrucao = 15;

    [Tooltip("Uma breve descrição da torre.")]
    [TextArea(3, 5)]
    public string descricao = "Descrição padrão da torre.";

}
