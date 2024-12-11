using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // 中心点
    public Vector2 CenterPoint = Vector2.zero;
    public float GridSize = 1.0f;
    public float Gap = 0.1f;

    private List<NumberGrid> grids;
    private List<MoveLine> lines;

    public NumberGrid grid;
    public MoveLine line;

    public ObjectPool<NumberGrid> gridPool;
    public ObjectPool<MoveLine> linePool;

    private static BoardManager instance;

    private int[,] initialGridState = null;
    private int[,] gridState = null;

    // 当前焦点的格子
    private NumberGrid activeGrid = null;

    // 当前的数字
    private int currentNumber = 0;

    public static BoardManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        instance = this;
        grids = new List<NumberGrid>();
        lines = new List<MoveLine>();
        gridPool = new ObjectPool<NumberGrid>(grid, 10);
        linePool = new ObjectPool<MoveLine>(line, 10);
    }

    private void CollectAll()
    {
        foreach (var grid in grids)
        {
            grid.Recycle();
            gridPool.ReturnToPool(grid);
        }
        grids.Clear();
        foreach (var line in lines)
        {
            linePool.ReturnToPool(line);
        }
        lines.Clear();
    }

    public void SetBoards(int[,] grids)
    {
        CollectAll();
        int length = grids.GetLength(0);

        Vector3 offset = new Vector3(-(GridSize + Gap) * (length - 1) / 2, (GridSize + Gap) * (length - 1) / 2, 0);

        // 这里修改成和JSON数组一样的顺序
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                NumberGrid grid = gridPool.Get();
                grid.OnNumberGridClicked = OnNumberGridClicked;
                grid.OnNumberGridEnter = OnNumberGridEnter;
                grid.OnNumberGridUp = OnNumberGridUp;
                grid.gameObject.SetActive(true);
                grid.transform.position = new Vector3(GridSize * j + Gap * j, -(GridSize * i + Gap * i), 0) + offset;
                grid.Number = grids[i, j];
                grid.x = i;
                grid.y = j;
                this.grids.Add(grid);
            }
        }
        initialGridState = grids;

        // 拷贝一份 grid 作为棋盘的初始状态
        gridState = new int[grids.GetLength(0), grids.GetLength(1)];
        for (int i = 0; i < grids.GetLength(0); i++)
        {
            for (int j = 0; j < grids.GetLength(1); j++)
            {
                gridState[i, j] = grids[i, j];
            }
        }
    }

    private void OnNumberGridClicked(NumberGrid grid)
    {
        if (activeGrid == null)
        {
            activeGrid = grid;
        }
        else
        {
            return;
        }

        if (activeGrid.Number == 0 || activeGrid.Number == currentNumber + 1)
        {
            currentNumber++;
            activeGrid.Number = currentNumber;
            activeGrid.Occupy = true;
        }
    }


    private void OnNumberGridEnter(NumberGrid grid)
    {
        if (activeGrid == null)
        {
            return;
        }

        if (activeGrid.Number == gridState.GetLength(0) * gridState.GetLength(1))
        {
            // 达成目标
        }

        if (IsGridsNear(activeGrid, grid))
        {
            if (grid.Number == 0 || grid.Number == currentNumber + 1)
            {
                currentNumber++;
                grid.Number = currentNumber;
                grid.Occupy = true;
                ConnectGrid(activeGrid, grid);
                activeGrid = grid;

            }
        }
    }

    private void OnNumberGridUp(NumberGrid grid)
    {

    }

    private bool IsGridsNear(NumberGrid a, NumberGrid b)
    {
        if (a.x == b.x && Mathf.Abs(a.y - b.y) == 1)
        {
            return true;
        }
        if (a.y == b.y && Mathf.Abs(a.x - b.x) == 1)
        {
            return true;
        }
        return false;
    }

    private void ConnectGrid(NumberGrid a, NumberGrid b)
    {
        MoveLine line = linePool.Get();

        // 确定是横向还是纵向
        if (a.x == b.x)
        {
            float smallX = a.transform.position.x < b.transform.position.x ? a.transform.position.x : b.transform.position.x;
            line.transform.position = new Vector3(smallX + (GridSize + Gap) / 2, a.transform.position.y, 0);
            line.transform.localScale = new Vector3((GridSize + Gap), 0.1f, 1);
        }
        else
        {
            float smallY = a.transform.position.y < b.transform.position.y ? a.transform.position.y : b.transform.position.y;
            line.transform.position = new Vector3(a.transform.position.x, smallY + (GridSize + Gap) / 2, 0);
            line.transform.localScale = new Vector3(0.1f, (GridSize + Gap), 1);
        }
    }
}
