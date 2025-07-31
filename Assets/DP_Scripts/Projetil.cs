using UnityEngine;

public class Projetil : MonoBehaviour
{
    public float velocidade = 10f;
    public int danoCausado = 15; // O dano será definido pelo inimigo que atirou
    public float tempoDeVida = 5f; // Para o projétil se destruir se não acertar nada

    void Start()
    {
        // Move o projétil para frente (baseado na sua rotação)
        GetComponent<Rigidbody2D>().linearVelocity = transform.up * velocidade;
        // Destrói o projétil depois de um tempo
        Destroy(gameObject, tempoDeVida);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Tenta encontrar o sistema de vida no objeto atingido
        SistemaDeVida vida = other.GetComponent<SistemaDeVida>();
        if (vida != null && other.CompareTag("Crystal"))
        {
            vida.ReceberDano(danoCausado);
        }

        // Destrói o projétil ao colidir com qualquer coisa que tenha um collider
        Destroy(gameObject);
    }
}
