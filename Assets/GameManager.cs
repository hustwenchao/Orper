public enum GameState { Start, Playing, Pause, End }

public class GameManager
{

    private GameManager()
    {

    }

    private static GameManager _instance;
    public static GameManager Instance
    {

        get
        {
            if (_instance == null)
            {
                _instance = new GameManager();
            }
            return _instance;
        }

    }

    public GameState gameState = GameState.Start;
}
