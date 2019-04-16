using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField]
    private int bombsUsed;
    [SerializeField]
    //Limit 6
    private int radius = 3;
    [SerializeField]
    //Limit 0.2f
    private float moveTime = .5f;
    [SerializeField]
    //Limit 1f;
    private float explodeTime = 2f;
    [SerializeField]
    private int maxHp;
    private int currentHp;
    [SerializeField]
    //Limit 40
    private int damage;
    [SerializeField]
    //Limit 5 
    private int bombNumber;

    //Getters and Setters
    public void setRadius(int r)
    {
        this.radius = r;
    }

    public int getRadius()
    {
        return this.radius;
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
        bombNumber = 1;
        this.currentHp = this.maxHp;
        bombsUsed = 0;
    }

    //Function to regulate stats and set a limit for them
    private void RegulateStats()
    {
        if (this.radius > 6)
            this.radius = 6;
        if (this.moveTime < 0.2f)
            this.moveTime = 0.2f;
        if (this.explodeTime < 1f)
            this.explodeTime = 1f;
        if (this.damage > 40)
            this.damage = 40;
        if (this.bombNumber > 5)
            this.bombNumber = 5;
    }

    // Update is called once per frame
    void Update()
    {
        RegulateStats();
    }
}
