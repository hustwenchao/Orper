using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    public Button GenerateMapBtn;
    public Slider slider;
    public Slider hideCountSlider;
    public GameObject block;
    public GameObject line;
    public int hideCount;
    private List<GameObject> cachedGos = new List<GameObject>();
    private List<GameObject> activeGos = new List<GameObject>();
    private int[,] grids;

    private List<GameObject> lines = new List<GameObject>();
    private List<GameObject> cachedLines = new List<GameObject>();

    int currentX = 0;
    int currentY = 0;
    int currentNumber = 0;

    bool finished = false;

    private void Awake()
    {
        GenerateMapBtn.onClick.AddListener(ClickGenerateMap);
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        OnSliderValueChanged(slider.value);
    }

    private void OnSliderValueChanged(float value)
    {
        hideCountSlider.minValue = 1;
        hideCountSlider.maxValue = (int)slider.value * (int)slider.value - 1;
    }

    private void ClickGenerateMap()
    {
        ClearAllLines();

        grids = CardCreator.CreateMap((int)slider.value);

        while (grids == null)
        {
            UnityEngine.Debug.Log("生成失败，重新生成");
            grids = CardCreator.CreateMap((int)slider.value);
        }

        hideCount = (int)hideCountSlider.value;

        // 随机隐藏几个格子，让用户来连接
        HideGrids(grids, hideCount);

        Generate(grids);

        currentX = 0;
        currentY = 0;
        currentNumber = 0;
        finished = false;
        DebugGameManager.Instance.gameState = GameState.Start;
    }

    private void HideGrids(int[,] grids, int hideCount)
    {
        int count = 0;
        while (count < hideCount)
        {
            int x = Random.Range(0, grids.GetLength(0));
            int y = Random.Range(0, grids.GetLength(1));
            if (grids[x, y] == 0)
            {
                continue;
            }
            else
            {
                grids[x, y] = 0;
                count++;
            }
        }
    }

    private void GacheAllBlocks()
    {
        foreach (GameObject activeGo in activeGos)
        {
            activeGo.SetActive(false);
            activeGo.GetComponent<SquareGrid>().Recycle();
            cachedGos.Add(activeGo);
        }
        activeGos.Clear();
    }

    private GameObject GetGo()
    {
        GameObject go;
        if (cachedGos.Count > 0)
        {
            go = cachedGos[0];
            cachedGos.RemoveAt(0);
            go.SetActive(true);
            activeGos.Add(go);
            return go;
        }

        go = Instantiate(block);
        go.SetActive(true);
        activeGos.Add(go);
        return go;
    }

    private GameObject GetLine()
    {
        GameObject go;
        if (cachedLines.Count > 0)
        {
            go = cachedLines[0];
            cachedLines.RemoveAt(0);
            go.SetActive(true);
            lines.Add(go);
            return go;
        }

        go = Instantiate(line);
        go.SetActive(true);
        lines.Add(go);
        return go;
    }

    public void Generate(int[,] grids)
    {
        GacheAllBlocks();

        for (int i = 0; i < grids.GetLength(0); i++)
        {
            for (int j = 0; j < grids.GetLength(1); j++)
            {
                GameObject go = GetGo();
                go.SetActive(true);
                SquareGrid squareGrid = go.GetComponent<SquareGrid>();
                squareGrid.Number = grids[i, j];
                squareGrid.OnSquareGridClicked += OnSquareGridClicked;
                squareGrid.OnSquareGridEnter += OnSquareGridEnter;
                squareGrid.OnSquareGridUp += OnSquareGridUp;
                squareGrid.SetIndex(i, j);
                go.transform.position = new Vector3(1.1f * i - 1.1f * grids.GetLength(0) * 0.5f, 1.1f * j - 1.1f * grids.GetLength(1) * 0.5f, 0);
            }
        }
    }

    private bool CheckGameState()
    {
        if (currentNumber == grids.GetLength(0) * grids.GetLength(1))
        {
            // 已经成功
            finished = true;
            DebugGameManager.Instance.gameState = GameState.End;
            return true;
        }

        // 判断当前的点能否走到下一个点
        if (currentX > 0 && (grids[currentX - 1, currentY] == currentNumber + 1 || grids[currentX - 1, currentY] == 0))
        {
            return true;
        }

        if (currentX < grids.GetLength(0) - 1 && (grids[currentX + 1, currentY] == currentNumber + 1 || grids[currentX + 1, currentY] == 0))
        {
            return true;
        }

        if (currentY > 0 && (grids[currentX, currentY - 1] == currentNumber + 1 || grids[currentX, currentY - 1] == 0))
        {
            return true;
        }

        if (currentY < grids.GetLength(1) - 1 && (grids[currentX, currentY + 1] == currentNumber + 1 || grids[currentX, currentY + 1] == 0))
        {
            return true;
        }

        DebugGameManager.Instance.gameState = GameState.End;
        return false;
    }

    private void OnSquareGridUp(SquareGrid grid)
    {
        if (DebugGameManager.Instance.gameState == GameState.End)
        {
            return;
        }

        if (DebugGameManager.Instance.gameState == GameState.Playing)
        {
            DebugGameManager.Instance.gameState = GameState.Pause;
        }
    }

    private void OnSquareGridClicked(SquareGrid grid)
    {
        if (DebugGameManager.Instance.gameState == GameState.End)
        {
            return;
        }

        DebugGameManager.Instance.gameState = GameState.Playing;

        if (grid.Number == 0 || grid.Number == currentNumber + 1)
        {
            if (currentNumber != 0)
            {
                DrawLine(currentX, currentY, grid.x, grid.y);
            }
            currentNumber++;
            grid.Number = currentNumber;
            currentX = grid.x;
            currentY = grid.y;
            grids[currentX, currentY] = currentNumber;

            // Draw Line
        }

        if (!CheckGameState())
        {
            // 游戏结束
            UnityEngine.Debug.Log("游戏结束, You Failed !");
        }

        if (finished)
        {
            // 游戏结束
            UnityEngine.Debug.Log("游戏结束, You Success !");
        }
    }

    private void ClearAllLines()
    {
        foreach (GameObject line in lines)
        {
            line.SetActive(false);
            cachedLines.Add(line);
        }
        lines.Clear();
    }

    private void DrawLine(int startX, int startY, int endX, int endY)
    {
        GameObject line = GetLine();

        // 判定是绘制横线还是竖线
        if (startX == endX)
        {
            // 竖线
            line.transform.position = new Vector3(1.1f * startX - 1.1f * grids.GetLength(0) * 0.5f, 1.1f * (startY + endY) * 0.5f - 1.1f * grids.GetLength(1) * 0.5f, 0);
            line.transform.localScale = new Vector3(0.1f, 1.1f * Mathf.Abs(startY - endY), 1);
        }
        else
        {
            // 横线
            line.transform.position = new Vector3(1.1f * (startX + endX) * 0.5f - 1.1f * grids.GetLength(0) * 0.5f, 1.1f * startY - 1.1f * grids.GetLength(1) * 0.5f, 0);
            line.transform.localScale = new Vector3(1.1f * Mathf.Abs(startX - endX), 0.1f, 1);
        }
    }

    private void OnSquareGridEnter(SquareGrid grid)
    {
        if (DebugGameManager.Instance.gameState == GameState.End)
        {
            return;
        }

        if (DebugGameManager.Instance.gameState == GameState.Pause)
        {
            return;
        }

        // 游戏还未开始
        if (currentNumber == 0)
        {
            return;
        }

        if (grid.Number == 0 || grid.Number == currentNumber + 1)
        {
            if (currentNumber != 0)
            {
                DrawLine(currentX, currentY, grid.x, grid.y);
            }

            currentNumber++;
            grid.Number = currentNumber;

            currentX = grid.x;
            currentY = grid.y;
            grids[currentX, currentY] = currentNumber;



        }

        if (!CheckGameState())
        {
            // 游戏结束
            UnityEngine.Debug.Log("游戏结束, You Failed !");
        }

        if (finished)
        {
            // 游戏结束
            UnityEngine.Debug.Log("游戏结束, You Success !");
        }
    }
}
