using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 玩家状态管理器
/// </summary>
public class PlayerManager
{
    private const string PLAYER_DATA = "PlayerData.json";

    private int currentLevel = 0;
    public int CurrentLevel { get { return currentLevel; } }

    // 保存玩家解决的路径
    private List<int[,]> playerSolvedLevels = new List<int[,]>();

    private static PlayerManager _instance;
    public static PlayerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerManager();
            }
            return _instance;
        }
    }

    private void LoadPlayerData(string filepath)
    {
        try
        {
            string json = File.ReadAllText(filepath);
            JObject playerDataObj = JObject.Parse(json);
            currentLevel = (int)playerDataObj["currentLevel"];

            // 读取玩家解决的方案
            JArray solvedLevels = (JArray)playerDataObj["solvedLevels"];
            foreach (JArray levelArray in solvedLevels)
            {
                int rows = levelArray.Count;
                int cols = (levelArray[0] as JArray).Count;

                int[,] level = new int[rows, cols];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        level[i, j] = (int)levelArray[i][j];
                    }
                }
                playerSolvedLevels.Add(level);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("加载玩家数据失败：" + e.Message);
        }
    }

    public void SavePlayerData()
    {
        try
        {
            string filepath = Path.Join(Application.persistentDataPath, PLAYER_DATA);
            JObject playerDataObj = new JObject();
            playerDataObj["currentLevel"] = currentLevel;

            // 将玩家解决的方案保存到JSON中
            JArray solvedLevels = new JArray();
            foreach (int[,] level in playerSolvedLevels)
            {
                JArray levelArray = new JArray();
                for (int i = 0; i < level.GetLength(0); i++)
                {
                    JArray rowArray = new JArray();
                    for (int j = 0; j < level.GetLength(1); j++)
                    {
                        rowArray.Add(level[i, j]);
                    }
                    levelArray.Add(rowArray);
                }
                solvedLevels.Add(levelArray);
            }

            playerDataObj["solvedLevels"] = solvedLevels;

            File.WriteAllText(filepath, playerDataObj.ToString());
        }
        catch (System.Exception e)
        {
            Debug.LogError("保存玩家数据失败：" + e.Message);
        }
    }

    /// <summary>
    /// 初始化，如果存在玩家数据则加载，否则创建新的玩家数据
    /// </summary>
    public void Init()
    {
        string filepath = Path.Join(Application.persistentDataPath, PLAYER_DATA);
        if (File.Exists(filepath))
        {
            LoadPlayerData(filepath);
        }
        else
        {
            currentLevel = 1;
        }
    }

    public void AddLevel()
    {
        currentLevel += 1;
    }
}
