using UnityEngine;

// Garante que o objeto sempre terá os componentes essenciais.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ControladorShoot : MonoBehaviour
{
    [Header("Atributos de Movimento")]
    [Tooltip("A velocidade com que o projétil se move para frente.")]
    public float velocidade = 20f;
    [Tooltip("Quanto tempo (em segundos) o projétil existe antes de ser destruído se não atingir nada.")]
    public float tempoDeVida = 2f;

    [Header("Atributos de Dano")]
    [Tooltip("A quantidade de dano que este projétil causa ao impactar.")]
    public int dano = 25;
    [Tooltip("Prefab do efeito visual a ser criado no impacto (opcional).")]
    public GameObject efeitoDeImpacto;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * velocidade;
        Destroy(gameObject, tempoDeVida);
    }

    void OnTriggerEnter2D(Collider2D outroCollider)
    {
        // --- LÓGICA ATUALIZADA AQUI ---

        // Se o projétil atingir um inimigo...
        if (outroCollider.CompareTag("Enemy"))
        {
            // Tenta pegar o componente de vida do inimigo.
            SistemaDeVida vidaDoAlvo = outroCollider.GetComponent<SistemaDeVida>();
            if (vidaDoAlvo != null)
            {
                // Causa dano ao inimigo.
                vidaDoAlvo.ReceberDano(dano);
            }

            // Cria o efeito de impacto (se houver).
            if (efeitoDeImpacto != null)
            {
                Instantiate(efeitoDeImpacto, transform.position, Quaternion.identity);
            }

            // Destrói o projétil.
            Destroy(gameObject);
        }
        // Se, em vez disso, o projétil atingir o cristal...
        else if (outroCollider.CompareTag("Crystal"))
        {
            // O projétil NÃO causa dano.

            // Apenas cria o efeito de impacto (se houver).
            if (efeitoDeImpacto != null)
            {
                Instantiate(efeitoDeImpacto, transform.position, Quaternion.identity);
            }

            // E é destruído.
            Destroy(gameObject);
        }
    }
}