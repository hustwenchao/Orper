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
    public int hideCount;
    private List<GameObject> cachedGos = new List<GameObject>();
    private List<GameObject> activeGos = new List<GameObject>();
    private int[,] grids;

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
        GameManager.Instance.gameState = GameState.Start;
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
            GameManager.Instance.gameState = GameState.End;
            return true;
        }

        // 判断当前的点能否走到下一个点
        if (currentX > 0 && (grids[currentX - 1, currentY] == currentNumber + 1 || grids[currentX - 1, currentY] == 0))
        {
            Debug.Log(1);
            return true;
        }

        if (currentX < grids.GetLength(0) - 1 && (grids[currentX + 1, currentY] == currentNumber + 1 || grids[currentX + 1, currentY] == 0))
        {
            Debug.Log(2);
            return true;
        }

        if (currentY > 0 && (grids[currentX, currentY - 1] == currentNumber + 1 || grids[currentX, currentY - 1] == 0))
        {
            Debug.Log(3);
            return true;
        }

        if (currentY < grids.GetLength(1) - 1 && (grids[currentX, currentY + 1] == currentNumber + 1 || grids[currentX, currentY + 1] == 0))
        {
            Debug.Log(4);
            return true;
        }

        Debug.Log(5);
        GameManager.Instance.gameState = GameState.End;
        return false;
    }

    private void OnSquareGridUp(SquareGrid grid)
    {
        if (GameManager.Instance.gameState == GameState.End)
        {
            return;
        }

        if (GameManager.Instance.gameState == GameState.Playing)
        {
            GameManager.Instance.gameState = GameState.Pause;
        }
    }

    private void OnSquareGridClicked(SquareGrid grid)
    {
        if (GameManager.Instance.gameState == GameState.End)
        {
            return;
        }

        GameManager.Instance.gameState = GameState.Playing;

        if (grid.Number == 0 || grid.Number == currentNumber + 1)
        {
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

    private void OnSquareGridEnter(SquareGrid grid)
    {
        if (GameManager.Instance.gameState == GameState.End)
        {
            return;
        }

        if (GameManager.Instance.gameState == GameState.Pause)
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
