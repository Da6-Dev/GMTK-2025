using UnityEngine;

// Este script será o "cérebro" que armazena todos os atributos e upgrades do jogador.
public class PlayerStats : MonoBehaviour
{
    #region Singleton
    // O padrão Singleton garante que sempre teremos uma e apenas uma instância deste script.
    // Qualquer outro script pode acessá-lo facilmente chamando PlayerStats.Instance
    public static PlayerStats Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Opcional: descomente se o jogador persistir entre as cenas.
        }
    }
    #endregion

    [Header("Atributos Base - Poder")]
    public float baseDamage = 25f;
    public float baseFireRate = 0.1f; // Corresponde ao "intervaloDeTiro"
    public float baseProjectileSpeed = 20f;
    public float baseProjectileLifetime = 2f; // Usado para calcular o range
    public float baseProjectileSize = 1f; // Multiplicador (1 = tamanho normal)

    [Header("Modificadores de Upgrade - Poder")]
    // Estes valores serão alterados pela árvore de talentos
    public float damageMultiplier = 1f;
    public float fireRateMultiplier = 1f;
    public float rangeMultiplier = 1f;
    public float projectileSizeMultiplier = 1f;
    public int additionalProjectiles = 0;
    public float spreadMultiplier = 1f;
    public int pierceCount = 0;

    [Header("Upgrades Ativados (Booleanos)")]
    // Flags que ativam comportamentos especiais
    public bool hasExplosiveShot = false;
    public bool hasPiercingShot = false;
    public bool hasSlowShot = false;
    public bool hasSuperSlow = false;
    public bool hasWeakenShot = false;
    public bool hasWeakenAura = false;
    public bool hasDefenseDownShot = false;
    public bool canStackDefenseDown = false;

    [Header("Atributos Base - Dispersão")]
    public float maxSpread = 15f;
    public float spreadIncreaseRate = 20f;
    public float spreadRecoveryRate = 30f;

    [Header("Atributos Base - Efeitos")]
    public float explosionRadius = 3f;
    public float explosionDamage = 15f; // Dano fixo da explosão, pode ser % do dano do projétil se preferir.

    [Header("Atributos - Overclock")]
    public bool isOverclockUnlocked = false;
    public float overclockMultiplier = 2f; // Dobra a cadência de tiro atual
    public float overclockDuration = 5f;   // Duração do efeito em segundos
    public float overclockCooldown = 20f;
    public bool IsOverclockActive { get; private set; }
    public bool IsOverclockOnCooldown { get; private set; }

    [Header("Atributos - Crítico")]
    public float critChance = 0f;
    public float critMultiplier = 2f; // O dano crítico padrão é 2x

    [Header("Atributos - Execução")]
    public bool isExecuteUnlocked = false;
    public float executeHealthThreshold = 0.2f; // 20% de vida
    public float executeChance = 0.1f;

    [Header("Atributos - Controle")]
    public float slowAmount = 0.25f; // 25% de lentidão
    public float slowDuration = 3f;
    public float slowDamageMultiplier = 1.1f;
    public float weakenAmount = 0.25f;
    public float weakenDuration = 4f;
    public float weakenAuraLifetime = 5f;
    public float weakenAuraSizeMultiplier = 1f;
    public float defenseDownAmount = 0.1f;
    public float defenseDownDuration = 5f;
    public int defenseDownMaxStacks = 1;

    [Header("Atributos - Tático (Carrinho)")]
    public float cartSpeedMultiplier = 1f;
    public bool canChangeDirection = false;

    [Header("Atributos - Controle de Trilho")]
    public float directionChangeCooldown = 2f;

    [Header("Atributos - Dash")]
    public bool isDashUnlocked = false;
    public float dashSpeed = 40f;
    public float dashDuration = 0.25f;
    public float dashCooldown = 4f;

    [Header("Referências de Prefabs")]
    [Tooltip("Arraste o prefab da Aura de Enfraquecimento para este campo.")]
    public GameObject weakenAuraPrefab;

    // --- PROPRIEDADES CALCULADAS ---
    // Outros scripts usarão estas propriedades para obter os valores finais.
    // Ex: O dano final é o dano base multiplicado pelos upgrades.
    public float CurrentDamage { get { return baseDamage * damageMultiplier; } }
    public float CurrentFireRate
    {
        get
        {
            // Começa com o multiplicador base
            float finalMultiplier = fireRateMultiplier;

            // Se o Overclock estiver ativo, aplica seu bônus
            if (IsOverclockActive)
            {
                finalMultiplier *= overclockMultiplier;
            }

            // Lembra? Cadência maior = intervalo menor, por isso dividimos.
            return baseFireRate / finalMultiplier;
        }
    }
    public float CurrentProjectileLifetime { get { return baseProjectileLifetime * rangeMultiplier; } }
    public float CurrentProjectileSize { get { return baseProjectileSize * projectileSizeMultiplier; } }

    public bool IsDashing { get; private set; }
    public bool IsDashOnCooldown { get; private set; }
    public bool IsDirectionChangeOnCooldown { get; private set; }

    public void Upgrade_TiroMelhorado()
    {
        // Aumenta o multiplicador de dano em 25% (0.25)
        damageMultiplier += 0.25f;
        Debug.Log("UPGRADE: Tiro Melhorado! Novo Multiplicador de Dano: " + damageMultiplier);
    }

    public void Upgrade_AumentarRange()
    {
        // Aumenta o multiplicador de range em 10% (0.10)
        rangeMultiplier += 0.10f;
        Debug.Log("UPGRADE: Range Aumentado! Novo Multiplicador de Range: " + rangeMultiplier);
    }

    public void Upgrade_TiroExplosivo()
    {
        // Ativa a flag do tiro explosivo
        hasExplosiveShot = true;
        Debug.Log("UPGRADE: Tiro Explosivo ativado!");
    }

    public void Upgrade_AdicionarTiro()
    {
        additionalProjectiles++;
        Debug.Log("UPGRADE: +1 Tiro! Total de projéteis extras: " + additionalProjectiles);
    }

    public void Upgrade_ReduzirSpread()
    {
        // Reduz a dispersão em 20%
        spreadMultiplier *= 0.8f;
        Debug.Log("UPGRADE: Menos Spread! Novo multiplicador de dispersão: " + spreadMultiplier);
    }

    public void Upgrade_AdicionarPiercing()
    {
        hasPiercingShot = true;
        // O upgrade diz para atravessar 1 inimigo
        pierceCount = 1;
        Debug.Log("UPGRADE: Piercing Ativado! Atravessa " + pierceCount + " inimigo(s).");
    }

    public void Upgrade_AumentarFireRate20()
    {
        // Aumenta a cadência em 20%. Somamos 0.2 ao multiplicador.
        fireRateMultiplier += 0.2f;
        Debug.Log("UPGRADE: +20% Fire Rate! Novo multiplicador: " + fireRateMultiplier);
    }

    public void Upgrade_AumentarFireRate50()
    {
        // Aumenta a cadência em 50%. Somamos 0.5 ao multiplicador.
        fireRateMultiplier += 0.5f;
        Debug.Log("UPGRADE: +50% Fire Rate! Novo multiplicador: " + fireRateMultiplier);
    }

    public void Upgrade_UnlockOverclock()
    {
        isOverclockUnlocked = true;
        Debug.Log("UPGRADE: Overclock Desbloqueado! Pressione R para ativar.");
    }

    // Método chamado pelo jogador para tentar ativar a habilidade
    public void ActivateOverclock()
    {
        // Só ativa se estiver desbloqueado E não estiver em recarga
        if (isOverclockUnlocked && !IsOverclockOnCooldown)
        {
            // Inicia a corrotina que controla o tempo da habilidade
            StartCoroutine(OverclockCoroutine());
        }
        else if (IsOverclockOnCooldown)
        {
            Debug.Log("Overclock está em cooldown!");
        }
    }

    // Corrotina para gerenciar os tempos de Overclock
    private System.Collections.IEnumerator OverclockCoroutine()
    {
        Debug.Log("OVERCLOCK ATIVADO!");
        IsOverclockActive = true;
        IsOverclockOnCooldown = true; // Já entra em cooldown para não poder reativar

        // Espera a duração do efeito
        yield return new WaitForSeconds(overclockDuration);

        Debug.Log("Overclock terminou.");
        IsOverclockActive = false;

        // Espera o tempo de recarga
        yield return new WaitForSeconds(overclockCooldown);

        Debug.Log("Overclock pronto para usar novamente!");
        IsOverclockOnCooldown = false;
    }

    public void Upgrade_CritChance10()
    {
        critChance = 0.1f;
        critMultiplier = 2f;
        Debug.Log("UPGRADE: Crítico Nível 1! Chance: 10%, Dano: 2x");
    }

    public void Upgrade_UnlockExecute()
    {
        isExecuteUnlocked = true;
        Debug.Log("UPGRADE: Execução Desbloqueada!");
    }

    public void Upgrade_CritChance30()
    {
        critChance = 0.3f;
        critMultiplier = 3f;
        Debug.Log("UPGRADE: Crítico Nível 2! Chance: 30%, Dano: 3x");
    }

    public void Upgrade_UnlockSlowShot()
    {
        hasSlowShot = true;
        Debug.Log("UPGRADE: Tiro com Lentidão ativado! Inimigos ficarão 25% mais lentos.");
    }

    public void Upgrade_SuperSlow()
    {
        // É uma boa prática verificar se a habilidade anterior foi desbloqueada
        if (!hasSlowShot)
        {
            Debug.LogWarning("É necessário desbloquear o 'Tiro com Lentidão' primeiro!");
            return;
        }

        hasSuperSlow = true;
        slowAmount = 0.50f; // Aumenta a lentidão para 50%
        Debug.Log("UPGRADE: Super Lentidão! Inimigos ficam 50% mais lentos e recebem +10% de dano.");
    }

    public void Upgrade_UnlockWeakenShot()
    {
        hasWeakenShot = true;
        Debug.Log("UPGRADE: Tiro Enfraquecedor ativado! Dano dos inimigos atingidos reduzido em 25%.");
    }

    public void Upgrade_UnlockWeakenAura()
    {
        if (!hasWeakenShot)
        {
            Debug.LogWarning("É preciso desbloquear o 'Tiro Enfraquecedor' primeiro!");
            return;
        }
        hasWeakenAura = true;
        Debug.Log("UPGRADE: Área de Enfraquecimento ativada! Tiros criarão auras no impacto.");
    }

    public void Upgrade_IncreaseAuraSize()
    {
        if (!hasWeakenAura)
        {
            Debug.LogWarning("É preciso desbloquear a 'Área de Enfraquecimento' primeiro!");
            return;
        }
        // Aumenta o tamanho em 50%
        weakenAuraSizeMultiplier += 0.5f;
        Debug.Log("UPGRADE: Área de Enfraquecimento aumentada! Novo multiplicador de tamanho: " + weakenAuraSizeMultiplier);
    }

    public void Upgrade_UnlockDefenseDown()
    {
        hasDefenseDownShot = true;
        Debug.Log("UPGRADE: Tiro Corrosivo ativado! Inimigos atingidos recebem 10% a mais de dano.");
    }

    public void Upgrade_UnlockStackingDefenseDown()
    {
        if (!hasDefenseDownShot)
        {
            Debug.LogWarning("É preciso desbloquear o 'Tiro Corrosivo' primeiro!");
            return;
        }
        canStackDefenseDown = true;
        defenseDownMaxStacks = 3;
        Debug.Log("UPGRADE: Corrosão Acumulativa! Debuff de defesa agora acumula até 3 vezes.");
    }

    public void Upgrade_IncreaseCartSpeed()
    {
        // Aumenta a velocidade em 25%
        cartSpeedMultiplier += 0.25f;
        Debug.Log("UPGRADE TÁTICO: Velocidade do Carrinho aumentada! Novo multiplicador: " + cartSpeedMultiplier);
    }

    public void Upgrade_UnlockDash()
    {
        isDashUnlocked = true;
        Debug.Log("UPGRADE TÁTICO: Dash Desbloqueado! Pressione Shift Esquerdo para usar.");
    }

    public void ActivateDash()
    {
        // Só ativa se estiver desbloqueado E não estiver em uso ou em recarga
        if (isDashUnlocked && !IsDashing && !IsDashOnCooldown)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private System.Collections.IEnumerator DashCoroutine()
    {
        Debug.Log("DASH!");
        IsDashing = true;
        IsDashOnCooldown = true;

        yield return new WaitForSeconds(dashDuration);
        IsDashing = false;

        Debug.Log("Dash terminou. Entrou em cooldown.");
        float cooldownRemaining = dashCooldown - dashDuration;
        yield return new WaitForSeconds(cooldownRemaining);

        IsDashOnCooldown = false;
        Debug.Log("Dash pronto para usar novamente!");
    }

    public void Upgrade_IncreaseCartSpeed50()
    {
        // Adiciona mais 50% ao multiplicador de velocidade.
        // Se o jogador já tinha +25%, o total será +75%.
        cartSpeedMultiplier += 0.50f;
        Debug.Log("UPGRADE TÁTICO: Super Velocidade do Carrinho! Novo multiplicador: " + cartSpeedMultiplier);
    }

    public void Upgrade_UnlockDirectionChange()
    {
        canChangeDirection = true;
        Debug.Log("UPGRADE TÁTICO: Troca de Direção liberada! Pressione Q para inverter o movimento.");
    }

    public void TriggerDirectionChangeCooldown()
    {
        // Só inicia a corrotina se não houver uma recarga em andamento
        if (!IsDirectionChangeOnCooldown)
        {
            StartCoroutine(DirectionChangeCooldownCoroutine());
        }
    }

    private System.Collections.IEnumerator DirectionChangeCooldownCoroutine()
    {
        IsDirectionChangeOnCooldown = true;
        Debug.Log("Troca de direção entrou em cooldown.");
        yield return new WaitForSeconds(directionChangeCooldown);
        IsDirectionChangeOnCooldown = false;
        Debug.Log("Troca de direção pronta novamente.");
    }

    public void Upgrade_ReduceDirectionChangeCooldown()
    {
        // Verifica se a habilidade de trocar de direção já foi liberada
        if (!canChangeDirection)
        {
            Debug.LogWarning("É preciso desbloquear a 'Troca de Direção' primeiro!");
            return;
        }

        // Reduz o cooldown pela metade (multiplica por 0.5)
        directionChangeCooldown *= 0.5f;
        Debug.Log("UPGRADE TÁTICO: Cooldown da Troca de Direção reduzido! Novo tempo: " + directionChangeCooldown + "s.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V)) { Upgrade_TiroMelhorado(); }
        if (Input.GetKeyDown(KeyCode.B)) { Upgrade_AdicionarTiro(); }
        if (Input.GetKeyDown(KeyCode.J)) { Upgrade_UnlockDash(); }
    }
}