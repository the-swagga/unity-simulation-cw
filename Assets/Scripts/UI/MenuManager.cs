using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button mainGameButton;
    [SerializeField] private Button enemyGameButton;

    private void Awake()
    {
        mainGameButton.onClick.AddListener(LoadMainGame);
        enemyGameButton.onClick.AddListener(LoadEnemyGame);
    }

    private void LoadMainGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void LoadEnemyGame()
    {
        SceneManager.LoadScene("EnemyScene");
    }
}
