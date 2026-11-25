using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    public GameObject tooltipObject; 
    public Text tooltipText;

    private void Start()
    {
        tooltipObject.SetActive(false);
    }

    public void Mostrar(string texto)
    {
        tooltipText.text = texto;
        tooltipObject.SetActive(true);
    }

    public void Esconder()
    {
        tooltipObject.SetActive(false);
    }

    //private void Update()
    //{
    //    if (tooltipObject.activeSelf)
    //    {
    //        Vector2 pos;
    //        RectTransform canvasRect = tooltipObject.transform.parent.GetComponent<RectTransform>();
    //
    //        RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //            canvasRect,
    //            Input.mousePosition,
    //            null, 
    //            out pos);
    //
    //        tooltipObject.GetComponent<RectTransform>().localPosition = pos + new Vector2(190, 5);
    //    }
    //}
}