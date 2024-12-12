using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public enum NumberGridState { Init, Right, Wrong }

public class NumberGrid : MonoBehaviour
{
    private Dictionary<NumberGridState, Color> colors = new Dictionary<NumberGridState, Color>
    {
        {NumberGridState.Init, Color.white},
        {NumberGridState.Right, Color.green},
        {NumberGridState.Wrong, Color.red}
    };

    private NumberGridState state;
    public NumberGridState State
    {
        get { return state; }
    }

    private TextMeshPro numberText;
    private SpriteRenderer spriteRenderer;

    public Action<NumberGrid> OnNumberGridDown;
    public Action<NumberGrid> OnNumberGridEnter;
    public Action<NumberGrid> OnNumberGridUp;

    public int x;
    public int y;

    private int number;
    public int Number
    {
        get { return number; }
    }

    public void UpdateNumberAndState(int number, NumberGridState state)
    {
        this.number = number;
        numberText.text = number == 0 ? string.Empty : number.ToString();
        this.state = state;
        spriteRenderer.color = colors[state];
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
        if (OnNumberGridDown != null)
        {
            OnNumberGridDown.Invoke(this);
        }
    }

    // 鼠标滑动当前节点
    private void OnMouseOver()
    {

    }

    // 鼠标进入当前节点
    private void OnMouseEnter()
    {
        if (OnNumberGridEnter != null && Input.GetMouseButton((int)MouseButton.Left))
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
        OnNumberGridDown = null;
        OnNumberGridEnter = null;
        OnNumberGridUp = null;
    }

    public void SetIndex(int i, int j)
    {
        x = i;
        y = j;
    }
}
