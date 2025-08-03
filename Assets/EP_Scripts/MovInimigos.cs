using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovInimigos : MonoBehaviour
{
    private Transform posicaoDoCristal;

    public float velocidadeDoInimigo = 1f;

    void Start()
    {
        posicaoDoCristal = GameObject.FindGameObjectWithTag("Crystal").transform;
    }


    void Update()
    {
        SeguirJogador();
    }

    private void SeguirJogador()
    {
        transform.position = Vector2.MoveTowards(transform.position, posicaoDoCristal.position, velocidadeDoInimigo * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Crystal"))
        {
            Destroy(gameObject);
        }
    }
}
