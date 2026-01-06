using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public string SceneName;

    public Image fadeImage;

    public void GoToGame()
    {
        SceneManager.LoadScene(SceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public IEnumerator FadeOut(bool isPlaying)
    {
        float time = 0;
        Color colour = fadeImage.color;
        while (time < 1)
        {
            time += Time.deltaTime;
            colour.a += Time.deltaTime;
            fadeImage.color = colour;
            yield return null;
        }

        if (isPlaying)
        {
            GoToGame();
        }

        else
        {
            QuitGame();
        }

    }

    public void ButtonPress(bool isPlaying)
    {
        StartCoroutine(FadeOut(isPlaying));
    }
}
