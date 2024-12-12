using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
/// <summary>
/// 加载棋盘管理器
/// </summary>
public class LevelManager
{
    private List<int[,]> gameLevels = new List<int[,]>();

    private static LevelManager _instance;
    public static LevelManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LevelManager();
            }
            return _instance;
        }
    }

    private LevelManager() { }

    public void LoadLevels()
    {
        // 加载棋盘
#if UNITY_EDITOR
        // Editor下读取JSON

        try
        {
            string json = File.ReadAllText(Application.dataPath + "/Resources/Levels.json");
            JArray levels = JArray.Parse(json);
            foreach (JObject levelObj in levels)
            {
                int level = (int)levelObj["level"];

                JArray boardsArray = (JArray)levelObj["boards"];

                int rows = boardsArray.Count;
                int cols = boardsArray[0].Count();

                int[,] boards = new int[rows, cols];
                // 填充 int[,] 数组
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        boards[i, j] = (int)boardsArray[i][j];
                    }
                }
                gameLevels.Add(boards);
            }
            Debug.Log(string.Format("加载完成，总共关卡{0}", gameLevels.Count));
        }
        catch (System.Exception e)
        {
            Debug.LogError("读取关卡配置文件失败：" + e.Message);
            return;
        }
#endif
    }

    public int[,] GetLevelConfig(int level)
    {
        return gameLevels[level - 1];
    }

    public int GetMaxLevel()
    {
        return gameLevels.Count;
    }
}
