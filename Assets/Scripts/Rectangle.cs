using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Rectangle : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    public List<Connection> addingConnections;

    Image image;
    private void Start()
    {
        image = GetComponent<Image>();
        image.color = Random.ColorHSV();
    }
    
    #region EVENTS
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            MoveRectangle(eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //Двойной щелчок ЛКМ - создание прямоугольника
        if (eventData.clickCount == 2 && eventData.button == PointerEventData.InputButton.Left)
        {
            DeleteRectangle();
            return;
        }
        //Щелчок ПКМ - создание связи из прямоугольника, на который совершили нажатие
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //Если в данный момент не создается связь, то создаем
            if (RectanglesManager.instance.creatingConnection == null)
            {
                CreateConnection();
            }
            //Иначе завершаем создание связи
            else
            {
                EndConnection();
            }
        }
    }
    #endregion

    #region FUNCTIONS
    /// <summary>
    /// Создание связи из выбранного прямоугольника
    /// </summary>
    public void CreateConnection()
    {
        Connection newConnection = Instantiate(RectanglesManager.instance.connectionPrefab, Connection.WorldToScreenPoint(Input.mousePosition), Quaternion.identity, RectanglesManager.instance.connectionParent);
        newConnection.SetConnection(this);
        RectanglesManager.instance.creatingConnection = newConnection;
        addingConnections.Add(newConnection);
    }

    /// <summary>
    /// Окончание создания связи на выбранном прямоугольнике
    /// </summary>
    public void EndConnection()
    {
        //Если к выбранному прямоугольнику еще не создана связь из начального, завершаем создание связи
        if (CheckEndRectangleToConnection(this, RectanglesManager.instance.creatingConnection.rectangles[0]))
        {
            RectanglesManager.instance.creatingConnection.EndConnection(this);
            addingConnections.Add(RectanglesManager.instance.creatingConnection);
            RectanglesManager.instance.creatingConnection = null;
        }
        //Иначе уничтожаем связь
        else
        {
            RectanglesManager.instance.creatingConnection.DestroyConnection();
        }
    }

    /// <summary>
    /// Уничтожение прямоугольника
    /// </summary>
    public void DeleteRectangle()
    {
        //Сначала удаляем все связи прямоугольника
        Connection[] connections = addingConnections.ToArray();

        foreach (Connection connection in connections)
            connection.DestroyConnection();

        RectanglesManager.instance.colliders.Remove(this.GetComponent<Collider2D>());
        Destroy(this.gameObject);
    } 

    /// <summary>
    /// Передвигает прямоугольник вслед за пользовательским вводом
    /// </summary>
    /// <param name="eventData">Пользовательский ввод</param>
    public void MoveRectangle(PointerEventData eventData)
    {
        image.rectTransform.localPosition += (Vector3)eventData.delta;

        //Передвигаем все связи вслед за прямоугольником
        foreach (Connection item in addingConnections)
        {
            item.Move(this);
        }
    }

    /// <summary>
    /// Проверка на наличие связи между прямоугольниками
    /// </summary>
    /// <param name="first">Первый прямоугольник</param>
    /// <param name="second">Второй прямоугольник</param>
    /// <returns></returns>
    public static bool CheckEndRectangleToConnection(Rectangle first, Rectangle second)
    {
        foreach (Connection connection in first.addingConnections)
        {
            if (connection.rectangles[0] == second || connection.rectangles[1] == second)
            {
                print("false");
                return false;
            }
        }
        return true;
    }
    #endregion
}