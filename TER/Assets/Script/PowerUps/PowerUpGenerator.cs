using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpGenerator : MonoBehaviour
{

    public GameObject[] powerUpList;
   
    [SerializeField]
    private int chanceToSpawnPowerUp = 40;
    

    public void Generate(Vector3 pos)
    {
        int tempRandom = Random.Range(0, 101);
        if(tempRandom <= chanceToSpawnPowerUp)
        {
            int powerUpChance = Random.Range(0, 101);
            if (powerUpChance >= 0 && powerUpChance <= 15)
            {
                //Spawn Range Power Up
                Debug.Log("RANGE");

                Instantiate(powerUpList[0], pos, Quaternion.identity);
            }
            else if(powerUpChance > 15 && powerUpChance <= 30)
            {
                //Spawn Damage Power Up
                Instantiate(powerUpList[2], pos, Quaternion.identity);
            }
            else if(powerUpChance > 30 && powerUpChance <= 50)
            {
                //Spawn Health Power Up ( Heal )
                Instantiate(powerUpList[4], pos, Quaternion.identity);
            }
            else if(powerUpChance > 50 && powerUpChance <= 70)
            {
                //Spawn Speed Power Up 
                Instantiate(powerUpList[6], pos, Quaternion.identity);
            }
            else if(powerUpChance > 70 && powerUpChance <= 80)
            {
                //Spawn Bomb Limit Power Up
                Instantiate(powerUpList[1], pos, Quaternion.identity);
            }
            else if(powerUpChance > 80 && powerUpChance <= 100)
            {
                //Timer
                Instantiate(powerUpList[3], pos, Quaternion.identity);
            }
        }
        else if(tempRandom >= 95)
        {
            Instantiate(powerUpList[5], pos, Quaternion.identity);

        }
    }
}


