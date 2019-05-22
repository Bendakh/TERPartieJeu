using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndgameMenuUI : MonoBehaviour
{
   public void ReturnToMainMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    public void StartGameButton()
    {
        SceneManager.LoadScene(1);
    }
}
