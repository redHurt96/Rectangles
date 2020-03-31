using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Connection : MonoBehaviour
{
    public Rectangle[] rectangles = new Rectangle[2];
    [HideInInspector]
    public LineRenderer lineRenderer;
    [SerializeField] bool coroutineIsRunning = false;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
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
        print("set conn from " + rectangles[0].name + " " + endRect.name);

        coroutineIsRunning = false;
        lineRenderer.SetPosition(1, endRect.transform.position - new Vector3(0f, 0f, lineRenderer.startWidth));
        rectangles[1] = endRect;
    }

    /// <summary>
    /// Перемещение связи вслед за прикрепленным к ней прямоугольником
    /// </summary>
    /// <param name="rectangle">Прямоугольник который двигает пользователь</param>
    public void Move(Rectangle rectangle)
    {
        int pointIndex = rectangle == rectangles[0] ? 0 : 1;
        lineRenderer.SetPosition(pointIndex, rectangle.transform.position - new Vector3(0f, 0f, lineRenderer.startWidth));
    }

    /// <summary>
    /// Уничтожает связь между прямоугольниками
    /// </summary>
    public void DestroyConnection()
    {
        print("destroy conn");
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
        while(coroutineIsRunning)
        {
            lineRenderer.SetPosition(1, WorldToScreenPoint(Input.mousePosition));

            yield return null;
        }

        yield break;
    }

    /// <summary>
    /// Перевод координат пользовательского ввода в экранные
    /// </summary>
    /// <param name="input">Позиция пользовательского ввода</param>
    /// <returns></returns>
    public static Vector3 WorldToScreenPoint(Vector3 input)
    {
        return new Vector3((input.x - Screen.width/2) * RectanglesManager.instance.canvasRect.localScale.x, (input.y - Screen.height * .5f) * RectanglesManager.instance.canvasRect.localScale.y, 0f);
    }
}