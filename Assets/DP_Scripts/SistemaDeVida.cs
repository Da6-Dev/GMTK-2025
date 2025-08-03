using UnityEngine;

public class SistemaDeVida : MonoBehaviour
{
    [Header("Configurações de Vida")]
    [Tooltip("A quantidade máxima de vida que este objeto pode ter.")]
    public int vidaMaxima = 100;

    [HideInInspector] 
    public int recompensaPorMorte = 0;
    private int vidaAtual;
    private float damageMultiplier = 1f; 

    private Animator animator;
    private bool estaMorto = false;
    public float tempoAnimacaoMorte = 1f;

    [Header("Áudio (apenas para Crystal)")]
    public AudioClip somDanoFraco;
    public AudioClip somDanoMedio;
    public AudioClip somDanoForte;
    private AudioSource audioSource;

    public int VidaAtual
    {
        get { return vidaAtual; }
        private set { vidaAtual = value; }
    }

    void Awake()
    {
        animator = GetComponent<Animator>(); // Pega o Animator
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        vidaAtual = vidaMaxima;
    }

    public void ReceberDano(int quantidadeDeDano)
    {
        if (estaMorto) return;

        int danoFinal = Mathf.RoundToInt(quantidadeDeDano * damageMultiplier);
        vidaAtual -= danoFinal;

        // Se for o Crystal, toca um som aleatório entre os três disponíveis
        if (CompareTag("Crystal"))
        {
            AudioClip[] sons = new AudioClip[] { somDanoFraco, somDanoMedio, somDanoForte };
            var sonsValidos = System.Array.FindAll(sons, s => s != null);
            if (sonsValidos.Length > 0 && !audioSource.isPlaying)
            {
                int idx = Random.Range(0, sonsValidos.Length);
                audioSource.PlayOneShot(sonsValidos[idx]);
            }
        }

        if (vidaAtual <= 0)
        {
            vidaAtual = 0;
            Morrer();
        }
    }

    public void SetDamageMultiplier(float multiplier)
    {
        this.damageMultiplier = multiplier;
    }

    private void Morrer()
    {
        estaMorto = true;
        vidaAtual = 0;

        // Aciona a animação de morte
        animator.SetTrigger("Die");

        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<EnemyMovement>().enabled = false;
        GetComponent<EnemyAttack>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        if (recompensaPorMorte > 0)
        {
            EconomyManager.Instance.AdicionarDinheiro(recompensaPorMorte);
        }

        Destroy(gameObject, tempoAnimacaoMorte);
    }
}