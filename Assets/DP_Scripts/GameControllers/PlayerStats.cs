using UnityEngine;
using System.Collections.Generic;

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

    private HashSet<string> upgradesComprados = new HashSet<string>();

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

    public void TentarComprarUpgrade(string upgradeID)
    {
        if (upgradesComprados.Contains(upgradeID))
        {
            Debug.Log($"Upgrade '{upgradeID}' já foi comprado.");
            return;
        }

        int custo = 0;
        string preRequisito = null;

        // --- ÁRVORE DE HABILIDADES COMPLETA ---
        switch (upgradeID)
        {
            // ==========================================================
            // === 1. RAMO DE PODER ================================
            // ==========================================================

            // --- Caminho do Tiro Básico ---
            case "PODER_DANO_1":
                custo = 10;
                preRequisito = null;
                break;
            case "PODER_RANGE_1":
                custo = Mathf.CeilToInt(10 * 1.3f);
                preRequisito = "PODER_DANO_1";
                break;
            case "PODER_TIRO_EXPLOSIVO":
                custo = Mathf.CeilToInt(13 * 1.3f);
                preRequisito = "PODER_RANGE_1";
                break;

            // --- Caminho do Triple Shot ---
            case "PODER_PROJETIL_EXTRA_1":
                custo = 15;
                preRequisito = null;
                break;
            case "PODER_MENOS_SPREAD":
                custo = Mathf.CeilToInt(15 * 1.3f);
                preRequisito = "PODER_PROJETIL_EXTRA_1";
                break;
            case "PODER_PIERCING_1":
                custo = Mathf.CeilToInt(20 * 1.3f);
                preRequisito = "PODER_MENOS_SPREAD";
                break;
            
            // --- Caminho do Fire Rate ---
            case "PODER_FIRERATE_1":
                custo = 12;
                preRequisito = null;
                break;
            case "PODER_OVERCLOCK":
                custo = Mathf.CeilToInt(12 * 1.3f);
                preRequisito = "PODER_FIRERATE_1";
                break;
            case "PODER_FIRERATE_2":
                 custo = Mathf.CeilToInt(16 * 1.3f);
                 preRequisito = "PODER_OVERCLOCK";
                 break;

            // --- Caminho do Crítico ---
            case "PODER_CRITICO_1":
                custo = 20;
                preRequisito = null;
                break;
            case "PODER_EXECUCAO":
                custo = Mathf.CeilToInt(20 * 1.3f);
                preRequisito = "PODER_CRITICO_1";
                break;
            case "PODER_CRITICO_2":
                custo = Mathf.CeilToInt(26 * 1.3f);
                preRequisito = "PODER_EXECUCAO";
                break;

            // ==========================================================
            // === 2. RAMO DE CONTROLE =============================
            // ==========================================================

            // --- Caminho de Lentidão ---
            case "CONTROLE_LENTIDAO_1":
                custo = 15;
                preRequisito = null;
                break;
            case "CONTROLE_SUPER_LENTIDAO":
                custo = Mathf.CeilToInt(15 * 1.3f);
                preRequisito = "CONTROLE_LENTIDAO_1";
                break;
            case "CONTROLE_LENTIDAO_2":
                custo = Mathf.CeilToInt(20 * 1.3f);
                preRequisito = "CONTROLE_SUPER_LENTIDAO";
                break;

            // --- Caminho de Debuff de Ataque ---
            case "CONTROLE_ENFRAQUECER_1":
                custo = 15;
                preRequisito = null;
                break;
            case "CONTROLE_AURA_ENFRAQUECER":
                custo = Mathf.CeilToInt(15 * 1.3f);
                preRequisito = "CONTROLE_ENFRAQUECER_1";
                break;
            case "CONTROLE_AURA_MAIOR":
                custo = Mathf.CeilToInt(20 * 1.3f);
                preRequisito = "CONTROLE_AURA_ENFRAQUECER";
                break;
            
            // --- Caminho de Debuff de Defesa ---
            case "CONTROLE_CORROSAO_1":
                custo = 20;
                preRequisito = null;
                break;
            case "CONTROLE_CORROSAO_STACK":
                custo = Mathf.CeilToInt(20 * 1.3f);
                preRequisito = "CONTROLE_CORROSAO_1";
                break;

            // ==========================================================
            // === 3. RAMO TÁTICO ==================================
            // ==========================================================

            // --- Caminho de Velocidade ---
            case "TATICO_VELOCIDADE_1":
                custo = 10;
                preRequisito = null;
                break;
            case "TATICO_DASH":
                custo = Mathf.CeilToInt(10 * 1.3f);
                preRequisito = "TATICO_VELOCidade_1";
                break;
            case "TATICO_VELOCIDADE_2":
                custo = Mathf.CeilToInt(13 * 1.3f);
                preRequisito = "TATICO_DASH";
                break;
            
            // --- Caminho de Controle de Trilho ---
            case "TATICO_INVERTER_TRILHO":
                custo = 10;
                preRequisito = null;
                break;
            case "TATICO_REDUZIR_CD_INVERSAO":
                custo = Mathf.CeilToInt(10 * 1.3f);
                preRequisito = "TATICO_INVERTER_TRILHO";
                break;
            case "TATICO_IGNORAR_CD_INVERSAO":
                custo = Mathf.CeilToInt(13 * 1.3f);
                preRequisito = "TATICO_REDUZIR_CD_INVERSAO";
                break;

            // --- Caminho de Sinergia com Torre (Efeitos a implementar) ---
            case "TATICO_SINERGIA_TORRE_1":
                custo = 25;
                preRequisito = null;
                break;
            case "TATICO_BOOST_TORRE":
                custo = Mathf.CeilToInt(25 * 1.3f);
                preRequisito = "TATICO_SINERGIA_TORRE_1";
                break;
            case "TATICO_SINERGIA_TORRE_2":
                custo = Mathf.CeilToInt(33 * 1.3f);
                preRequisito = "TATICO_BOOST_TORRE";
                break;

            // --- Caminho de Mais Carrinhos (Efeitos a implementar) ---
            case "TATICO_CARRINHO_1":
                custo = 50;
                preRequisito = null;
                break;
            case "TATICO_CARRINHO_SKILL":
                custo = Mathf.CeilToInt(50 * 1.3f);
                preRequisito = "TATICO_CARRINHO_1";
                break;
            case "TATICO_CARRINHO_2":
                custo = Mathf.CeilToInt(65 * 1.3f);
                preRequisito = "TATICO_CARRINHO_SKILL";
                break;
        }

        // --- LÓGICA DE VALIDAÇÃO E COMPRA (permanece a mesma) ---

        if (custo == 0)
        {
            Debug.LogError($"ID de upgrade desconhecido: '{upgradeID}'");
            return;
        }
        
        if (preRequisito != null && !upgradesComprados.Contains(preRequisito))
        {
            Debug.Log($"Pré-requisito '{preRequisito}' para o upgrade '{upgradeID}' não foi comprado.");
            return;
        }

        if (EconomyManager.Instance.GastarDinheiro(custo))
        {
            Debug.Log($"Upgrade '{upgradeID}' comprado por {custo} moedas!");
            AplicarUpgrade(upgradeID);
            upgradesComprados.Add(upgradeID);
        }
    }

    private void AplicarUpgrade(string upgradeID)
    {
        switch (upgradeID)
        {
            // ==========================================================
            // === 1. RAMO DE PODER ================================
            // ==========================================================

            // --- Caminho do Tiro Básico ---
            case "PODER_DANO_1":
                damageMultiplier += 0.25f;
                Debug.Log("EFEITO: Dano aumentado! Novo Multiplicador: " + damageMultiplier);
                break;
            case "PODER_RANGE_1":
                rangeMultiplier += 0.10f;
                Debug.Log("EFEITO: Range aumentado! Novo Multiplicador: " + rangeMultiplier);
                break;
            case "PODER_TIRO_EXPLOSIVO":
                hasExplosiveShot = true;
                Debug.Log("EFEITO: Tiro Explosivo ativado!");
                break;

            // --- Caminho do Triple Shot ---
            case "PODER_PROJETIL_EXTRA_1":
                additionalProjectiles++;
                Debug.Log("EFEITO: +1 Tiro! Total de projéteis extras: " + additionalProjectiles);
                break;
            case "PODER_MENOS_SPREAD":
                spreadMultiplier *= 0.8f;
                Debug.Log("EFEITO: Menos Spread! Novo multiplicador de dispersão: " + spreadMultiplier);
                break;
            case "PODER_PIERCING_1":
                hasPiercingShot = true;
                pierceCount = 1;
                Debug.Log("EFEITO: Piercing Ativado! Atravessa " + pierceCount + " inimigo(s).");
                break;

            // --- Caminho do Fire Rate ---
            case "PODER_FIRERATE_1":
                fireRateMultiplier += 0.2f;
                Debug.Log("EFEITO: +20% Fire Rate! Novo multiplicador: " + fireRateMultiplier);
                break;
            case "PODER_OVERCLOCK":
                isOverclockUnlocked = true;
                Debug.Log("EFEITO: Overclock Desbloqueado! Pressione R para ativar.");
                break;
            case "PODER_FIRERATE_2":
                fireRateMultiplier += 0.5f;
                Debug.Log("EFEITO: +50% Fire Rate! Novo multiplicador: " + fireRateMultiplier);
                break;

            // --- Caminho do Crítico ---
            case "PODER_CRITICO_1":
                critChance = 0.1f;
                critMultiplier = 2f;
                Debug.Log("EFEITO: Crítico Nível 1! Chance: 10%, Dano: 2x");
                break;
            case "PODER_EXECUCAO":
                isExecuteUnlocked = true;
                Debug.Log("EFEITO: Execução Desbloqueada!");
                break;
            case "PODER_CRITICO_2":
                critChance = 0.3f;
                critMultiplier = 3f;
                Debug.Log("EFEITO: Crítico Nível 2! Chance: 30%, Dano: 3x");
                break;

            // ==========================================================
            // === 2. RAMO DE CONTROLE =============================
            // ==========================================================

            // --- Caminho de Lentidão ---
            case "CONTROLE_LENTIDAO_1":
                hasSlowShot = true;
                Debug.Log("EFEITO: Tiro com Lentidão ativado! Inimigos ficarão 25% mais lentos.");
                break;
            case "CONTROLE_SUPER_LENTIDAO":
                hasSuperSlow = true; // Habilita o bônus de dano em inimigos lentos.
                Debug.Log("EFEITO: Super Lentidão! Inimigos lentos agora recebem +10% de dano.");
                break;
            case "CONTROLE_LENTIDAO_2":
                slowAmount = 0.50f;
                Debug.Log("EFEITO: Lentidão aumentada para 50%!");
                break;

            // --- Caminho de Debuff de Ataque ---
            case "CONTROLE_ENFRAQUECER_1":
                hasWeakenShot = true;
                Debug.Log("EFEITO: Tiro Enfraquecedor ativado! Dano dos inimigos atingidos reduzido em 25%.");
                break;
            case "CONTROLE_AURA_ENFRAQUECER":
                hasWeakenAura = true;
                Debug.Log("EFEITO: Área de Enfraquecimento ativada! Tiros criarão auras no impacto.");
                break;
            case "CONTROLE_AURA_MAIOR":
                weakenAuraSizeMultiplier += 0.5f;
                Debug.Log("EFEITO: Área de Enfraquecimento aumentada!");
                break;

            // --- Caminho de Debuff de Defesa ---
            case "CONTROLE_CORROSAO_1":
                hasDefenseDownShot = true;
                Debug.Log("EFEITO: Tiro Corrosivo ativado! Inimigos atingidos recebem 10% a mais de dano.");
                break;
            case "CONTROLE_CORROSAO_STACK":
                canStackDefenseDown = true;
                defenseDownMaxStacks = 3;
                Debug.Log("EFEITO: Corrosão Acumulativa! Debuff de defesa agora acumula até 3 vezes.");
                break;

            // ==========================================================
            // === 3. RAMO TÁTICO ==================================
            // ==========================================================

            // --- Caminho de Velocidade ---
            case "TATICO_VELOCIDADE_1":
                cartSpeedMultiplier += 0.25f;
                Debug.Log("EFEITO: Velocidade do Carrinho aumentada em 25%!");
                break;
            case "TATICO_DASH":
                isDashUnlocked = true;
                Debug.Log("EFEITO: Dash Desbloqueado! Pressione Shift Esquerdo para usar.");
                break;
            case "TATICO_VELOCIDADE_2":
                cartSpeedMultiplier += 0.50f;
                Debug.Log("EFEITO: Velocidade do Carrinho aumentada em mais 50%!");
                break;

            // --- Caminho de Controle de Trilho ---
            case "TATICO_INVERTER_TRILHO":
                canChangeDirection = true;
                Debug.Log("EFEITO: Troca de Direção liberada! Pressione Q para inverter o movimento.");
                break;
            case "TATICO_REDUZIR_CD_INVERSAO":
                directionChangeCooldown *= 0.5f;
                Debug.Log("EFEITO: Cooldown da Troca de Direção reduzido!");
                break;
            case "TATICO_IGNORAR_CD_INVERSAO":
                // TODO: Implementar a lógica de ignorar um cooldown por round.
                Debug.Log("EFEITO: Ignorar Cooldown de Inversão (Lógica a ser implementada).");
                break;
                
            // --- Caminhos com lógica a ser implementada ---
            case "TATICO_SINERGIA_TORRE_1":
                 // TODO: Implementar lógica de bônus de dano para torres próximas.
                Debug.Log("EFEITO: Bônus de 10% de dano para torres próximas (Lógica a ser implementada).");
                break;
            case "TATICO_BOOST_TORRE":
                // TODO: Implementar habilidade de boost em torre.
                Debug.Log("EFEITO: Habilidade de Boost em Torre liberada (Lógica a ser implementada).");
                break;
            case "TATICO_SINERGIA_TORRE_2":
                // TODO: Aumentar o bônus de dano para 25%.
                Debug.Log("EFEITO: Bônus de dano de sinergia aumentado para 25% (Lógica a ser implementada).");
                break;
            case "TATICO_CARRINHO_1":
                // TODO: Implementar a adição de um novo carrinho.
                Debug.Log("EFEITO: Novo carrinho liberado (Lógica a ser implementada).");
                break;
            case "TATICO_CARRINHO_SKILL":
                // TODO: Implementar a skill de todos os carrinhos atirarem juntos.
                Debug.Log("EFEITO: Skill de tiro simultâneo liberada (Lógica a ser implementada).");
                break;
            case "TATICO_CARRINHO_2":
                // TODO: Implementar a adição de um segundo carrinho extra.
                Debug.Log("EFEITO: Segundo carrinho extra liberado (Lógica a ser implementada).");
                break;
        }
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

}