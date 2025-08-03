using UnityEngine;
using UnityEngine.EventSystems;

public class AtivarToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string textoTooltip;
    private ToolTip tooltip;

    void Start()
    {
        tooltip = Object.FindFirstObjectByType<ToolTip>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.Mostrar(textoTooltip);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.Esconder();
    }
}