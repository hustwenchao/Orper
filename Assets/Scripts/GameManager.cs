using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameState state = GameState.Start;

    public GameState State
    {
        get { return state; }
    }

    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

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
        BoardManager.Instance.SetBoards(LevelManager.Instance.GetLevelConfig(level));
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;
    }

    public void LevelSuccess()
    {
        if (PlayerManager.Instance.CurrentLevel < LevelManager.Instance.GetMaxLevel())
        {
            PlayerManager.Instance.AddLevel();
            canvas.Refresh();
            SetGameBoard(PlayerManager.Instance.CurrentLevel);
        }
    }
}
