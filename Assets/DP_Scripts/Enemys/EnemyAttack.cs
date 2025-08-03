using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Configurações de Ataque")]
    [Tooltip("O dano que este inimigo causa por ataque.")]
    public int dano = 10;

    [Tooltip("O tempo (em segundos) entre cada ataque.")]
    public float tempoEntreAtaques = 2f;

    [Tooltip("Marque se o ataque for corpo a corpo (melee). Desmarque para ataque à distância.")]
    public bool ehMelee = true;

    [Header("Referências (Apenas para Ranged)")]
    [Tooltip("O objeto do projétil que será disparado.")]
    public GameObject prefabDoProjetil;

    [Tooltip("O ponto de onde o projétil será disparado.")]
    public Transform pontoDeDisparo;

    private EnemyMovement ia;
    private float proximoAtaquePermitido = 0f;
    private float damageModifier = 1f;
    private Animator animator;

    void Awake()
    {
        ia = GetComponent<EnemyMovement>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (ia == null || ia.alvoObjeto == null) return;

        float distancia = Vector2.Distance(transform.position, ia.alvoObjeto.transform.position);

        if (distancia <= ia.distanciaParaParar && Time.time >= proximoAtaquePermitido)
        {
            if (!ehMelee)
            {
                AtacarADistancia();
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!ehMelee || Time.time < proximoAtaquePermitido || other.gameObject != ia.alvoObjeto) return;

        SistemaDeVida vidaDoAlvo = other.GetComponent<SistemaDeVida>();

        if (vidaDoAlvo != null)
        {
            AtacarCorpoACorpo(vidaDoAlvo);
        }
    }

    private void AtacarCorpoACorpo(SistemaDeVida alvo)
    {
        animator.SetTrigger("Attack");

        int danoFinal = Mathf.RoundToInt(dano * damageModifier);

        alvo.ReceberDano(danoFinal);

        proximoAtaquePermitido = Time.time + tempoEntreAtaques;
    }

    private void AtacarADistancia()
    {
        animator.SetTrigger("Attack");

        if (prefabDoProjetil == null || pontoDeDisparo == null)
        {
            Debug.LogError("Inimigo ranged " + gameObject.name + " não tem prefab de projétil ou ponto de disparo configurado!");
            return;
        }

        GameObject projetil = Instantiate(prefabDoProjetil, pontoDeDisparo.position, pontoDeDisparo.rotation);

        EnemyProjectile scriptDoProjetil = projetil.GetComponent<EnemyProjectile>();
        if (scriptDoProjetil != null)
        {
            int danoFinal = Mathf.RoundToInt(dano * damageModifier);
            scriptDoProjetil.danoCausado = danoFinal;
        }

        proximoAtaquePermitido = Time.time + tempoEntreAtaques;
    }
    
    public void SetDamageModifier(float modifier)
    {
        this.damageModifier = modifier;
    }
}