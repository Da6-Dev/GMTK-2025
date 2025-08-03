using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TowerController : MonoBehaviour
{
    [Header("Atributos da Torre")]
    public float range = 15f;
    public float timeBetweenAttacks = 1f;
    public TowerData towerData; // Referência ao scriptable object com os dados da torre
    public int damage = 5; // Dano da torre, pode ser ajustado no TowerData

    [Header("Configurações Adicionais")]
    [Tooltip("A Layer onde os inimigos se encontram. Precisa ser configurado no Unity.")]
    public LayerMask enemyLayer;

    [Header("Referências")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    [Tooltip("Array com 8 sprites. 0=Direita, e segue em sentido horário.")]
    public Sprite[] directionSprites = new Sprite[8];

    [Header("Áudio")]
    public AudioClip somTiro;
    private AudioSource audioSource;

    private List<Transform> enemiesInRange = new List<Transform>();
    private Transform currentTarget;
    private float attackCooldown = 0f;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        towerData = GetComponent<TowerData>();
        if (towerData != null)
        {
            damage = towerData.danoAtual;
        }
        else
        {
            Debug.LogError("TowerData não está configurado no prefab da torre!");
            this.enabled = false; // Desabilita o script se TowerData não estiver configurado
        }
        // Pega o componente SpriteRenderer da própria torre
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("ERRO: O prefab da torre precisa de um componente SpriteRenderer!");
            this.enabled = false;
        }

        GetComponent<CircleCollider2D>().radius = range;

        // Adiciona um AudioSource se não houver um
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = somTiro;
    }

    void Update()
    {
        // Limpa a lista de alvos que não existem mais
        enemiesInRange.RemoveAll(item => item == null);

        // Se o alvo atual foi destruído ou saiu do alcance, perde o foco
        if (currentTarget != null && !enemiesInRange.Contains(currentTarget))
        {
            currentTarget = null;
        }

        // Se a torre está ociosa (sem alvo), ela procura por um
        if (currentTarget == null)
        {
            // Primeiro, tenta encontrar o mais próximo na lista atual
            FindClosestEnemy();

            // Se AINDA ASSIM não achou (lista pode estar vazia ou o inimigo spawnou dentro)
            // fazemos uma varredura ativa
            if(currentTarget == null)
            {
                ScanForNewEnemies();
                // Tenta encontrar o mais próximo de novo com a lista potencialmente atualizada
                FindClosestEnemy();
            }
        }
        
        // Se, depois de tudo, um alvo foi encontrado, ataca
        if (currentTarget != null)
        {
            AimAtTarget();
            attackCooldown -= Time.deltaTime;

            if (attackCooldown <= 0f)
            {
                Shoot();
                attackCooldown = timeBetweenAttacks;
            }
        }
    }

    void FindClosestEnemy()
    {
        Transform bestTarget = null;
        float closestDistance = float.MaxValue;

        foreach (Transform enemy in enemiesInRange)
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                bestTarget = enemy;
            }
        }
        currentTarget = bestTarget;
    }

    void ScanForNewEnemies()
    {
        // Dispara um círculo invisível e coleta tudo que estiver na "enemyLayer"
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, enemyLayer);
        foreach (var hit in hits)
        {
            // Se encontrou um inimigo que não estava na nossa lista, adiciona ele
            if (!enemiesInRange.Contains(hit.transform))
            {
                enemiesInRange.Add(hit.transform);
            }
        }
    }

    private void AimAtTarget()
    {
        // Calcula a direção e o ângulo para o alvo
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Usa o ângulo para obter o índice do sprite correto (de 0 a 7)
        int spriteIndex = GetIndexPorAngulo(angle);

        // Atualiza o sprite da torre
        spriteRenderer.sprite = directionSprites[spriteIndex];
    }

    private void Shoot()
    {
        GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        TowerProjectile projectile = projectileGO.GetComponent<TowerProjectile>();

        if (projectile != null)
        {
            projectile.Initialize(currentTarget, damage);
        }

        // Reproduz o som do tiro
        audioSource.Play();
    }

    // Detecta inimigos entrando no alcance
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (!enemiesInRange.Contains(other.transform))
            {
                enemiesInRange.Add(other.transform);
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (enemiesInRange.Contains(other.transform))
            {
                enemiesInRange.Remove(other.transform);
            }
            if(other.transform == currentTarget)
            {
                currentTarget = null;
            }
        }
    }
    private int GetIndexPorAngulo(float angulo)
    {
        if (angulo < 0) angulo += 360;

        if (angulo >= 337.5 || angulo < 22.5) return 0; // Direita
        if (angulo >= 22.5 && angulo < 67.5) return 1;  // Cima-Direita
        if (angulo >= 67.5 && angulo < 112.5) return 2; // Cima
        if (angulo >= 112.5 && angulo < 157.5) return 3; // Cima-Esquerda
        if (angulo >= 157.5 && angulo < 202.5) return 4; // Esquerda
        if (angulo >= 202.5 && angulo < 247.5) return 5; // Baixo-Esquerda
        if (angulo >= 247.5 && angulo < 292.5) return 6; // Baixo
        if (angulo >= 292.5 && angulo < 337.5) return 7; // Baixo-Direita

        return 0; // Padrão
    }
}