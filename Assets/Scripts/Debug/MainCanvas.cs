using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour
{
    public TextMeshProUGUI title;
    public Button restartGameBtn;

    private void Awake()
    {
        restartGameBtn.onClick.AddListener(ClickRestartGame);
    }

    public void Refresh()
    {
        title.text = string.Format("Game Level : {0}", PlayerManager.Instance.CurrentLevel);
    }

    private void ClickRestartGame()
    {

    }
}
