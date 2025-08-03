using UnityEngine;

public class SistemaDeVida : MonoBehaviour
{
    [Header("Configurações de Vida")]
    [Tooltip("A quantidade máxima de vida que este objeto pode ter.")]
    public int vidaMaxima = 100;

    // Variável privada para armazenar a vida atual.
    private int vidaAtual;
    private float damageMultiplier = 1f;

    // A propriedade 'get' permite que outros scripts leiam a vida atual,
    // mas o 'private set' impede que eles a modifiquem diretamente.
    public int VidaAtual
    {
        get { return vidaAtual; }
        private set { vidaAtual = value; }
    }

    // Start é chamado antes do primeiro frame.
    void Start()
    {
        // Ao iniciar, a vida atual é definida como a vida máxima.
        vidaAtual = vidaMaxima;
    }

    // --- Função Pública para Causar Dano ---
    // Outros scripts (como o do projétil) chamarão esta função.
    public void ReceberDano(int quantidadeDeDano)
    {
        // Reduz a vida atual pela quantidade de dano recebido.
        int danoFinal = Mathf.RoundToInt(quantidadeDeDano * damageMultiplier);
        vidaAtual -= danoFinal;

        // Imprime no console para sabermos que o dano foi recebido (ótimo para debugar).
        Debug.Log(gameObject.name + " recebeu " + danoFinal + " de dano. Vida restante: " + vidaAtual);

        // --- AQUI VOCÊ PODE ADICIONAR FEEDBACK VISUAL/SONORO ---
        // Ex: Chamar uma função para piscar o sprite de vermelho, tocar um som de "hit", etc.
        // StartCoroutine(PiscarFeedbackDeDano());

        // Verifica se a vida chegou a zero ou menos.
        if (vidaAtual <= 0)
        {
            // Garante que a vida não fique negativa no display.
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

        // --- AQUI VOCÊ ADICIONA AS CONSEQUÊNCIAS DA MORTE ---
        // Ex: Tocar uma animação de explosão, dropar itens, adicionar pontos ao jogador, etc.
        // Instantiate(efeitoDeExplosao, transform.position, Quaternion.identity);

        // A ação mais simples é destruir o GameObject.
        Destroy(gameObject);
    }
}