using UnityEngine;

public class Crystal : MonoBehaviour
{
    public static Transform instance;

    void Awake()
    {
        instance = transform;
    }
}