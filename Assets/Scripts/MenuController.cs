using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void SelectEasy()
    {
        RunManager.Instance.SetDifficulty(Difficulty.Easy);
    }

    public void SelectNormal()
    {
        RunManager.Instance.SetDifficulty(Difficulty.Normal);
    }

    public void SelectHard()
    {
        RunManager.Instance.SetDifficulty(Difficulty.Hard);
    }

    public void StartGame()
    {
        RunManager.Instance.StartNewRun();
        SceneManager.LoadScene("GameScene");
    }
}