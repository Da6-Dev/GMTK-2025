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

    // Variáveis privadas
    private EnemyMovement ia; // Corrigido de InimigoIA para EnemyMovement
    private float proximoAtaquePermitido = 0f;
    private float damageModifier = 1f;

    void Awake()
    {
        // Pega o componente de IA para saber se está no alcance
        ia = GetComponent<EnemyMovement>(); // Corrigido de InimigoIA para EnemyMovement
    }

    void Update()
    {
        // Se a IA não encontrou o alvo, não faz nada.
        if (ia == null || ia.alvoObjeto == null) return;

        // Calcula a distância até o alvo
        float distancia = Vector2.Distance(transform.position, ia.alvoObjeto.transform.position);

        // Verifica se está no alcance de parada (que agora consideramos alcance de ATIRAR)
        // e se o tempo de cooldown do ataque já passou.
        if (distancia <= ia.distanciaParaParar && Time.time >= proximoAtaquePermitido)
        {
            if (!ehMelee) // Se for um inimigo de ataque à distância
            {
                AtacarADistancia();
            }
            // A lógica de ataque melee será tratada por colisão para ser mais simples.
        }
    }

    // Para o ataque MELEE, usamos a física de colisão.
    // Garante que o inimigo e o alvo tenham um Collider2D.
    // O collider do INIMIGO deve ser um Trigger.
    void OnTriggerStay2D(Collider2D other)
    {
        // Se não for melee, ou se o tempo de ataque não passou, ou se não for o alvo, ignora.
        if (!ehMelee || Time.time < proximoAtaquePermitido || other.gameObject != ia.alvoObjeto) return;

        // Tenta pegar o componente de vida do objeto que colidiu
        // Supondo que você tenha um script "SistemaDeVida"
        SistemaDeVida vidaDoAlvo = other.GetComponent<SistemaDeVida>();

        if (vidaDoAlvo != null)
        {
            AtacarCorpoACorpo(vidaDoAlvo);
        }
    }

    private void AtacarCorpoACorpo(SistemaDeVida alvo)
    {
        // Calcula o dano final com o modificador
        int danoFinal = Mathf.RoundToInt(dano * damageModifier);

        Debug.Log(gameObject.name + " ataca corpo a corpo, causando " + danoFinal + " de dano.");
        alvo.ReceberDano(danoFinal); // Usa o dano final

        proximoAtaquePermitido = Time.time + tempoEntreAtaques;
    }

    private void AtacarADistancia()
    {
        // Validações
        if (prefabDoProjetil == null || pontoDeDisparo == null)
        {
            Debug.LogError("Inimigo ranged " + gameObject.name + " não tem prefab de projétil ou ponto de disparo configurado!");
            return;
        }

        Debug.Log(gameObject.name + " atira em direção ao alvo.");

        GameObject projetil = Instantiate(prefabDoProjetil, pontoDeDisparo.position, pontoDeDisparo.rotation);

        EnemyProjectile scriptDoProjetil = projetil.GetComponent<EnemyProjectile>();
        if (scriptDoProjetil != null)
        {
            // Calcula o dano final com o modificador
            int danoFinal = Mathf.RoundToInt(dano * damageModifier);
            scriptDoProjetil.danoCausado = danoFinal; // Passa o dano final para o projétil
        }

        proximoAtaquePermitido = Time.time + tempoEntreAtaques;
    }
    
    public void SetDamageModifier(float modifier)
    {
        this.damageModifier = modifier;
    }
}