using UnityEngine;

public class GameManager : MonoBehaviour
{
    public MainCanvas canvas;

    private void LoadPlayerState()
    {
        PlayerManager.Instance.Init();
    }

    private void LoadGameLevels()
    {
        // 加载游戏棋盘的配置
        LevelManager.Instance.LoadLevels();
    }

    private void Start()
    {
        LoadGameLevels();
        LoadPlayerState();

        // 初始化游戏
        SetGameBoard(PlayerManager.Instance.CurrentLevel);

        canvas.Refresh();
    }

    private void SetGameBoard(int level)
    {
        BoardManager.Instance.SetBoards(LevelManager.Instance.GetLevel(level));
    }
}
