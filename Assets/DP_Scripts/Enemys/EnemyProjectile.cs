using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float velocidade = 10f;
    public int danoCausado = 15;
    public float tempoDeVida = 5f;

    void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity = transform.up * velocidade;
        Destroy(gameObject, tempoDeVida);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        SistemaDeVida vida = other.GetComponent<SistemaDeVida>();
        if (vida != null && other.CompareTag("Crystal"))
        {
            vida.ReceberDano(danoCausado);
        }

        Destroy(gameObject);
    }
}