using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bomb : MonoBehaviour
{
    public bool exploded;
    private bool thereIsBomb;
    private ArrayList toExplode;

    [SerializeField]
    private float countdown;

    private Stats owner;

    [SerializeField]
    private Tilemap tileMap;

    [SerializeField]
    private Tile wall;

    [SerializeField]
    private Tile destructible;

    [SerializeField]
    private GameObject explosionPrefab;

    private int radius;
    private float explodeTime;
    [SerializeField]
    private int damage;
    

    //Bomb instantiation, the attribute owner will take the owner of the bomb to take some info from it 
    /*public Bomb(Player owner)
    {
        this.owner = owner;
        this.radius = this.owner.getRadius();
    }*/

    private void SetStats(Stats stats)
    {
        this.owner = stats;
        this.radius = stats.getRadius();
        this.explodeTime = stats.getExplodeTime();
        this.damage = stats.getDamage();
    }

    // Start is called before the first frame update
    void Start()
    {
        this.exploded = false;
        tileMap = GameObject.FindGameObjectWithTag("obstacles").GetComponentInParent<Tilemap>();
        toExplode = new ArrayList();
        //this.radius = 3;
    }

    // Update is called once per frame
    void Update()
    {
        this.explodeTime -= Time.deltaTime;

        if(this.explodeTime <= 0f && !exploded)
        {
            
            Explode(this.gameObject.transform.position);
            
        }

        /*if(thereIsBomb)
        {
            foreach(Bomb b in toExplode)
            {
                b.Explode(b.gameObject.transform.position);
                Destroy(b.gameObject);
            }
        }*/
    }

 
    public void Explode(Vector2 worldPos)
    {
        exploded = true;
        
        Vector3Int posCell = tileMap.WorldToCell(worldPos);
        bool nBool = true;
        bool sBool = true;
        bool eBool = true;
        bool wBool = true;
        CellExplode(posCell);
        for(int i = 1; i < this.radius; i++)
        {  
            if(eBool)
                eBool = CellExplode(posCell + new Vector3Int(i, 0, 0));
            if(wBool)
                wBool = CellExplode(posCell + new Vector3Int(-i, 0, 0));
            if(nBool)
                nBool = CellExplode(posCell + new Vector3Int(0, i, 0));
            if(sBool)
                sBool = CellExplode(posCell + new Vector3Int(0, -i, 0));
        }


        /*if (thereIsBomb)
        {
            foreach (Bomb b in toExplode)
            {
                b.Explode(b.gameObject.transform.position);
                toExplode.Remove(b);
                Destroy(b.gameObject);
            }
        }*/
        /*if (thereIsBomb)
        {
            foreach (Bomb b in toExplode)
            {
                b.Explode(b.gameObject.transform.position);
                Destroy(b.gameObject);
            }
        }*/

        //Square explosion
        /*CellExplode(posCell);
        CellExplode(posCell + new Vector3Int(0, this.radius - 1, 0));
        CellExplode(posCell + new Vector3Int(0, -this.radius + 1, 0));
        for (int i = 1; i < this.radius; i++)
        {
            CellExplode(posCell + new Vector3Int(i, 0, 0));
            CellExplode(posCell + new Vector3Int(-i, 0, 0));
           
        }
        int x = (this.radius - 2);
        for (int j = 1; j < (this.radius - 1); j++)
        {
            
                for (int k = -x; k <= x; k++)
                {
                    CellExplode(posCell + new Vector3Int(k, j, 0));
                    CellExplode(posCell + new Vector3Int(k, -j, 0));
                   
                }
            x--; if (x < 0) break;
        }*/

        int newVal = this.owner.getBombsUsed();
        newVal--;
        this.owner.setBombsUsed(newVal);

        Destroy(gameObject);
    }

    public bool CellExplode(Vector3Int cell)
    {
        Tile tile = tileMap.GetTile<Tile>(cell);

        //See if there is bombs
        thereIsBomb = false;
        Bomb[] bombList = FindObjectsOfType<Bomb>();
        Player[] playerList = FindObjectsOfType<Player>();

        for (int i = 0; i < bombList.Length; i++)
        {
            if(bombList[i].Equals(this.gameObject.GetComponent<Bomb>()))
            {
                //Debug.Log("nope");
                continue;
            }

            Vector3Int bombPos = tileMap.WorldToCell(bombList[i].transform.position);
            if (bombPos == cell && !bombList[i].exploded)
            {
                // toExplode.Add(bombList[i]);
                //Explode the bomb then remove it from the tab TRY THIS BEFORE DOING THE TRIGGERS
                bombList[i].Explode(bombList[i].transform.position);
            }
            
        }

        if (toExplode.Count > 0)
        {
            thereIsBomb = true;
        }

        //Damage to players by comparing positions
        for (int i = 0; i < playerList.Length; i++)
        {
            Vector3Int playerPos = tileMap.WorldToCell(playerList[i].transform.position);
            if (playerPos == cell)
            {
                playerList[i].SendMessage("TakeDamage", this.damage);
            }
        }


        //Actions
        if (tile == wall)
        {
            return false;
        }
        
        else if(tile == destructible)
        {
            //Do the thing
            tileMap.SetTile(cell, null);
            Vector3 posExplTemp = tileMap.GetCellCenterWorld(cell);
            Instantiate(explosionPrefab, posExplTemp, Quaternion.identity);
            FindObjectOfType<PowerUpGenerator>().Generate(posExplTemp);
            return false;
        }
        
        Vector3 posExpl = tileMap.GetCellCenterWorld(cell);
        Instantiate(explosionPrefab, posExpl, Quaternion.identity);

        return true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            GetComponent<BoxCollider2D>().isTrigger = false;
        }
    }
}
