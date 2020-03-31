using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Connection : MonoBehaviour
{
    public Rectangle[] rectangles = new Rectangle[2];
    LineRenderer lineRenderer;
    ConnectionRect connectionCollider;
    bool coroutineIsRunning = false;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        connectionCollider = transform.GetChild(0).GetComponent<ConnectionRect>();
    }

    private void Start()
    {
        StartCoroutine(WaitForEndRectangle());
    }
    
    /// <summary>
    /// создание связи
    /// </summary>
    /// <param name="startRect">прямоугольник, из которого создается связь</param>
    public void SetConnection(Rectangle startRect)
    {
        lineRenderer.SetPosition(0, startRect.transform.position - new Vector3(0f, 0f, lineRenderer.startWidth));
        rectangles[0] = startRect;
        StartCoroutine(WaitForEndRectangle());
    }

    /// <summary>
    /// завершает создание связи
    /// </summary>
    /// <param name="endRect">прямоугольник, к которому протянута связь</param>
    public void EndConnection(Rectangle endRect)
    {
        coroutineIsRunning = false;
        lineRenderer.SetPosition(1, endRect.transform.position - new Vector3(0f, 0f, lineRenderer.startWidth));
        rectangles[1] = endRect;

        //создание коллайдера для реализации возможности его удаления
        connectionCollider.gameObject.SetActive(true);
        connectionCollider.SetRect(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1), lineRenderer.startWidth);
    }

    /// <summary>
    /// Перемещение связи вслед за прикрепленным к ней прямоугольником
    /// </summary>
    /// <param name="rectangle">Прямоугольник который двигает пользователь</param>
    public void Move(Rectangle rectangle)
    {
        int pointIndex = rectangle == rectangles[0] ? 0 : 1;
        lineRenderer.SetPosition(pointIndex, rectangle.transform.position - new Vector3(0f, 0f, lineRenderer.startWidth));
        connectionCollider.SetRect(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1), lineRenderer.startWidth);
    }

    /// <summary>
    /// Уничтожает связь между прямоугольниками
    /// </summary>
    public void DestroyConnection()
    {
        coroutineIsRunning = false;

        //Стирание ссылок на связь у прямоуольников, между которыми она протянута
        if (rectangles[0] != null) rectangles[0].addingConnections.Remove(this);
        if (rectangles[1] != null) rectangles[1].addingConnections.Remove(this);
         
        Destroy(gameObject);
    }

    /// <summary>
    /// Протягивает связь к позиции пользовательского ввода до завершения ее создания
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForEndRectangle()
    {
        if (coroutineIsRunning) yield break;

        coroutineIsRunning = true;
        while(coroutineIsRunning)
        {
            lineRenderer.SetPosition(1, WorldToScreenPoint(Input.mousePosition));
            yield return null;
        }
    }

    /// <summary>
    /// Перевод координат пользовательского ввода в экранные
    /// </summary>
    /// <param name="input">Позиция пользовательского ввода</param>
    /// <returns></returns>
    public static Vector3 WorldToScreenPoint(Vector3 input)
    {
        return new Vector3((input.x - Screen.width/2) * Manager.instance.canvasRect.localScale.x, (input.y - Screen.height * .5f) * Manager.instance.canvasRect.localScale.y, 0f);
    }
}