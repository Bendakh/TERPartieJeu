using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUi : MonoBehaviour
{
    [SerializeField]
    private GameObject optionsPanel;
    [SerializeField]
    private GameObject singlePlayer;
    [SerializeField]
    private GameObject multiPlayer;
    [SerializeField]
    private GameObject mainPanel;


    public void SinglePlayer()
    {
        singlePlayer.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void MultiPlayer()
    {
        multiPlayer.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void OptionsButton()
    {
        optionsPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void SinglePlayerReturn()
    {
        mainPanel.SetActive(true);
        singlePlayer.SetActive(false);  
    }

    public void OptionsReturn()
    {
        mainPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    public void StartGameButton()
    {
        SceneManager.LoadScene(1);
    }
}
