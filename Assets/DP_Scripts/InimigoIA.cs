using UnityEngine;

// Garante que o GameObject tenha os componentes necessários para o script funcionar.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class InimigoIA : MonoBehaviour
{
    [Header("Alvo")]
    [Tooltip("Arraste o GameObject do Cristal (ou qualquer outro alvo) para este campo.")]
    public GameObject alvoObjeto; // Simplificado: agora só precisamos de uma referência.

    [Header("Configurações de Movimento")]
    [Tooltip("A velocidade de movimento do inimigo.")]
    public float velocidade = 2.5f;
    [Tooltip("A que distância do alvo o inimigo deve parar de se mover.")]
    public float distanciaParaParar = 1f;

    // --- Componentes e Variáveis Internas (Privadas) ---
    private Rigidbody2D rb;
    private SpriteRenderer meuSpriteRenderer;

    // Variáveis do alvo que serão cacheadas para eficiência
    private Transform transformDoAlvo;
    private SpriteRenderer spriteRendererDoAlvo;

    // Awake é chamado uma única vez no início, ideal para configurar referências.
    void Awake()
    {
        // Pega os componentes do próprio inimigo e os armazena (caching).
        rb = GetComponent<Rigidbody2D>();
        meuSpriteRenderer = GetComponent<SpriteRenderer>();

        // Valida e configura as referências do alvo para não ter que buscá-las toda hora.
        if (alvoObjeto != null)
        {
            transformDoAlvo = alvoObjeto.transform;
            spriteRendererDoAlvo = alvoObjeto.GetComponent<SpriteRenderer>();

            // Se o alvo não tiver um SpriteRenderer, avisa no console para ajudar a debugar.
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

    // Update é chamado a cada frame. Ideal para lógicas visuais, como a ordem do sprite.
    void Update()
    {
        // Se não tivermos a referência do sprite do alvo, não fazemos nada.
        if (spriteRendererDoAlvo == null) return;

        AtualizarOrdemDoSprite();
    }

    // FixedUpdate é chamado em um intervalo de tempo fixo. Ideal para física.
    void FixedUpdate()
    {
        // Se o alvo não foi definido, o inimigo não se move.
        if (transformDoAlvo == null)
        {
            PararMovimento();
            return;
        }

        // Mede a distância entre o inimigo e o alvo.
        float distancia = Vector2.Distance(transform.position, transformDoAlvo.position);

        // Se a distância for menor ou igual à distância de parada, o inimigo para.
        if (distancia <= distanciaParaParar)
        {
            PararMovimento();
        }
        // Caso contrário, ele se move em direção ao alvo.
        else
        {
            MoverEmDirecaoAoAlvo();
        }
    }

    // --- Métodos de Ação (para organizar o código) ---

    private void MoverEmDirecaoAoAlvo()
    {
        // Calcula a direção normalizada para o alvo.
        Vector2 direcao = (transformDoAlvo.position - transform.position).normalized;

        // Aplica a velocidade na direção calculada.
        rb.linearVelocity = direcao * velocidade;

        // Gira o inimigo para que ele "olhe" para onde está indo.
        Girar(direcao);
    }

    private void PararMovimento()
    {
        // Zera a velocidade para parar o movimento completamente.
        rb.linearVelocity = Vector2.zero;
    }

    private void Girar(Vector2 direcao)
    {
        // Calcula o ângulo em graus a partir do vetor de direção.
        float angulo = Mathf.Atan2(direcao.y, direcao.x) * Mathf.Rad2Deg;

        // Aplica a rotação. O "- 90f" é um ajuste comum se o seu sprite original
        // estiver "olhando para cima" em vez de "para a direita". Ajuste se necessário.
        rb.rotation = angulo - 90f;
    }

    private void AtualizarOrdemDoSprite()
    {
        // Compara a posição Y para decidir quem aparece na frente.
        if (transform.position.y < transformDoAlvo.position.y)
        {
            // Inimigo está abaixo do cristal -> aparece na FRENTE.
            meuSpriteRenderer.sortingOrder = spriteRendererDoAlvo.sortingOrder + 1;
        }
        else
        {
            // Inimigo está acima do cristal -> aparece ATRÁS.
            meuSpriteRenderer.sortingOrder = spriteRendererDoAlvo.sortingOrder - 1;
        }
    }
}