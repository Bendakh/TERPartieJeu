using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePowerUp : PowerUp
{
    public override void Buff(Player player)
    {
        player.setRadius(player.getRadius() + 1);
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
