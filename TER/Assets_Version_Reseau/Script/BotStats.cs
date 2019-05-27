using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotStats : MonoBehaviour
{
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

    public void setMoveTime(float value)
    {
        this.moveTime = value;
    }
    
    public float getMoveTime()
    {
        return this.moveTime;
    }

    public void setRadius(int r)
    {
        this.radius = r;
    }

    public int getRadius()
    {
        return this.radius;
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

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
