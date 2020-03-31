using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConnectionRect : MonoBehaviour, IPointerClickHandler
{
    RectTransform rect;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    public void SetRect(Vector2 point0, Vector2 point1, float width)
    {
        rect.sizeDelta = new Vector2(Vector2.Distance(point0, point1), width) / Manager.instance.canvasRect.localScale.x;

        Vector2 midPoint = (point0 + point1) / 2;
        rect.transform.position = midPoint;
        float angle = (Mathf.Abs(point0.y - point1.y) / Mathf.Abs(point0.x - point1.x));
        if ((point0.y < point1.y && point0.x > point1.x) || (point1.y < point0.y && point1.x > point0.x))
        {
            angle *= -1;
        }
        angle = Mathf.Rad2Deg * Mathf.Atan(angle);
        rect.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount == 2 && eventData.button == PointerEventData.InputButton.Left)
            transform.parent.GetComponent<Connection>().DestroyConnection();
    }
}
