using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToCharSelect()
    {
        if(GameManagerScript.gmInstance.GetGameState() == GameManagerScript.GameState.mainMenu)
            StartCoroutine(SetCharSelect());
    }

    private IEnumerator SetCharSelect()
    {
        
        yield return new WaitForSeconds(0.25f);
        GameManagerScript.gmInstance.SetGameState(GameManagerScript.GameState.charSelect);
    }

    public void PlayUISound()
    {
        if (GameManagerScript.gmInstance.GetGameState() == GameManagerScript.GameState.mainMenu)
            GetComponent<AudioSource>().Play();
    }

    public void QuitGame()
    {
        if (GameManagerScript.gmInstance.GetGameState() == GameManagerScript.GameState.mainMenu || GameManagerScript.gmInstance.paused)
            Application.Quit();
    }

    public void TogglePauseViaUI()
    {
        StartCoroutine(GameManagerScript.gmInstance.TogglePause());
    }

    public void RestartScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("CharacterSelection");
    }
}
