using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private GameObject player;

    [SerializeField]
    private Image hpBar;
    [SerializeField]
    private TextMeshProUGUI hpDisplay;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
       // if (player != null)
       // {
           // hpBar.fillAmount = (float)player.GetComponent<Player>().getHp() / (float)player.GetComponent<Player>().getMaxHp();
           // hpDisplay.text = player.GetComponent<Player>().getHp() + "/" + player.GetComponent<Player>().getMaxHp();
       //  }
    }
}
