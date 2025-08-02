using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class StatusEffectController : MonoBehaviour
{
    private List<StatusEffect> activeEffects = new List<StatusEffect>();
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
                needsRecalculation = true;
            }
        }

        if (needsRecalculation)
        {
            UpdateAllModifiers();
        }
    }

    public void ApplySlow(float amount, float duration, float damageMultiplier)
    {
        activeEffects.RemoveAll(e => e.type == EffectType.Slow);

        StatusEffect slowEffect = new StatusEffect
        {
            type = EffectType.Slow,
            magnitude = amount,
            duration = duration,
            damageTakenMultiplier = damageMultiplier
        };
        activeEffects.Add(slowEffect);

        UpdateAllModifiers();
    }

    public void ApplyWeaken(float amount, float duration)
    {
        activeEffects.RemoveAll(e => e.type == EffectType.Weaken);

        StatusEffect weakenEffect = new StatusEffect
        {
            type = EffectType.Weaken,
            magnitude = amount,
            duration = duration
        };
        activeEffects.Add(weakenEffect);

        UpdateAllModifiers();
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
        }
        else
        {
            // Se já está no máximo, apenas renova a duração do acúmulo mais antigo
            var oldestStack = defenseDownEffects.OrderBy(e => e.duration).FirstOrDefault();
            if (oldestStack != null)
            {
                oldestStack.duration = duration;
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
                // Soma o bônus de todos os stacks de defesa reduzida
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

public class StatusEffect
{
    public EffectType type;
    public float magnitude;
    public float duration;
    public float damageTakenMultiplier = 1f;
}

public enum EffectType
{
    Slow,
    Poison,
    Weaken,
    DefenseDown
}