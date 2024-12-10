using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapGenerator : MonoBehaviour
{
    public Button GenerateMapBtn;
    public Slider slider;
    public GameObject block;
    private List<GameObject> cachedGos = new List<GameObject>();
    private List<GameObject> activeGos = new List<GameObject>();

    private void Awake()
    {
        GenerateMapBtn.onClick.AddListener(ClickGenerateMap);
    }

    private void ClickGenerateMap()
    {
        int[,] grids = CardCreator.CreateMap((int)slider.value);
        while (grids == null)
        {
            UnityEngine.Debug.Log("生成失败，重新生成");
            grids = CardCreator.CreateMap((int)slider.value);
        }
        Generate(grids);
    }

    private void GacheAllBlocks()
    {
        foreach (GameObject activeGo in activeGos)
        {
            activeGo.SetActive(false);
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
                go.GetComponentInChildren<TextMeshPro>().text = grids[i, j].ToString();
                go.transform.position = new Vector3(1.1f * i, 1.1f * j, 0);
            }
        }
    }


}
