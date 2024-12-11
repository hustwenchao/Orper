public enum GameState { Start, Playing, Pause, End }

public class DebugGameManager
{

    private DebugGameManager()
    {

    }

    private static DebugGameManager _instance;
    public static DebugGameManager Instance
    {

        get
        {
            if (_instance == null)
            {
                _instance = new DebugGameManager();
            }
            return _instance;
        }

    }

    public GameState gameState = GameState.Start;
}
