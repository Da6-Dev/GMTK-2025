using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Alvo")]
    [Tooltip("Arraste o GameObject do Cristal (ou qualquer outro alvo) para este campo.")]
    public GameObject alvoObjeto;

    [Header("Configurações de Movimento")]
    [Tooltip("A velocidade de movimento do inimigo.")]
    public float velocidade = 2.5f;
    [Tooltip("A que distância do alvo o inimigo deve parar de se mover.")]
    public float distanciaParaParar = 1f; 
    private Rigidbody2D rb;
    private SpriteRenderer meuSpriteRenderer;
    private float speedModifier = 1f;
    private Transform transformDoAlvo;
    private SpriteRenderer spriteRendererDoAlvo;
    private Animator animator;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 
        meuSpriteRenderer = GetComponent<SpriteRenderer>(); 
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (alvoObjeto != null)
        {
            transformDoAlvo = alvoObjeto.transform;
            spriteRendererDoAlvo = alvoObjeto.GetComponent<SpriteRenderer>();

            if (spriteRendererDoAlvo == null)
            {
                Debug.LogWarning("O objeto alvo '" + alvoObjeto.name + "' não possui um componente SpriteRenderer. A lógica de ordenação de sprite não funcionará.");
            }
        }
        else
        {
            Debug.LogError("O 'alvoObjeto' não foi atribuído no Inspector para o inimigo '" + gameObject.name + "'. O inimigo não se moverá.");
        }
    }

    void Update()
    {
        if (spriteRendererDoAlvo == null) return;

        AtualizarOrdemDoSprite();
    }

    void FixedUpdate()
    {
        if (transformDoAlvo == null)
        {
            PararMovimento();
            return;
        }

        float distancia = Vector2.Distance(transform.position, transformDoAlvo.position);

        if (distancia <= distanciaParaParar)
        {
            PararMovimento();
        }
        else
        {
            MoverEmDirecaoAoAlvo();
        }
    }

    private void MoverEmDirecaoAoAlvo()
    {
        animator.SetFloat("Speed", 1f);

        Vector2 direcao = (transformDoAlvo.position - transform.position).normalized;

        rb.linearVelocity = direcao * velocidade * speedModifier;

        if (direcao.x > 0.01f)
        {
            meuSpriteRenderer.flipX = false;
        }
        else if (direcao.x < -0.01f)
        {
            meuSpriteRenderer.flipX = true;
        }
    }

    private void PararMovimento()
    {
        animator.SetFloat("Speed", 0f);
        rb.linearVelocity = Vector2.zero; 
    }

    private void AtualizarOrdemDoSprite()
    {
        if (transform.position.y < transformDoAlvo.position.y)
        {
            meuSpriteRenderer.sortingOrder = spriteRendererDoAlvo.sortingOrder + 1;
        }
        else
        {
            meuSpriteRenderer.sortingOrder = spriteRendererDoAlvo.sortingOrder - 1;
        }
    }

    public void SetSpeedModifier(float newModifier)
    {
        this.speedModifier = newModifier;
    }
}