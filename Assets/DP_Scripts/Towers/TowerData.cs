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

    [Tooltip("O dano causado por esta torre.")]
    public int danoAtual = 5;

    [Tooltip("O dano que a torre causará no próximo nível.")]
    public int danoUpgrade = 2;

    [Tooltip("O custo em dinheiro para fazer upgrade desta torre.")]
    public int custoUpgrade = 10;

    [Tooltip("O nível atual da torre.")]
    public int nivel = 1;

    [Tooltip("O ícone da torre que será exibido na UI.")]
    public Sprite icon;

}
