using UnityEngine;

public class AtaqueInimigo : MonoBehaviour
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
    private InimigoIA ia;
    private float proximoAtaquePermitido = 0f;

    void Awake()
    {
        // Pega o componente de IA para saber se está no alcance
        ia = GetComponent<InimigoIA>();
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
        SistemaDeVida vidaDoAlvo = other.GetComponent<SistemaDeVida>();

        if (vidaDoAlvo != null)
        {
            AtacarCorpoACorpo(vidaDoAlvo);
        }
    }

    private void AtacarCorpoACorpo(SistemaDeVida alvo)
    {
        Debug.Log(gameObject.name + " ataca corpo a corpo, causando " + dano + " de dano.");
        alvo.ReceberDano(dano);
        
        // Define o tempo do próximo ataque
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
        
        // Cria o projétil no ponto de disparo
        GameObject projetil = Instantiate(prefabDoProjetil, pontoDeDisparo.position, pontoDeDisparo.rotation);
        
        // ---- AQUI VOCÊ PRECISA DE UM SCRIPT NO PROJÉTIL ----
        // O projétil precisa de seu próprio script para se mover e causar dano no impacto.
        // Vamos supor que ele tenha um script "Projetil.cs" que recebe o dano a ser causado.
        Projetil scriptDoProjetil = projetil.GetComponent<Projetil>();
        if (scriptDoProjetil != null)
        {
            scriptDoProjetil.danoCausado = dano; // Passa o dano para o projétil
        }

        // Define o tempo do próximo ataque
        proximoAtaquePermitido = Time.time + tempoEntreAtaques;
    }
}