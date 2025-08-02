using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class ControladorCarrinho : MonoBehaviour
{
    [Header("Configurações do Caminho")]
    [Tooltip("A sequência de pontos (waypoints) que o carrinho deve seguir em ordem.")]
    public Transform[] waypoints;

    [Header("Economia")]
    [Tooltip("A quantidade de dinheiro ganha a cada volta completa.")]
    public int dinheiroPorVolta = 10;

    [Header("Controles de Movimento")]
    public float velocidadeMaxima = 8f;
    public float taxaAceleracao = 2f;
    public float taxaFreio = 5f;

    [Header("Gráficos e Colisão")]
    public Sprite[] spritesDirecao = new Sprite[8];
    public float anguloDeOffset = 0f;
    public Transform transformDoCollider;

    // Variáveis de controle interno
    private int waypointAtualIndex = 0;
    private float velocidadeAtual;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private PlayerStats stats;
    private bool isMovingForward = true;

    // Variáveis de controle de voltas
    private int ultimoWaypointAlcancado = 0;
    private int waypointDeInicioDaVolta;
    public int VoltasCompletas { get; private set; }

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
            Debug.LogWarning("ControladorCarrinho não encontrou a instância de PlayerStats.");
        }
        if (waypoints.Length > 0)
        {
            transform.position = waypoints[0].position;
            waypointAtualIndex = 0; // Começa no primeiro waypoint
            ultimoWaypointAlcancado = 0; // Define o último ponto alcançado como o inicial
            waypointDeInicioDaVolta = 0; // A primeira volta sempre começa no waypoint 0.
        }
        VoltasCompletas = 0;
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

            int proximoWaypoint;
            if (isMovingForward)
            {
                proximoWaypoint = (waypointAtualIndex + 1) % waypoints.Length;
            }
            else
            {
                proximoWaypoint = waypointAtualIndex - 1;
                if (proximoWaypoint < 0) { proximoWaypoint = waypoints.Length - 1; }
            }

            waypointDeInicioDaVolta = proximoWaypoint;

            stats.TriggerDirectionChangeCooldown();
        }
    }

    void FixedUpdate()
    {
        MoverCarrinho();
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

        if (distanceToTarget <= travelDistanceThisFrame)
        {
            DetectarVoltaCompleta(waypointAtualIndex, ultimoWaypointAlcancado);

            ultimoWaypointAlcancado = waypointAtualIndex;
            
            if (isMovingForward)
            {
                waypointAtualIndex = (waypointAtualIndex + 1) % waypoints.Length;
            }
            else
            {
                waypointAtualIndex--;
                if (waypointAtualIndex < 0) { waypointAtualIndex = waypoints.Length - 1; }
            }

            transform.position = targetWaypoint.position;
        }

        Vector2 direcao = (targetWaypoint.position - transform.position).normalized;
        rb.linearVelocity = direcao * velocidadeAtual;
    }

    void DetectarVoltaCompleta(int pontoDeChegada, int pontoDeOrigem)
    {
        if (pontoDeChegada == waypointDeInicioDaVolta && pontoDeChegada != pontoDeOrigem)
        {
            VoltasCompletas++;
            EconomyManager.Instance.AdicionarDinheiro(dinheiroPorVolta);
        }
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
    
    void AtualizarSpriteEgirarCollider()
    {
        if (rb.linearVelocity.sqrMagnitude < 0.01f) return;
        Vector2 direcao = rb.linearVelocity.normalized;
        float anguloBruto = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;
        if (transformDoCollider != null) transformDoCollider.rotation = Quaternion.Euler(0f, 0f, anguloBruto);
        float anguloFinal = (anguloBruto + anguloDeOffset + 360f) % 360f;
        int indexDoSprite = GetIndexPorAngulo(anguloFinal);
        if (spriteRenderer.sprite != spritesDirecao[indexDoSprite]) spriteRenderer.sprite = spritesDirecao[indexDoSprite];
    }
    
    private int GetIndexPorAngulo(float angulo)
    {
        if (angulo >= 337.5 || angulo < 22.5) return 0;
        if (angulo >= 22.5 && angulo < 67.5) return 1;
        if (angulo >= 67.5 && angulo < 112.5) return 2;
        if (angulo >= 112.5 && angulo < 157.5) return 3;
        if (angulo >= 157.5 && angulo < 202.5) return 4;
        if (angulo >= 202.5 && angulo < 247.5) return 5;
        if (angulo >= 247.5 && angulo < 292.5) return 6;
        if (angulo >= 292.5 && angulo < 337.5) return 7;
        return 0;
    }
}