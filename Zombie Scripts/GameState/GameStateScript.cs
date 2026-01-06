using Cinemachine;
using UnityEngine;

public class GameStateScript : MonoBehaviour
{
    [Header("Cameras")]
    public CinemachineVirtualCamera winCam;
    public CinemachineVirtualCamera lossCam;
    public CinemachineVirtualCamera playerCam;

    [Header("UI")]
    public GameObject HUD;
    public GameObject crossHair;
    public EndGamePanelScript endGamePanel;

    public static GameStateScript Instance { get; private set; }
    public static bool InstanceFound => Instance != null;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }

        else
            Destroy(this);
    }

    public void GameWin(string title, string time, int enemies)
    {
        winCam.Priority = 1;
        playerCam.Priority = 0;

        HUD.SetActive(false);
        crossHair.SetActive(false);

        endGamePanel.gameObject.SetActive(true);
        endGamePanel.UpdateTexts(title, time, enemies);
    }

    public void GameLoss(string title, string time, int enemies)
    {
        lossCam.Priority = 1;
        playerCam.Priority = 0;

        HUD.SetActive(false);
        crossHair.SetActive(false);

        endGamePanel.gameObject.SetActive(true);
        endGamePanel.UpdateTexts(title, time, enemies);
    }
}
