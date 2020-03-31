using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RectanglesManager : MonoBehaviour, IPointerClickHandler
{
    public static RectanglesManager instance;

    public GameObject rectPrefab;
    public Connection connectionPrefab;
    public GameObject emptyCollider;
    public List<Collider2D> colliders = new List<Collider2D>();

    public RectTransform canvasRect;
    [Space]
    public Transform rectParent;
    public Transform connectionParent;
    public Connection creatingConnection;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance == this)
            Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            CreateRectangle(eventData);
        else if (eventData.button == PointerEventData.InputButton.Right && creatingConnection != null)
        {
            creatingConnection.DestroyConnection();
        }
    }

    /// <summary>
    /// Создает прямоугольник
    /// </summary>
    /// <param name="eventData">позиция</param>
    void CreateRectangle(PointerEventData eventData)
    {
        print("create rect");

        GameObject rect = Instantiate(rectPrefab, eventData.pointerPressRaycast.worldPosition, Quaternion.identity, rectParent);
        BoxCollider2D box = rect.GetComponent<BoxCollider2D>();
        if (box.OverlapCollider(new ContactFilter2D(), colliders) == 0)
        {
            rect.GetComponent<Image>().enabled = true;
            colliders.Add(box);
        }
        else Destroy(rect);
    }
}