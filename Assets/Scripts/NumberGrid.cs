using System;
using TMPro;
using UnityEngine;

public class NumberGrid : MonoBehaviour
{
    private TextMeshPro numberText;
    private SpriteRenderer spriteRenderer;

    public Action<NumberGrid> OnNumberGridClicked;
    public Action<NumberGrid> OnNumberGridEnter;
    public Action<NumberGrid> OnNumberGridUp;

    public int x;
    public int y;

    private int _number;
    public int Number
    {
        get { return _number; }
        set
        {
            if (value > 0)
            {
                _number = value;
                numberText.text = _number.ToString();
            }
            else
            {
                _number = 0;
                numberText.text = "";
            }
        }
    }

    private bool occupy = false;
    public bool Occupy
    {
        get { return occupy; }
        set
        {
            occupy = value;
            spriteRenderer.color = value ? Color.cyan : Color.white;
        }
    }

    private void Awake()
    {
        numberText = GetComponentInChildren<TextMeshPro>();
        numberText.text = "";
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // 鼠标在当前节点按下
    private void OnMouseDown()
    {
        if (OnNumberGridClicked != null)
        {
            OnNumberGridClicked.Invoke(this);
        }
    }

    // 鼠标滑动当前节点
    private void OnMouseOver()
    {

    }

    // 鼠标进入当前节点
    private void OnMouseEnter()
    {
        if (OnNumberGridEnter != null)
        {
            OnNumberGridEnter.Invoke(this);
        }
    }

    // 鼠标在当前节点抬起
    private void OnMouseUp()
    {
        if (OnNumberGridUp != null)
        {
            OnNumberGridUp.Invoke(this);
        }
    }

    public void Recycle()
    {
        OnNumberGridClicked = null;
        OnNumberGridEnter = null;
        OnNumberGridUp = null;
    }

    public void SetIndex(int i, int j)
    {
        x = i;
        y = j;
    }
}
