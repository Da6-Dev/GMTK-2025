using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Tooltip("Arraste aqui o BoxCollider2D que define os limites da área jogável.")]
    public BoxCollider2D playableArea;

    [Tooltip("Uma margem de segurança para garantir que os inimigos apareçam fora dos limites.")]
    public float offScreenBuffer = 2f;


    // --- Variáveis de estado ---
    private int currentRound = 0;
    private List<EnemyData> availableEnemies = new List<EnemyData>();
    private Bounds playableBounds;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        if (playableArea != null)
        {
            playableBounds = playableArea.bounds;
        }
        else
        {
            Debug.LogError("O BoxCollider2D 'playableArea' não foi atribuído no EnemySpawner! O spawn de inimigos não funcionará.");
            enabled = false;
            return;
        }
        
        StartCoroutine(SpawnWaveCoroutine());
    }

    private IEnumerator SpawnWaveCoroutine()
    {
        while (true)
        {
            currentRound++;
            Debug.Log($"Iniciando Round {currentRound}");
            
            spawnedEnemies.Clear();

            UpdateAvailableEnemies();
            float threatBudget = initialThreatBudget * Mathf.Pow(threatScalingFactor, currentRound - 1);

            while (threatBudget > 0 && availableEnemies.Count > 0)
            {
                List<EnemyData> affordableEnemies = new List<EnemyData>();
                foreach (var enemyData in availableEnemies)
                {
                    if (threatBudget >= enemyData.threatCost)
                    {
                        affordableEnemies.Add(enemyData);
                    }
                }

                if (affordableEnemies.Count == 0)
                {
                    break;
                }

                EnemyData enemyToSpawn = affordableEnemies[Random.Range(0, affordableEnemies.Count)];
                
                GameObject newEnemyGO = Instantiate(enemyToSpawn.enemyPrefab, GetRandomSpawnPosition(), Quaternion.identity);
                
                spawnedEnemies.Add(newEnemyGO);

                SistemaDeVida vidaInimigo = newEnemyGO.GetComponent<SistemaDeVida>();
                if(vidaInimigo != null)
                {
                    vidaInimigo.recompensaPorMorte = enemyToSpawn.moneyOnDeath;
                }
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
                yield return new WaitForSeconds(spawnInterval);
            }
            
            float roundEndTime = Time.time + roundDuration;

            while (Time.time < roundEndTime)
            {
                spawnedEnemies.RemoveAll(item => item == null);

                if (spawnedEnemies.Count == 0)
                {
                    break;
                }

                yield return new WaitForSeconds(1f);
            }
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