using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    private Transform target;
    private float speed = 25f;
    private int damage;

    // Método para a torre "configurar" o projétil ao criá-lo
    public void Initialize(Transform _target, int _damage)
    {
        this.target = _target;
        this.damage = _damage;
    }

    void Update()
    {
        // Se o alvo não existe mais (foi destruído), o projétil se autodestrói
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        // Move o projétil em direção ao alvo
        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        // Faz o projétil "olhar" para a direção em que está se movendo
        transform.up = direction;
    }

    // Quando o projétil colide com algo
    void OnTriggerEnter2D(Collider2D other)
    {
        // Se colidiu com o alvo que estava perseguindo
        if (other.transform == target)
        {
            SistemaDeVida vidaDoAlvo = target.GetComponent<SistemaDeVida>();
            if (vidaDoAlvo != null)
            {
                vidaDoAlvo.ReceberDano(damage);
            }
            // Destrói o projétil ao atingir o alvo
            Destroy(gameObject);
        }
    }
}
