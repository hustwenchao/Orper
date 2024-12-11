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

        Vector3 offset = new Vector3((GridSize + Gap) * (length - 1) / 2, (GridSize + Gap) * (length - 1) / 2, 0);

        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                NumberGrid grid = gridPool.Get();
                grid.gameObject.SetActive(true);
                grid.transform.position = new Vector3(GridSize * i + Gap * i, GridSize * j + Gap * j, 0) - offset;
                grid.Number = grids[i, j];
                this.grids.Add(grid);
            }
        }
    }
}
