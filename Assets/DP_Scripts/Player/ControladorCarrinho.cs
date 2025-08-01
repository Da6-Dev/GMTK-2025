using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ControladorCarrinho : MonoBehaviour
{
    [Header("Configurações do Caminho")]
    [Tooltip("A sequência de pontos (waypoints) que o carrinho deve seguir em ordem.")]
    public Transform[] waypoints;

    [Header("Controles de Movimento")]
    [Tooltip("A velocidade máxima que o carrinho pode atingir.")]
    public float velocidadeMaxima = 8f;
    [Tooltip("A rapidez com que o carrinho acelera ao pressionar a tecla.")]
    public float taxaAceleracao = 2f;
    [Tooltip("A rapidez com que o carrinho para ao soltar a tecla.")]
    public float taxaFreio = 5f;

    [Header("Gráficos e Colisão")]
    [Tooltip("Array com 8 sprites para as direções. ORDEM: 0: Direita, 1: Cima-Direita, 2: Cima, etc.")]
    public Sprite[] spritesDirecao = new Sprite[8];
    [Tooltip("Um ajuste fino para o ângulo do sprite, caso a arte original não esteja alinhada para a direita.")]
    public float anguloDeOffset = 0f;
    [Tooltip("Arraste o objeto-filho que contém o BoxCollider2D para este campo.")]
    public Transform transformDoCollider; // ADICIONADO: Referência para o Transform do objeto-filho que tem o collider.

    // Variáveis de controle interno
    private int waypointAtualIndex = 0;
    private float velocidadeAtual;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private PlayerStats stats;
    private bool isMovingForward = true;
    private float CurrentMaxSpeed
    {
        get { return velocidadeMaxima * (stats != null ? stats.cartSpeedMultiplier : 1f); }
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        stats = PlayerStats.Instance;
        if (stats == null)
        {
            Debug.LogWarning("ControladorCarrinho não encontrou a instância de PlayerStats. Upgrades de velocidade não funcionarão.");
        }
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
        }
    }

    void Update()
    {
        GerenciarVelocidade();
        AtualizarSpriteEgirarCollider();

        if (Input.GetKeyDown(KeyCode.LeftShift) && stats != null)
        {
            stats.ActivateDash();
        }

        if (Input.GetKeyDown(KeyCode.Q) && stats != null && stats.canChangeDirection && !stats.IsDirectionChangeOnCooldown)
        {
            isMovingForward = !isMovingForward;
            Debug.Log("Direção do carrinho invertida!");
            if (isMovingForward)
            {
                waypointAtualIndex = (waypointAtualIndex + 1) % waypoints.Length;
            }
            else
            {
                waypointAtualIndex--;
                if (waypointAtualIndex < 0) { waypointAtualIndex = waypoints.Length - 1; }
            }

            stats.TriggerDirectionChangeCooldown();
        }
    }

    void FixedUpdate()
    {
        MoverCarrinho();
    }

    void GerenciarVelocidade()
    {
        if (stats != null && stats.IsDashing)
        {
            velocidadeAtual = stats.dashSpeed;
            return;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            velocidadeAtual = Mathf.Lerp(velocidadeAtual, CurrentMaxSpeed, taxaAceleracao * Time.deltaTime);
        }
        else
        {
            velocidadeAtual = Mathf.Lerp(velocidadeAtual, 0f, taxaFreio * Time.deltaTime);
        }
    }

    void MoverCarrinho()
    {
        if (velocidadeAtual < 0.01f || waypoints.Length == 0)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Transform targetWaypoint = waypoints[waypointAtualIndex];

        float distanceToTarget = Vector2.Distance(transform.position, targetWaypoint.position);

        float travelDistanceThisFrame = velocidadeAtual * Time.fixedDeltaTime;

        if (distanceToTarget < travelDistanceThisFrame)
        {
            Transform reachedWaypoint = targetWaypoint;
            if (isMovingForward)
            {
                waypointAtualIndex = (waypointAtualIndex + 1) % waypoints.Length;
            }
            else
            {
                waypointAtualIndex--;
                if (waypointAtualIndex < 0)
                {
                    waypointAtualIndex = waypoints.Length - 1;
                }
            }

            targetWaypoint = waypoints[waypointAtualIndex];

            transform.position = reachedWaypoint.position;
        }

        Vector2 direcao = (targetWaypoint.position - transform.position).normalized;
        rb.linearVelocity = direcao * velocidadeAtual;
    }

    void AtualizarSpriteEgirarCollider()
    {
        if (rb.linearVelocity.sqrMagnitude < 0.01f)
        {
            return;
        }

        Vector2 direcao = rb.linearVelocity.normalized;
        float anguloBruto = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;

        // --- ROTAÇÃO DO COLLIDER ---
        // Gira o Transform do objeto-filho para alinhar o colisor com a direção real do movimento.
        if (transformDoCollider != null)
        {
            transformDoCollider.rotation = Quaternion.Euler(0f, 0f, anguloBruto);
        }
        // ---------------------------

        // --- TROCA DO SPRITE (Lógica original mantida) ---
        // Usa o ângulo com offset para escolher o sprite correto das 8 direções.
        float anguloFinal = (anguloBruto + anguloDeOffset + 360f) % 360f;
        int indexDoSprite = GetIndexPorAngulo(anguloFinal);

        if (spriteRenderer.sprite != spritesDirecao[indexDoSprite])
        {
            spriteRenderer.sprite = spritesDirecao[indexDoSprite];
        }
        // ------------------------------------------------
    }


    private int GetIndexPorAngulo(float angulo)
    {
        // Esta função volta a ser usada normalmente.
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