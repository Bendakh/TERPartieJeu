using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EasyBot : MonoBehaviour
{
    [SerializeField]
    private GameObject bombPrefab;

    private Stats botStats;

    private int radius = 3;
    private int[] directions;
    private GameObject[] playersList;
    private Player nearestPlayer;
    private float explodeTime = 2f;
    private float explodeCounter;
    private Vector3Int bombPosition;
    int toAvoid = -1;
    [SerializeField]
    private Tile wall;

    [SerializeField]
    private Tile destructible;

    [SerializeField]
    private Tilemap tileMap;
    public enum States
    {

        PATROL,
        ATTACKING,
        FLEEING
            //Maybe make another fleeing state when hp <= 20%
    }

    private States currentState;

    private Vector3 lastPos;
    private bool canMove = true;
    private Rigidbody2D rb2d;
    

    // Start is called before the first frame update
    void Start()
    {
        botStats = this.GetComponent<Stats>();
        rb2d = GetComponent<Rigidbody2D>();
        tileMap = GameObject.FindGameObjectWithTag("obstacles").GetComponentInParent<Tilemap>();
        playersList = GameObject.FindGameObjectsWithTag("Player");
        FindNearestPlayer();
        currentState = States.PATROL;
        //directions = ComputeDirections();


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
        SetCurrentTarget(playersList[0].GetComponent<Player>());
        for (int i = 0; i < playersList.Length; i++)
        {
            if (playersList[i] != this.gameObject)
            {
                if (DistanceBetweenTwoPlayers(this.gameObject, nearestPlayer.gameObject) >= DistanceBetweenTwoPlayers(this.gameObject, playersList[i]))
                {
                    SetCurrentTarget(playersList[i].GetComponent<Player>());
                }
            }
        }
    }

    //Haut Droite Bas Gauche
    private int[] ComputeDirections()
    {
        int[] directions = { 0, 0, 0, 0 };
        Vector3Int pos = tileMap.WorldToCell(this.gameObject.transform.position);
        //On vérifie les 4 cellules adjacentes
        Tile tileUp = tileMap.GetTile<Tile>(pos + new Vector3Int(0, 1, 0));
        Tile tileRight = tileMap.GetTile<Tile>(pos + new Vector3Int(1, 0, 0));
        Tile tileDown = tileMap.GetTile<Tile>(pos + new Vector3Int(0, -1, 0));
        Tile tileLeft = tileMap.GetTile<Tile>(pos + new Vector3Int(-1, 0, 0));
        //Obstacles
        if (tileUp == wall)
        {
            directions[0] = 2;
        }

        if (tileRight == wall)
        {
            directions[1] = 2;
        }

        if (tileDown == wall)
        {
            directions[2] = 2;
        }

        if (tileLeft == wall)
        {
            directions[3] = 2;
        }
        //Destructibles

        if (tileUp == destructible)
        {
            directions[0] = 1;
        }

        if (tileRight == destructible)
        {
            directions[1] = 1;
        }

        if (tileDown == destructible)
        {
            directions[2] = 1;
        }

        if (tileLeft == destructible)
        {
            directions[3] = 1;
        }



        return directions;
    }

    private void Patrol()
    {
        if(DistanceBetweenTwoPlayers(this.gameObject,nearestPlayer.gameObject) <= 10)
        {
            this.currentState = States.ATTACKING;
        }

        directions = ComputeDirections();
        int action = Random.Range(0, 4);
        while (directions[action] == 2)
        {
            action = Random.Range(0, 4);
        }
        //Debug.Log(action);
        //Debug.Log(directions[action]);

        if (directions[action] == 0)
        {
            if (action == 0)
            {
                //Debug.Log("Go Up");
                Move(0, 1);
            }
            if (action == 1)
            {
                //Debug.Log("Go Right");
                Move(1, 0);
            }
            if (action == 2)
            {
                //Debug.Log("Go Bas");
                Move(0, -1);
            }
            if (action == 3)
            {
                //Debug.Log("Go Left");
                Move(-1, 0);
            }
        }

        else if (directions[action] == 1)
        {

            //Debug.Log("Put a bomb");
            
            //Instantiate Bomb
            
            this.explodeCounter = this.explodeTime;
            directions = ComputeDirections();

            int fleeAction = Random.Range(0, 4);

            while (directions[fleeAction] != 0)
            {
                fleeAction = Random.Range(0, 4);
            }

            if (fleeAction == 0)
            {
                toAvoid = 2;
                Move(0, 1);
            }
            if (fleeAction == 1)
            {
                toAvoid = 3;
                Move(1, 0);
            }
            if (fleeAction == 2)
            {
                toAvoid = 0;
                Move(0, -1);
            }
            if (fleeAction == 3)
            {
                toAvoid = 1;
                Move(-1, 0);
            }

            this.currentState = States.FLEEING;
        }

    }

    private void Flee()
    {
        

        directions = ComputeDirections();
        int fleeAction = Random.Range(0, 4);

        while (directions[fleeAction] != 0 || fleeAction == toAvoid)
        {
            fleeAction = Random.Range(0, 4);
        }

        if (fleeAction == 0)
        {              
            Move(0, 1);
        }
        if (fleeAction == 1)
        {              
            Move(1, 0);
        }
        if (fleeAction == 2)
        {            
            Move(0, -1);
        }
        if (fleeAction == 3)
        {              
            Move(-1, 0);
        }


        if (explodeCounter < 0)
        {
            toAvoid = -1;
            this.currentState = States.PATROL;
        }
        
    }

    private void Attack()
    {
        if (DistanceBetweenTwoPlayers(this.gameObject, nearestPlayer.gameObject) >= 10)
        {
            this.currentState = States.PATROL;
        }

        //Attack code
        Vector2 toOtherPlayer = new Vector2(this.gameObject.transform.position.x - nearestPlayer.transform.position.x, this.gameObject.transform.position.y - nearestPlayer.transform.position.y);
        //If on the same row    
        //Debug.Log(toOtherPlayer);
        if(toOtherPlayer.y == 0)
        {
            //DO THE SHIT 
            if(Mathf.Abs(toOtherPlayer.x) <= (radius + 1))
            {
                Debug.Log("Put bomb on x");
                //Put a bomb
                PutABomb();
                //Flee
                this.currentState = States.FLEEING;
                
            }
        }

        //If on the same column    
        else if (toOtherPlayer.x == 0)
        {
            //DO SOME MORE SHIT
            if(Mathf.Abs(toOtherPlayer.y) <= (radius + 1))
            {
                Debug.Log("Put bomb on y");
                //Put a bomb
                PutABomb();
                //Flee
                this.currentState = States.FLEEING;
                
                
            }
        }

        //If not on the same column and not on the same row   
        //Go on the same one of any of them
        else if (toOtherPlayer.x != 0 && toOtherPlayer.y != 0)
        {
            //This is going to be hard to make
            //See if column or row is closer and go by the closer one
            if(Mathf.Abs(toOtherPlayer.x) <= Mathf.Abs(toOtherPlayer.y))
            {
                //Debug.Log("X is shorter than Y");
                if(toOtherPlayer.x < -0.3f)
                {
                    //Go Right
                    directions = ComputeDirections();
                    if(directions[1] == 0)
                    {
                        Move(1, 0);
                    }
                    else
                    {
                        int dir = -1;
                        for(int i = 0; i < 4; i++)
                        {
                            if (directions[i] == 0)
                                dir = i;
                        }

                        if (dir == 0)
                        {              
                            Move(0, 1);
                        }
                        if (dir == 2)
                        {
                            Move(0, -1);
                        }
                        if (dir == 3)
                        {                           
                            Move(-1, 0);
                        }
                    }
                }

                else if(toOtherPlayer.x > 0.3f)
                {
                    //Go Left
                    directions = ComputeDirections();
                    if (directions[3] == 0)
                    {
                        Move(-1, 0);
                    }
                    else
                    {
                        int dir = -1;
                        for (int i = 0; i < 4; i++)
                        {
                            if (directions[i] == 0)
                                dir = i;
                        }

                        if (dir == 0)
                        {
                            Move(0, 1);
                        }
                        if (dir == 1)
                        {
                            Move(1, 0);
                        }
                        if (dir == 2)
                        {
                            Move(0, -1);
                        }                     
                    }
                }
            }

            else if (Mathf.Abs(toOtherPlayer.x) > Mathf.Abs(toOtherPlayer.y))
            {
                Debug.Log("Y is shorter than X");
                if(toOtherPlayer.y < -0.3f)
                {
                    //Go up
                    directions = ComputeDirections();
                    if (directions[0] == 0)
                    {
                        Debug.Log("GoUp");
                        Move(0, 1);
                        
                    }
                    else
                    {
                        int dir = -1;
                        for (int i = 0; i < 4; i++)
                        {
                            if (directions[i] == 0)
                                dir = i;
                        }

                        if (dir == 1)
                        {
                            Move(1, 0);
                        }
                        if (dir == 2)
                        {
                            Move(0, -1);
                        }
                        if (dir == 3)
                        {
                            Move(-1, 0);
                        }
                    }
                }

                else if(toOtherPlayer.y > 0.3f)
                {
                    //Go down
                    directions = ComputeDirections();
                    if (directions[2] == 0)
                    {
                        Debug.Log("GoDown");
                        Move(0, -1);
                    }
                    else
                    {
                        int dir = -1;
                        for (int i = 0; i < 4; i++)
                        {
                            if (directions[i] == 0)
                                dir = i;
                        }
                        if (dir == 0)
                        {
                            Move(0, 1);
                        }
                        if (dir == 1)
                        {
                            Move(1, 0);
                        }
                        if (dir == 3)
                        {
                            Move(-1, 0);
                        }
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log(Vector2.Distance(this.gameObject.transform.position, nearestPlayer.transform.position));
        //Debug.Log(new Vector2(this.gameObject.transform.position.x - nearestPlayer.transform.position.x, this.gameObject.transform.position.y - nearestPlayer.transform.position.y));
        if (currentState == States.PATROL)
        {
            if (canMove)
                Patrol();
        }
        if (currentState == States.ATTACKING)
        {
            //Distance of attack 10
            
            if (canMove)
                Attack();
        }
        if (currentState == States.FLEEING)
        {
           if(canMove)
                Flee();

           explodeCounter -= Time.deltaTime;
        }
    }

    //Move machine
    private void Move(int x, int y)
    {
        lastPos = transform.position;

        Vector3 startPoint = transform.position;
        lastPos = startPoint;
        Vector3 endPoint = startPoint + new Vector3(x, y, 0);
        StartCoroutine(smoothTranslation(endPoint));


    }

    IEnumerator smoothTranslation(Vector3 end)
    {

        canMove = false;

        float sqrDist = (transform.position - end).sqrMagnitude;
        while (sqrDist > float.Epsilon)
        {
            Vector3 newPos = Vector3.MoveTowards(rb2d.position, end, (1 / this.botStats.getMoveTime()) * Time.deltaTime);
            rb2d.MovePosition(newPos);
            sqrDist = (transform.position - end).sqrMagnitude;
            /*if (obstacle)
            {
                this.transform.position = lastPos;
                obstacle = false;
                break;
            }*/
            yield return null;
        }

        canMove = true;

    }

    private void PutABomb()
    {
        if (this.botStats.getBombsUsed() < this.botStats.getBombNumber())
        {
            Vector3 pos = this.transform.position;
            Vector3Int cell = tileMap.WorldToCell(pos);
            Vector3 tempPos = tileMap.GetCellCenterWorld(cell);

            GameObject bombToSpawn = Instantiate(bombPrefab, tempPos, Quaternion.identity);
            bombToSpawn.SendMessage("SetStats", this.botStats);

            int temp = this.botStats.getBombsUsed();
            temp++;
            this.botStats.setBombsUsed(temp);
        }

    }
}
