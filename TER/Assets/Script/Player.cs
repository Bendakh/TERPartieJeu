using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{
    //NOTE TO FUTUR SELF: SET A LIMIT FOR THE POWER UPS
    //Misc
    [SerializeField]
    private int bombsUsed;


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

    //Stats
    [SerializeField]
    private int radius = 3;
    [SerializeField]
    private float moveTime = .5f;
    [SerializeField]
    private float explodeTime = 2f;
    [SerializeField]
    private int maxHp;
    private int currentHp;
    [SerializeField]
    private int damage;
    [SerializeField]
    private int bombNumber;

    //Getters and Setters
    public void setRadius(int r)
    {
        this.radius = r;
    }

    public float getMoveTime()
    {
        return this.moveTime;
    }

    public void setMoveTime(float mt)
    {
        this.moveTime = mt;
    }

    public float getExplodeTime()
    {
        return this.explodeTime;
    }

    public void setExplodeTime(float et)
    {
        this.explodeTime = et;
    }

    public void setMaxHp(int val)
    {
        this.maxHp = val;
    }

    public int getMaxHp()
    {
        return this.maxHp;
    }

    public void setHp(int val)
    {
        this.currentHp = val;
    }

    public int getHp()
    {
        return this.currentHp;
    }

    public void setDamage(int value)
    {
        this.damage = value;
    }

    public int getDamage()
    {
        return this.damage;
    }

    public void setBombNumber(int value)
    {
        this.bombNumber = value;
    }

    public int getBombNumber()
    {
        return this.bombNumber;
    }

    public void setBombsUsed(int value)
    {
        this.bombsUsed = value;
    }

    public int getBombsUsed()
    {
        return this.bombsUsed;
    }

    // Start is called before the first frame update
    void Start()
    {
        bombsUsed = 0;
        obstacle = false;
        bombNumber = 1;
        canMove = true;
        rb2d = GetComponent<Rigidbody2D>();
        tileMap = GameObject.FindGameObjectWithTag("obstacles").GetComponentInParent<Tilemap>();

        Vector3Int tempPos = tileMap.WorldToCell(this.transform.position);
        this.transform.position = tileMap.GetCellCenterWorld(tempPos);

        this.currentHp = this.maxHp;
    }

    public int getRadius()
    {
        return this.radius;
    }

    // Update is called once per frame
    void FixedUpdate()
    {      
        if(canMove)
            MoveCompute();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && bombsUsed < this.bombNumber)
        {
            PutBomb();
            bombsUsed++;
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
        bombToSpawn.SendMessage("SetStats",this);
    }

    void MoveCompute()
    { 
        int h = (int) Input.GetAxisRaw("Horizontal");
        int v = (int) Input.GetAxisRaw("Vertical");

        Vector3 currentPos = tileMap.WorldToCell(this.transform.position); 

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
        StartCoroutine(smoothTranslation(endPoint));
        
        
    }

    IEnumerator smoothTranslation(Vector3 end)
    {
        
        canMove = false;
      
        float sqrDist = (transform.position - end).sqrMagnitude;
        while (sqrDist > float.Epsilon)
        {
            Vector3 newPos = Vector3.MoveTowards(rb2d.position, end, (1 / moveTime) * Time.deltaTime);
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
   
        canMove = true;
        
    }

    public void TakeDamage(int dmg)
    {
        this.currentHp -= dmg;
        RegulateHp();
        if (this.currentHp <= 0)
            Die();
    }

    private void RegulateHp()
    {
        if (this.currentHp > this.maxHp)
            this.currentHp = this.maxHp;
        if (this.currentHp < 0)
            this.currentHp = 0;
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
