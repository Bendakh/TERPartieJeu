using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EasyBot : MonoBehaviour
{
    private bool[] directions;
    private GameObject[] playersList;
    private Player nearestPlayer;

    [SerializeField]
    private Tile wall;

    [SerializeField]
    private Tile destructible;

    [SerializeField]
    private Tilemap tileMap;
    public enum States
    {
        IDLE,
        PATROL,
        ATTACKING,
        FLEEING
    }

    private States currentState;
    // Start is called before the first frame update
    void Start()
    {
        tileMap = GameObject.FindGameObjectWithTag("obstacles").GetComponentInParent<Tilemap>();
        playersList = GameObject.FindGameObjectsWithTag("Player");   
        FindNearestPlayer();
        currentState = States.IDLE;
        directions = ComputeDirections();
    }

    private void SetCurrentTarget(Player nearestPlayer)
    {
        if (nearestPlayer != this)
        {
            this.nearestPlayer = nearestPlayer;
        }
    }

    private float DistanceBetweenTwoPlayers(GameObject me, GameObject p)
    {
        return Vector2.Distance(me.transform.position, p.transform.position);
    }

    private void FindNearestPlayer()
    {
        SetCurrentTarget(playersList[1].GetComponent<Player>());
        for (int i = 1; i < playersList.Length; i++)
        {
            if(DistanceBetweenTwoPlayers(this.gameObject,nearestPlayer.gameObject) >= DistanceBetweenTwoPlayers(this.gameObject,playersList[i]))
            {
                SetCurrentTarget(playersList[i].GetComponent<Player>());
            }
        }
    }

    //Haut Droite Bas Gauche
    private bool[] ComputeDirections()
    {
        bool[] directions = { true, true, true, true };
        Vector3Int pos = tileMap.WorldToCell(this.gameObject.transform.position);
        //On vérifie les 4 cellules adjacentes
        Tile tileUp = tileMap.GetTile<Tile>(pos + new Vector3Int(0, 1, 0));
        Tile tileRight = tileMap.GetTile<Tile>(pos + new Vector3Int(1, 0, 0));
        Tile tileDown = tileMap.GetTile<Tile>(pos + new Vector3Int(0, -1, 0));
        Tile tileLeft = tileMap.GetTile<Tile>(pos + new Vector3Int(-1, 0, 0));
        if(tileUp == wall || tileUp == destructible)
        {
            directions[0] = false;
        }

        if (tileRight == wall || tileRight == destructible)
        {
            directions[1] = false;
        }

        if (tileDown == wall || tileDown == destructible)
        {
            directions[2] = false;
        }

        if (tileLeft == wall || tileLeft == destructible)
        {
            directions[3] = false;
        }



        return directions;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(currentState == States.IDLE)
        {

        }
        if(currentState == States.PATROL)
        {

        }
        if (currentState == States.ATTACKING)
        {

        }
        if (currentState == States.FLEEING)
        {

        }
    }
}
