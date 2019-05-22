using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;

    [SerializeField]
    private GameObject playerPrefab;

    Stats[] playerList;

    private void Awake()
    {
        if (_instance == null)
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        playerList = FindObjectsOfType<Stats>();
    }

    // Update is called once per frame
    void Update()
    {
        playerList = FindObjectsOfType<Stats>();
        if (playerList.Length == 1)
        {
            Debug.Log("GAMEOVER");
        }
    }

    /*public void InstantiatePlayer(int id, string pseudo, Color color, Vector2 worldPos)
    {
        GameObject playerAvatar = Instantiate(playerPrefab, worldPos, Quaternion.identity);
        playerAvatar.GetComponent<SpriteRenderer>().color = color;
        playerList.Add(playerAvatar.GetComponent<Player>());


    }

    public void RemovePlayer(int id, string pseudo, Player player)
    {
        playerList.Remove(player);
    }*/
}
