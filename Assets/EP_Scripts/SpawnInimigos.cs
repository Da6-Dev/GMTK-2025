using Unity.VisualScripting;
using UnityEngine;

public class SpawnInimigos : MonoBehaviour
{
    public GameObject inimigoPrefab;

    public Transform waypoint1;
    public Transform waypoint2;
    public Transform waypoint3;
    public Transform waypoint4;
    public Transform waypoint5;
    public Transform waypoint6;
    public Transform waypoint7;
    public Transform waypoint8;
    public Transform waypoint9;
    public Transform waypoint10;
    public Transform waypoint11;
    public Transform waypoint12;
    public Transform waypoint13;
    public Transform waypoint14;
    public Transform waypoint15;
    public Transform waypoint16;

    private int num = 6;

    private void Update()
    {
        Mov();
    }
    void Mov()
    {
        if(num > 0)
        {
            int sorteio = Random.Range(1, 16);
            if (sorteio == 1)
            {
                GameObject Inimigo = Instantiate(inimigoPrefab, waypoint1.position, Quaternion.identity);
            }
            if (sorteio == 2)
            {
                GameObject Inimigo2 = Instantiate(inimigoPrefab, waypoint2.position, Quaternion.identity);
            }
            if (sorteio == 3)
            {
                GameObject Inimigo3 = Instantiate(inimigoPrefab, waypoint3.position, Quaternion.identity);
            }
            if (sorteio == 4)
            {
                GameObject Inimigo4 = Instantiate(inimigoPrefab, waypoint4.position, Quaternion.identity);
            }
            if (sorteio == 5)
            {
                GameObject Inimigo5 = Instantiate(inimigoPrefab, waypoint5.position, Quaternion.identity);
            }
            if (sorteio == 6)
            {
                GameObject Inimigo6 = Instantiate(inimigoPrefab, waypoint6.position, Quaternion.identity);
            }
            if (sorteio == 7)
            {
                GameObject Inimigo7 = Instantiate(inimigoPrefab, waypoint7.position, Quaternion.identity);
            }
            if (sorteio == 8)
            {
                GameObject Inimigo8 = Instantiate(inimigoPrefab, waypoint8.position, Quaternion.identity);
            }
            if (sorteio == 9)
            {
                GameObject Inimigo9 = Instantiate(inimigoPrefab, waypoint9.position, Quaternion.identity);
            }
            if (sorteio == 10)
            {
                GameObject Inimigo10 = Instantiate(inimigoPrefab, waypoint10.position, Quaternion.identity);
            }
            if (sorteio == 11)
            {
                GameObject Inimigo11 = Instantiate(inimigoPrefab, waypoint11.position, Quaternion.identity);
            }
            if (sorteio == 12)
            {
                GameObject Inimigo12 = Instantiate(inimigoPrefab, waypoint12.position, Quaternion.identity);
            }
            if (sorteio == 13)
            {
                GameObject Inimigo13 = Instantiate(inimigoPrefab, waypoint13.position, Quaternion.identity);
            }
            if (sorteio == 14)
            {
                GameObject Inimigo14 = Instantiate(inimigoPrefab, waypoint14.position, Quaternion.identity);
            }
            if (sorteio == 15)
            {
                GameObject Inimigo15 = Instantiate(inimigoPrefab, waypoint15.position, Quaternion.identity);
            }
            if (sorteio == 16)
            {
                GameObject Inimigo16 = Instantiate(inimigoPrefab, waypoint16.position, Quaternion.identity);
            }
            num -= 1;
        }

    }
}
