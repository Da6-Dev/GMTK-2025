using UnityEngine;

public class PlayerShooting : MonoBehaviour // NOME DA CLASSE MUDOU
{
    [Header("Referências")]
    [Tooltip("Array com 8 sprites para a mira. 0=Direita, e segue em sentido horário.")]
    public Sprite[] spritesDeMira = new Sprite[8];
    [Tooltip("O objeto (bala) que será disparado.")]
    public GameObject projetilPrefab;
    [Tooltip("O ponto de origem de onde os projéteis são disparados.")]
    public Transform pontoDeTiro;

    // --- VARIÁVEIS DE STATS REMOVIDAS DAQUI ---
    // O intervaloDeTiro, dispersaoMaxima, etc., foram removidos.
    // Agora eles serão lidos do PlayerStats.

    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private float angulo;
    private float proximoTiro;
    private float dispersaoAtual;

    // NOVO: Referência para o nosso script de stats
    private PlayerStats stats;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;

        // NOVO: Pegamos a instância do PlayerStats no início.
        stats = PlayerStats.Instance;
        if (stats == null)
        {
            Debug.LogError("ERRO: PlayerStats.Instance não foi encontrado na cena!");
            this.enabled = false; // Desativa o script se o cérebro não for encontrado.
        }
    }

    void Update()
    {
        HandleAim();
        HandleShooting();

        // NOVO: Verifica se a tecla para ativar o Overclock foi pressionada
        if (Input.GetKeyDown(KeyCode.R)) // Usando a tecla R como exemplo
        {
            stats.ActivateOverclock();
        }
    }

    void HandleAim()
    {
        Vector2 posMouseNoMundo = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direcaoDaMira = (posMouseNoMundo - (Vector2)transform.position).normalized;
        angulo = Mathf.Atan2(direcaoDaMira.y, direcaoDaMira.x) * Mathf.Rad2Deg;
        int indexDoSprite = GetIndexPorAngulo(angulo);
        spriteRenderer.sprite = spritesDeMira[indexDoSprite];
    }

    void HandleShooting()
    {
        if (Input.GetMouseButton(0))
        {
            // MODIFICADO: Multiplicamos pelo spreadMultiplier para reduzir a dispersão com o upgrade.
            float maxSpreadFinal = stats.maxSpread * stats.spreadMultiplier;
            float increaseRateFinal = stats.spreadIncreaseRate * stats.spreadMultiplier;

            dispersaoAtual = Mathf.MoveTowards(dispersaoAtual, maxSpreadFinal, increaseRateFinal * Time.deltaTime);

            if (Time.time > proximoTiro)
            {
                proximoTiro = Time.time + stats.CurrentFireRate;
                Shoot();
            }
        }
        else
        {
            dispersaoAtual = Mathf.MoveTowards(dispersaoAtual, 0f, stats.spreadRecoveryRate * Time.deltaTime);
        }
    }

    void Shoot()
    {
        if (projetilPrefab == null || pontoDeTiro == null) return;

        int totalProjectiles = 1 + stats.additionalProjectiles;
        float spreadAngle = 10f; // Ângulo em graus entre os projéteis extras

        for (int i = 0; i < totalProjectiles; i++)
        {
            float angleOffset;
            if (totalProjectiles > 1)
            {
                // Calcula o ângulo para cada projétil para que fiquem simétricos
                angleOffset = (i - (totalProjectiles - 1) / 2.0f) * spreadAngle;
            }
            else
            {
                angleOffset = 0; // Sem offset se for um tiro só
            }

            // Adiciona a dispersão normal da arma (spray)
            float desvio = Random.Range(-dispersaoAtual, dispersaoAtual);

            // Calcula o ângulo final do tiro
            float anguloDoTiro = angulo + angleOffset + desvio;
            Quaternion rotacaoDoTiro = Quaternion.Euler(0, 0, anguloDoTiro);

            GameObject projetilObj = Instantiate(projetilPrefab, pontoDeTiro.position, rotacaoDoTiro);
            ProjectileController projController = projetilObj.GetComponent<ProjectileController>();

            if (projController != null)
            {
                // O Initialize agora precisa do pierceCount
                projController.Initialize(
                    stats.CurrentDamage,
                    stats.baseProjectileSpeed,
                    stats.CurrentProjectileLifetime,
                    stats.CurrentProjectileSize,
                    stats.hasExplosiveShot,
                    stats.hasPiercingShot,
                    stats.explosionRadius,
                    stats.explosionDamage,
                    stats.pierceCount,
                    stats.critChance,
                    stats.critMultiplier,
                    stats.isExecuteUnlocked,
                    stats.executeHealthThreshold,
                    stats.executeChance,
                    stats.hasSlowShot,
                    stats.slowAmount,
                    stats.slowDuration,
                    stats.hasSuperSlow ? stats.slowDamageMultiplier : 1f,
                    stats.hasWeakenShot,
                    stats.weakenAmount,
                    stats.weakenDuration,
                    stats.hasWeakenAura,
                    stats.weakenAuraLifetime,
                    stats.weakenAuraPrefab,
                    stats.weakenAuraSizeMultiplier,
                    stats.hasDefenseDownShot,
                    stats.defenseDownAmount,
                    stats.defenseDownDuration,
                    stats.defenseDownMaxStacks
                );
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

        return 0;
    }
}