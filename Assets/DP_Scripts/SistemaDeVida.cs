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

    public int VidaAtual
    {
        get { return vidaAtual; }
        private set { vidaAtual = value; }
    }

    void Awake()
    {
        animator = GetComponent<Animator>(); // Pega o Animator
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