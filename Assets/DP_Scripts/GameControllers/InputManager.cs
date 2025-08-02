using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [Tooltip("Defina aqui a camada onde APENAS os seus TowerSlots estão.")]
    public LayerMask towerSlotLayer;

    private TowerSlot currentHoveredSlot;

    void Update()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, towerSlotLayer);

        if (hit.collider != null)
        {
            TowerSlot hitSlot = hit.collider.GetComponent<TowerSlot>();

            if (currentHoveredSlot != hitSlot)
            {
                if (currentHoveredSlot != null)
                {
                    currentHoveredSlot.OnMouseExitSlot();
                }
                currentHoveredSlot = hitSlot;
                currentHoveredSlot.OnMouseEnterSlot();
            }
        }

        else if (currentHoveredSlot != null)
        {
            currentHoveredSlot.OnMouseExitSlot();
            currentHoveredSlot = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            if (currentHoveredSlot != null)
            {
                currentHoveredSlot.ClickSlot();
            }
        }
    }
}