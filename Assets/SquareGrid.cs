using System;
using TMPro;
using UnityEngine;

public class SquareGrid : MonoBehaviour
{
    public Action<SquareGrid> OnSquareGridClicked;
    public Action<SquareGrid> OnSquareGridEnter;
    public Action<SquareGrid> OnSquareGridUp;

    public int x;
    public int y;

    public bool alreadyEntered = false;

    private int _number;
    public int Number
    {
        get { return _number; }
        set
        {
            if (value > 0)
            {
                _number = value;
                GetComponentInChildren<TextMeshPro>().text = _number.ToString();
            }
            else
            {
                _number = 0;
                GetComponentInChildren<TextMeshPro>().text = "";
            }
        }
    }

    private void Awake()
    {
        GetComponentInChildren<TextMeshPro>().text = "";
    }

    // 鼠标在当前节点按下
    private void OnMouseDown()
    {
        if (OnSquareGridClicked != null)
        {
            OnSquareGridClicked.Invoke(this);
        }
    }

    // 鼠标滑动当前节点
    private void OnMouseOver()
    {

    }

    // 鼠标进入当前节点
    private void OnMouseEnter()
    {
        if (OnSquareGridEnter != null)
        {
            OnSquareGridEnter.Invoke(this);
        }
    }

    // 鼠标在当前节点抬起
    private void OnMouseUp()
    {
        if (OnSquareGridUp != null)
        {
            OnSquareGridUp.Invoke(this);
        }
    }

    public void Recycle()
    {
        OnSquareGridClicked = null;
        OnSquareGridEnter = null;
    }

    public void SetIndex(int i, int j)
    {
        x = i;
        y = j;
    }
}
