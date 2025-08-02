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

    public int VidaAtual
    {
        get { return vidaAtual; }
        private set { vidaAtual = value; }
    }

    void Start()
    {
        vidaAtual = vidaMaxima;
    }

    public void ReceberDano(int quantidadeDeDano)
    {
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

        if (recompensaPorMorte > 0)
        {
            EconomyManager.Instance.AdicionarDinheiro(recompensaPorMorte);
        }

        Destroy(gameObject);
    }
}