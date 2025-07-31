using UnityEngine;

public class ControladorMiraPlayer : MonoBehaviour
{
    [Header("Mira")]
    [Tooltip("Array com 8 sprites para a mira. 0=Direita, e segue em sentido horário.")]
    public Sprite[] spritesDeMira = new Sprite[8];
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private float angulo;

    [Header("Tiro")]
    [Tooltip("O objeto (bala) que será disparado.")]
    public GameObject projetilPrefab;
    [Tooltip("O ponto de origem de onde os projéteis são disparados.")]
    public Transform pontoDeTiro;
    [Tooltip("O intervalo em segundos entre cada tiro (cadência).")]
    public float intervaloDeTiro = 0.1f;
    private float proximoTiro;

    [Header("Dispersão da Arma (Spray)")]
    [Tooltip("A dispersão máxima da arma em graus quando se atira sem parar.")]
    public float dispersaoMaxima = 15f;
    [Tooltip("A velocidade com que a dispersão aumenta ao atirar.")]
    public float taxaAumentoDispersao = 20f;
    [Tooltip("A velocidade com que a dispersão diminui ao parar de atirar.")]
    public float taxaRecuperacaoDispersao = 30f;
    private float dispersaoAtual;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        GerenciarMira();
        GerenciarTiro();
    }

    void GerenciarMira()
    {
        Vector2 posMouseNoMundo = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direcaoDaMira = (posMouseNoMundo - (Vector2)transform.position).normalized;

        // Calcula o ângulo em graus da direção em que a mira está apontando.
        angulo = Mathf.Atan2(direcaoDaMira.y, direcaoDaMira.x) * Mathf.Rad2Deg;

        int indexDoSprite = GetIndexPorAngulo(angulo);
        spriteRenderer.sprite = spritesDeMira[indexDoSprite];
    }

    void GerenciarTiro()
    {
        // Verifica se o jogador está segurando o botão esquerdo do mouse.
        if (Input.GetMouseButton(0))
        {
            // Aumenta a dispersão da arma gradualmente até o valor máximo.
            dispersaoAtual = Mathf.MoveTowards(dispersaoAtual, dispersaoMaxima, taxaAumentoDispersao * Time.deltaTime);

            // Controla a cadência do tiro. Só permite atirar se o tempo de espera já passou.
            if (Time.time > proximoTiro)
            {
                // Define o tempo do próximo tiro.
                proximoTiro = Time.time + intervaloDeTiro;
                Atirar();
            }
        }
        else
        {
            // Se o jogador não está atirando, a dispersão volta a zero gradualmente.
            dispersaoAtual = Mathf.MoveTowards(dispersaoAtual, 0f, taxaRecuperacaoDispersao * Time.deltaTime);
        }
    }

    void Atirar()
    {
        if (projetilPrefab == null || pontoDeTiro == null) return;

        // Calcula um desvio aleatório para o tiro baseado na dispersão atual.
        float desvio = Random.Range(-dispersaoAtual, dispersaoAtual);
        float anguloDoTiro = angulo + desvio;
        
        // Cria o projétil na posição e rotação calculadas.
        Instantiate(projetilPrefab, pontoDeTiro.position, Quaternion.Euler(0, 0, anguloDoTiro));
    }
    
    private int GetIndexPorAngulo(float angulo)
    {
        // Converte ângulos negativos (ex: -90) para seus equivalentes positivos (ex: 270).
        if (angulo < 0)
        {
            angulo += 360;
        }

        // Divide o círculo de 360 graus em 8 "fatias" de 45 graus para escolher o sprite certo.
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