using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [Tooltip("Defina aqui a camada onde APENAS os seus TowerSlots estão.")]
    public LayerMask towerSlotLayer;

    private TowerSlot currentHoveredSlot;

    void Update()
    {
        // --- Lógica de Hover (Destaque ao passar o mouse) ---

        // Converte a posição do mouse para o mundo 2D
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        // Dispara um raio que só enxerga a camada dos slots
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, towerSlotLayer);

        // Se o raio atingiu um slot
        if (hit.collider != null)
        {
            TowerSlot hitSlot = hit.collider.GetComponent<TowerSlot>();
            
            // Se o slot que atingimos é diferente do que estávamos antes
            if (currentHoveredSlot != hitSlot)
            {
                // Desmarca o antigo
                if (currentHoveredSlot != null)
                {
                    currentHoveredSlot.OnMouseExitSlot();
                }
                // E marca o novo
                currentHoveredSlot = hitSlot;
                currentHoveredSlot.OnMouseEnterSlot();
            }
        }
        // Se o raio não atingiu nada, mas antes estávamos sobre um slot
        else if (currentHoveredSlot != null)
        {
            // Desmarca o slot antigo
            currentHoveredSlot.OnMouseExitSlot();
            currentHoveredSlot = null;
        }

        // --- Lógica de Clique ---

        // Se o botão esquerdo do mouse foi pressionado
        if (Input.GetMouseButtonDown(0))
        {
            // Primeiro, verifica se o clique foi em cima de algum elemento da UI (menu, botões, etc.)
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Se foi, ignora o clique no mundo do jogo para não construir atrás do menu
                return;
            }

            // Se o clique foi no mundo do jogo e estamos sobre um slot
            if (currentHoveredSlot != null)
            {
                // Manda o slot "se clicar"
                currentHoveredSlot.ClickSlot();
            }
        }
    }
}