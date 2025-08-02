using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// REMOVIDO: using Cinemachine; // Não precisamos mais desta linha

public class EnemySpawner : MonoBehaviour
{
    [Header("Configuração Geral")]
    [Tooltip("Lista com todos os tipos de inimigos possíveis no jogo (assets de EnemyData).")]
    public List<EnemyData> allEnemyTypes = new List<EnemyData>();

    [Tooltip("O tempo em segundos entre cada spawn de inimigo dentro de um mesmo round.")]
    public float spawnInterval = 0.5f;

    [Header("Balanceamento dos Rounds")]
    [Tooltip("O orçamento de ameaça inicial para o Round 1.")]
    public float initialThreatBudget = 20f;

    [Tooltip("O fator pelo qual o orçamento de ameaça aumenta a cada round. (Ex: 1.5 para 50% de aumento).")]
    public float threatScalingFactor = 1.5f;

    [Tooltip("Duração base de um round em segundos.")]
    public float roundDuration = 30f;

    [Header("Área de Spawn")]
    [Tooltip("O centro da área de spawn (usado para direcionar os inimigos).")]
    public Transform spawnCenter;

    // ALTERADO: A referência agora é diretamente para o BoxCollider2D que define a área jogável.
    [Tooltip("Arraste aqui o BoxCollider2D que define os limites da área jogável.")]
    public BoxCollider2D playableArea; 

    [Tooltip("Uma margem de segurança para garantir que os inimigos apareçam fora dos limites.")]
    public float offScreenBuffer = 2f;


    // --- Variáveis de estado ---
    private int currentRound = 0;
    private List<EnemyData> availableEnemies = new List<EnemyData>();
    private Bounds playableBounds; // Nome da variável alterado para maior clareza

    void Start()
    {
        // ALTERADO: A lógica agora pega os limites diretamente do BoxCollider2D.
        if (playableArea != null)
        {
            playableBounds = playableArea.bounds;
        }
        else
        {
            Debug.LogError("O BoxCollider2D 'playableArea' não foi atribuído no EnemySpawner! O spawn de inimigos não funcionará.");
            enabled = false; // Desabilita o script para evitar erros
            return;
        }
        
        StartCoroutine(SpawnWaveCoroutine());
    }

    // O resto do script (SpawnWaveCoroutine, UpdateAvailableEnemies, etc.) continua exatamente igual...

    private IEnumerator SpawnWaveCoroutine()
    {
        while (true)
        {
            currentRound++;
            Debug.Log($"Iniciando Round {currentRound}");
            UpdateAvailableEnemies();
            float threatBudget = initialThreatBudget * Mathf.Pow(threatScalingFactor, currentRound - 1);
            Debug.Log($"Orçamento de ameaça para o round: {threatBudget}");

            while (threatBudget > 0 && availableEnemies.Count > 0)
            {
                EnemyData enemyToSpawn = GetRandomAvailableEnemy();
                if (enemyToSpawn != null && threatBudget >= enemyToSpawn.threatCost)
                {
                    GameObject newEnemyGO = Instantiate(enemyToSpawn.enemyPrefab, GetRandomSpawnPosition(), Quaternion.identity);
                    EnemyMovement enemyMovement = newEnemyGO.GetComponent<EnemyMovement>();
                    if (enemyMovement != null)
                    {
                        enemyMovement.alvoObjeto = spawnCenter.gameObject;
                    }
                    else
                    {
                        Debug.LogWarning($"O prefab do inimigo '{enemyToSpawn.name}' não possui o componente EnemyMovement!");
                    }
                    threatBudget -= enemyToSpawn.threatCost;
                }
                yield return new WaitForSeconds(spawnInterval);
            }
            
            Debug.Log($"Round {currentRound} concluído. Aguardando para o próximo.");
            yield return new WaitForSeconds(roundDuration);
        }
    }

    private void UpdateAvailableEnemies()
    {
        availableEnemies.Clear();
        foreach (var enemyData in allEnemyTypes)
        {
            if (currentRound >= enemyData.minRoundToSpawn)
            {
                availableEnemies.Add(enemyData);
            }
        }
    }

    private EnemyData GetRandomAvailableEnemy()
    {
        if (availableEnemies.Count == 0) return null;
        return availableEnemies[Random.Range(0, availableEnemies.Count)];
    }

    // Este método não precisa de nenhuma alteração, pois ele já usa a variável 'Bounds'
    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 spawnPosition = Vector3.zero;
        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0: // Baixo
                spawnPosition = new Vector3(Random.Range(playableBounds.min.x, playableBounds.max.x), playableBounds.min.y - offScreenBuffer, 0);
                break;
            case 1: // Topo
                spawnPosition = new Vector3(Random.Range(playableBounds.min.x, playableBounds.max.x), playableBounds.max.y + offScreenBuffer, 0);
                break;
            case 2: // Esquerda
                spawnPosition = new Vector3(playableBounds.min.x - offScreenBuffer, Random.Range(playableBounds.min.y, playableBounds.max.y), 0);
                break;
            case 3: // Direita
                spawnPosition = new Vector3(playableBounds.max.x + offScreenBuffer, Random.Range(playableBounds.min.y, playableBounds.max.y), 0);
                break;
        }
        return spawnPosition;
    }
}