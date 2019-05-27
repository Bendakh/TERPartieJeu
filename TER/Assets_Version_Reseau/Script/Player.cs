using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    //NOTE TO FUTUR SELF: SET A LIMIT FOR THE POWER UPS
    //Misc
    


    //MoveEngine
    private bool obstacle;
    private Vector3 lastPos;
    [SerializeField]
    private bool canMove;
    [SerializeField]
    private Tilemap tileMap;
    private Rigidbody2D rb2d;

    [SerializeField]
    private GameObject bombPrefab;

    private Stats playerStats;
    
    public Stats getPlayerStats()
    {
        return this.playerStats;
    }
   

    

    // Start is called before the first frame update
    void Start()
    {
        playerStats = this.GetComponent<Stats>();
        
        obstacle = false;
        
        canMove = true;
        rb2d = GetComponent<Rigidbody2D>();
        tileMap = GameObject.FindGameObjectWithTag("obstacles").GetComponentInParent<Tilemap>();

        Vector3Int tempPos = tileMap.WorldToCell(this.transform.position);
        this.transform.position = tileMap.GetCellCenterWorld(tempPos);

        
    }

    

    // Update is called once per frame
    void FixedUpdate()
    {      
        if(canMove)
            MoveCompute();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && this.playerStats.getBombsUsed() < playerStats.getBombNumber())
        {
            PutBomb();
            int temp = this.playerStats.getBombsUsed();
            temp++;
            this.playerStats.setBombsUsed(temp);
        }

        RegulateHp();
    }

    private void PutBomb()
    {
        //Bomb bomb = new Bomb(this);

        Vector3 pos = this.transform.position;
        Vector3Int cell = tileMap.WorldToCell(pos);
        
        Vector3 tempPos = tileMap.GetCellCenterWorld(cell);

        GameObject bombToSpawn = Instantiate(bombPrefab, tempPos, Quaternion.identity);
        bombToSpawn.SendMessage("SetStats",this.getPlayerStats());
    }

    void MoveCompute()
    { 
        int h = (int) Input.GetAxisRaw("Horizontal");
        int v = (int) Input.GetAxisRaw("Vertical");

        Vector3 currentPos = tileMap.WorldToCell(this.transform.position); 
		
		// Temporairement
        if(h == 1)
        {
            Move(h, 0);
        }
        else if(h == -1)
        {
            Move(h, 0);
        }
        else if(v == 1)
        {
            Move(0, v);
        }
        else if(v == -1)
        {
            Move(0, v);
        }
    }

    private void Move(int x, int y)
    {
        lastPos = transform.position;
       
        Vector3 startPoint = transform.position;
        lastPos = startPoint;
        Vector3 endPoint = startPoint + new Vector3(x, y, 0);
		
		// le virer
		GameObject nm = GameObject.FindWithTag("NetworkManager");
		nm.GetComponent<NetworkManager>().changePosition(startPoint, endPoint, true);

        Debug.Log("POS "+endPoint[0] + "  - "+endPoint[1]);
        StartCoroutine(smoothTranslation(endPoint));
        
        
    }

    IEnumerator smoothTranslation(Vector3 end)
    {
        
        canMove = false;
      
        float sqrDist = (transform.position - end).sqrMagnitude;
        while (sqrDist > float.Epsilon)
        {
            Vector3 newPos = Vector3.MoveTowards(rb2d.position, end, (1 / playerStats.getMoveTime()) * Time.deltaTime);
            rb2d.MovePosition(newPos);
            sqrDist = (transform.position - end).sqrMagnitude;
            if (obstacle)
            {             
                this.transform.position = lastPos;
                obstacle = false;
                break;
            }
            yield return null;
        }
		
		GameObject nm = GameObject.FindWithTag("NetworkManager");
		nm.GetComponent<NetworkManager>().changePosition(transform.position, transform.position, false);
   
        canMove = true;
        
    }

    public void TakeDamage(int dmg)
    {
        int tempHp = this.playerStats.getHp();
        tempHp -= dmg;
        this.playerStats.setHp(tempHp);

        RegulateHp();

        if (this.playerStats.getHp() <= 0)
            Die();
    }

    private void RegulateHp()
    {
        if (this.playerStats.getHp() > this.playerStats.getMaxHp())
            this.playerStats.setHp(this.playerStats.getMaxHp());
        if (this.playerStats.getHp() < 0)
            this.playerStats.setHp(0);
    }

    private void Die()
    {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        obstacle = true; 
    }
}
