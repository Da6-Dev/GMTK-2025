using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicSortByY : MonoBehaviour
{
    private const int PrecisionMultiplier = 100;

    [Tooltip("Use isso para ajustar a ordem de um objeto específico. Um valor maior o trará para frente.")]
    [SerializeField] private int sortingOrderOffset = 0;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        spriteRenderer.sortingOrder = (int)(transform.position.y * -PrecisionMultiplier) + sortingOrderOffset;
    }
}