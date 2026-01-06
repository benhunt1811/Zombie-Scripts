using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGamePanelScript : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI enemyText;

    public void UpdateTexts(string title, string time, int enemies)
    {
        titleText.text = title;
        timeText.text = "Time: " + time;
        enemyText.text = "Enemies killed: " + enemies;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
