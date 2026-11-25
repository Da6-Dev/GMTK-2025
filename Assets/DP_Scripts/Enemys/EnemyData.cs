using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Data", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Tooltip("O prefab do inimigo que será instanciado.")]
    public GameObject enemyPrefab;

    [Tooltip("O 'custo' para spawnar este inimigo. Usado no sistema de orçamento de ameaça.")]
    public int threatCost = 1;

    [Tooltip("A partir de qual round este inimigo começa a aparecer.")]
    public int minRoundToSpawn = 1;

    [Tooltip("A quantidade de dinheiro que este inimigo concede ao ser derrotado.")]
    public int moneyOnDeath = 1;
}