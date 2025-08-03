using UnityEngine;

public class SistemaDeVida : MonoBehaviour
{
    [Header("Configura��es de Vida")]
    [Tooltip("A quantidade m�xima de vida que este objeto pode ter.")]
    public int vidaMaxima = 100;

    // Vari�vel privada para armazenar a vida atual.
    private int vidaAtual;
    private float damageMultiplier = 1f;

    // A propriedade 'get' permite que outros scripts leiam a vida atual,
    // mas o 'private set' impede que eles a modifiquem diretamente.
    public int VidaAtual
    {
        get { return vidaAtual; }
        private set { vidaAtual = value; }
    }

    // Start � chamado antes do primeiro frame.
    void Start()
    {
        // Ao iniciar, a vida atual � definida como a vida m�xima.
        vidaAtual = vidaMaxima;
    }

    // --- Fun��o P�blica para Causar Dano ---
    // Outros scripts (como o do proj�til) chamar�o esta fun��o.
    public void ReceberDano(int quantidadeDeDano)
    {
        // Reduz a vida atual pela quantidade de dano recebido.
        int danoFinal = Mathf.RoundToInt(quantidadeDeDano * damageMultiplier);
        vidaAtual -= danoFinal;

        // Imprime no console para sabermos que o dano foi recebido (�timo para debugar).
        Debug.Log(gameObject.name + " recebeu " + danoFinal + " de dano. Vida restante: " + vidaAtual);

        // --- AQUI VOC� PODE ADICIONAR FEEDBACK VISUAL/SONORO ---
        // Ex: Chamar uma fun��o para piscar o sprite de vermelho, tocar um som de "hit", etc.
        // StartCoroutine(PiscarFeedbackDeDano());

        // Verifica se a vida chegou a zero ou menos.
        if (vidaAtual <= 0)
        {
            // Garante que a vida n�o fique negativa no display.
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
        // Imprime no console que o objeto morreu.
        Debug.Log(gameObject.name + " morreu!");

        // --- AQUI VOC� ADICIONA AS CONSEQU�NCIAS DA MORTE ---
        // Ex: Tocar uma anima��o de explos�o, dropar itens, adicionar pontos ao jogador, etc.
        // Instantiate(efeitoDeExplosao, transform.position, Quaternion.identity);

        // A a��o mais simples � destruir o GameObject.
        Destroy(gameObject);
    }
}