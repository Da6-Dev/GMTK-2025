using UnityEngine;
using System.Collections.Generic;

public class WeakeningAuraController : MonoBehaviour
{
    private float weakenAmount;
    private float weakenDuration;

    // Guarda quem já foi afetado para não aplicar o debuff múltiplas vezes
    private List<Collider2D> alreadyAffected = new List<Collider2D>();

    public void Initialize(float auraLifetime, float wkAmt, float wkDur)
    {
        this.weakenAmount = wkAmt;
        this.weakenDuration = wkDur;
        // A aura se autodestrói depois de um tempo
        Destroy(gameObject, auraLifetime);
    }

    // Chamado quando um objeto entra no trigger da aura
    void OnTriggerEnter2D(Collider2D other)
    {
        // Se o inimigo já foi afetado por esta aura, ignore.
        if (alreadyAffected.Contains(other))
        {
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            StatusEffectController enemyStatus = other.GetComponent<StatusEffectController>();
            if (enemyStatus != null)
            {
                // Aplica o debuff e adiciona na lista de "já afetados"
                enemyStatus.ApplyWeaken(this.weakenAmount, this.weakenDuration);
                alreadyAffected.Add(other);
                Debug.Log(other.name + " entrou na área de enfraquecimento.");
            }
        }
    }
}