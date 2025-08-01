using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ProjectileController : MonoBehaviour
{
    // Stats do projétil
    private float damage;
    private float speed;
    private int pierceLeft;

    // Flags de comportamento
    private bool isExplosive;
    private bool isPiercing;

    // Stats de efeitos
    private float explosionRadius;
    private float explosionDamage;
    private float critChance;
    private float critMultiplier;
    private bool isExecuteUnlocked;
    private float executeHealthThreshold;
    private float executeChance;
    private bool canSlow;
    private float slowAmount;
    private float slowDuration;
    private float slowDamageMultiplier;
    private bool canWeaken;
    private float weakenAmount;
    private float weakenDuration;
    private bool createWeakenAura;
    private float weakenAuraLifetime;
    private GameObject weakenAuraPrefab;
    private float weakenAuraSizeMultiplier;
    private bool canApplyDefenseDown;
    private float defenseDownAmount;
    private float defenseDownDuration;
    private int defenseDownMaxStacks;

    [Header("Referências")]
    public GameObject efeitoDeImpacto;
    private Rigidbody2D rb;

    // MÉTODO ATUALIZADO para receber os stats de crítico
    public void Initialize(float dmg, float spd, float lifetime, float size, bool explosive, bool piercing, float exploRadius, float exploDmg, int pierceAmount, float critCh, float critMult, bool execUnlocked, float execHealth, float execChance, bool canSlow, float slowAmt, float slowDur, float slowDmgMult, bool canWkn, float wknAmt, float wknDur, bool createWknAura, float wknAuraLifetime, GameObject wknAuraPrefab, float wknAuraSizeMult, bool canDefDown, float defDownAmt, float defDownDur, int defDownMaxStacks)
    {
        {
            // Stats básicos
            this.damage = dmg;
            this.speed = spd;
            this.pierceLeft = pierceAmount;
            // Flags
            this.isExplosive = explosive;
            this.isPiercing = piercing;
            // Explosão
            this.explosionRadius = exploRadius;
            this.explosionDamage = exploDmg;
            // Crítico
            this.critChance = critCh;
            this.critMultiplier = critMult;
            // Execução
            this.isExecuteUnlocked = execUnlocked;
            this.executeHealthThreshold = execHealth;
            this.executeChance = execChance;
            // Lentidão
            this.canSlow = canSlow;
            this.slowAmount = slowAmt;
            this.slowDuration = slowDur;
            this.slowDamageMultiplier = slowDmgMult;
            // Enfraquecimento
            this.canWeaken = canWkn;
            this.weakenAmount = wknAmt;
            this.weakenDuration = wknDur;
            this.createWeakenAura = createWknAura;
            this.weakenAuraLifetime = wknAuraLifetime;
            this.weakenAuraPrefab = wknAuraPrefab;
            this.weakenAuraSizeMultiplier = wknAuraSizeMult;
            this.canApplyDefenseDown = canDefDown;
            this.defenseDownAmount = defDownAmt;
            this.defenseDownDuration = defDownDur;
            this.defenseDownMaxStacks = defDownMaxStacks;


            transform.localScale *= size;
            rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = transform.right * this.speed;
            Destroy(gameObject, lifetime);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        GameObject mainTarget = null;
        if (other.CompareTag("Enemy"))
        {
            mainTarget = other.gameObject;
            SistemaDeVida vidaDoAlvo = other.GetComponent<SistemaDeVida>();
            StatusEffectController enemyStatus = other.GetComponent<StatusEffectController>();
            if (enemyStatus != null)
            {
                if (this.canSlow)
                {
                    enemyStatus.ApplySlow(this.slowAmount, this.slowDuration, this.slowDamageMultiplier);
                }
                // ADICIONE ESTE NOVO BLOCO
                if (this.canWeaken && !this.createWeakenAura)
                {
                    enemyStatus.ApplyWeaken(this.weakenAmount, this.weakenDuration);
                }
                if (this.canApplyDefenseDown)
                {
                    enemyStatus.ApplyDefenseDown(this.defenseDownAmount, this.defenseDownDuration, this.defenseDownMaxStacks);
                }
            }
            if (vidaDoAlvo != null)
            {
                // --- NOVA LÓGICA DE DANO E CRÍTICO ---

                float finalDamage = this.damage;
                bool isCritical = Random.Range(0f, 1f) < this.critChance;

                if (isCritical)
                {
                    // Se for crítico, primeiro calcula o dano amplificado
                    finalDamage *= this.critMultiplier;
                    Debug.Log("CRITICAL HIT!");

                    // Agora, verifica a condição para EXECUTAR
                    float vidaPercentual = (float)vidaDoAlvo.VidaAtual / vidaDoAlvo.vidaMaxima;
                    if (isExecuteUnlocked && vidaPercentual <= executeHealthThreshold)
                    {
                        // Rola o dado para a chance de execução
                        if (Random.Range(0f, 1f) < this.executeChance)
                        {
                            Debug.Log("EXECUTE!");
                            finalDamage = 999999; // Dano massivo para garantir a morte
                        }
                    }
                }

                // Aplica o dano final calculado
                vidaDoAlvo.ReceberDano((int)finalDamage);
            }

            HandleImpact(mainTarget);
        }
        else if (other.CompareTag("Crystal"))
        {
            HandleImpact(null);
        }
    }

    // O resto do script (HandleImpact, Explode) permanece igual...
    private void HandleImpact(GameObject directHitTarget)
    {
        if (createWeakenAura && weakenAuraPrefab != null)
        {
            GameObject auraObj = Instantiate(weakenAuraPrefab, transform.position, Quaternion.identity);
            auraObj.transform.localScale *= this.weakenAuraSizeMultiplier;
            WeakeningAuraController auraController = auraObj.GetComponent<WeakeningAuraController>();
            if (auraController != null)
            {
                auraController.Initialize(this.weakenAuraLifetime, this.weakenAmount, this.weakenDuration);
            }
        }
        if (isExplosive) { Explode(directHitTarget); }
        if (efeitoDeImpacto != null) { Instantiate(efeitoDeImpacto, transform.position, Quaternion.identity); }
        if (directHitTarget != null) { pierceLeft--; }
        if (!isPiercing || (directHitTarget != null && pierceLeft < 0)) { Destroy(gameObject); }
    }

    private void Explode(GameObject directHitTarget)
    {
        Collider2D[] collidersInRadius = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D col in collidersInRadius)
        {
            if (col.gameObject == directHitTarget) continue;
            if (col.CompareTag("Enemy"))
            {
                SistemaDeVida vidaDoAlvo = col.GetComponent<SistemaDeVida>();
                if (vidaDoAlvo != null) { vidaDoAlvo.ReceberDano((int)this.explosionDamage); }
            }
        }
    }
}