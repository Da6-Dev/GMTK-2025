using UnityEngine;
using System.Collections.Generic;
using System.Linq;



// Esta classe vai gerenciar todos os debuffs em um inimigo.
public class StatusEffectController : MonoBehaviour
{
    // Uma lista de todos os efeitos de status ativos neste inimigo.
    private List<StatusEffect> activeEffects = new List<StatusEffect>();

    // Referência para o script de movimento do inimigo, que será afetado.
    private EnemyMovement enemyMovement;
    private SistemaDeVida vida;
    private EnemyAttack enemyAttack;

    void Awake()
    {
        enemyMovement = GetComponent<EnemyMovement>();
        vida = GetComponent<SistemaDeVida>();
    }

    void Update()
    {
        bool needsRecalculation = false;
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            activeEffects[i].duration -= Time.deltaTime;
            if (activeEffects[i].duration <= 0)
            {
                activeEffects.RemoveAt(i);
                needsRecalculation = true; // Um efeito expirou, precisamos recalcular!
            }
        }

        if (needsRecalculation)
        {
            // Após a atualização, recalcula os modificadores.
            UpdateAllModifiers();
        }
    }

    public void ApplySlow(float amount, float duration, float damageMultiplier)
    {
        // Remove qualquer efeito de lentidão antigo para evitar sobreposição estranha
        activeEffects.RemoveAll(e => e.type == EffectType.Slow);

        StatusEffect slowEffect = new StatusEffect
        {
            type = EffectType.Slow,
            magnitude = amount,
            duration = duration,
            damageTakenMultiplier = damageMultiplier // Atribui o novo valor
        };
        activeEffects.Add(slowEffect);
        Debug.Log(gameObject.name + " está sob efeito de lentidão. Multiplicador de dano: " + damageMultiplier);

        // Aplica os efeitos imediatamente
        UpdateAllModifiers();
    }

    public void ApplyWeaken(float amount, float duration)
    {
        activeEffects.RemoveAll(e => e.type == EffectType.Weaken);

        StatusEffect weakenEffect = new StatusEffect
        {
            type = EffectType.Weaken,
            magnitude = amount, // ex: 0.25 para 25% de redução
            duration = duration
        };
        activeEffects.Add(weakenEffect);
        Debug.Log(gameObject.name + " está enfraquecido, causando dano reduzido.");

        UpdateAllModifiers(); // Atualiza imediatamente
    }

    public void ApplyDefenseDown(float amount, float duration, int maxStacks)
    {
        var defenseDownEffects = activeEffects.Where(e => e.type == EffectType.DefenseDown).ToList();

        if (defenseDownEffects.Count < maxStacks)
        {
            // Se ainda não atingiu o máximo, adiciona um novo acúmulo
            activeEffects.Add(new StatusEffect
            {
                type = EffectType.DefenseDown,
                magnitude = amount,
                duration = duration
            });
            Debug.Log($"Defesa reduzida! Acúmulos: {defenseDownEffects.Count + 1}/{maxStacks}");
        }
        else
        {
            // Se já está no máximo, apenas renova a duração do acúmulo mais antigo
            var oldestStack = defenseDownEffects.OrderBy(e => e.duration).FirstOrDefault();
            if (oldestStack != null)
            {
                oldestStack.duration = duration;
                Debug.Log($"Defesa reduzida! Duração renovada. Acúmulos: {maxStacks}/{maxStacks}");
            }
        }

        UpdateAllModifiers();
    }

    private void UpdateAllModifiers()
    {
        // Lógica de Lentidão (sem alterações, apenas movida para cá)
        if (enemyMovement != null)
        {
            var slowEffects = activeEffects.Where(e => e.type == EffectType.Slow);
            if (slowEffects.Any())
            {
                float maxSlow = slowEffects.Max(e => e.magnitude);
                enemyMovement.SetSpeedModifier(1f - maxSlow);
            }
            else
            {
                enemyMovement.SetSpeedModifier(1f);
            }
        }

        if (vida != null)
        {
            float finalMultiplier = 1f;

            // Bônus de dano de efeitos de lentidão (como a Super Lentidão)
            var slowDamageEffects = activeEffects.Where(e => e.type == EffectType.Slow);
            if (slowDamageEffects.Any())
            {
                finalMultiplier = Mathf.Max(finalMultiplier, slowDamageEffects.Max(e => e.damageTakenMultiplier));
            }

            // Bônus de dano de efeitos de redução de defesa
            var defenseDownEffects = activeEffects.Where(e => e.type == EffectType.DefenseDown);
            if (defenseDownEffects.Any())
            {
                // Somamos o bônus de todos os stacks de defesa reduzida
                finalMultiplier += defenseDownEffects.Sum(e => e.magnitude);
            }

            vida.SetDamageMultiplier(finalMultiplier);
        }

        if (enemyAttack != null)
        {
            var weakenEffects = activeEffects.Where(e => e.type == EffectType.Weaken);
            if (weakenEffects.Any())
            {
                // Pega a redução mais forte
                float maxWeaken = weakenEffects.Max(e => e.magnitude);
                enemyAttack.SetDamageModifier(1f - maxWeaken); // Se a redução é 0.25, o dano é 75%
            }
            else
            {
                enemyAttack.SetDamageModifier(1f); // Volta ao normal
            }
        }
    }
}

// Uma classe simples para definir um efeito de status.
public class StatusEffect
{
    public EffectType type;
    public float magnitude;
    public float duration;
    public float damageTakenMultiplier = 1f; // ADICIONE ESTA LINHA
}

// Um enum para identificar os tipos de efeito.
public enum EffectType
{
    Slow,
    Poison,
    Weaken,
    DefenseDown
}