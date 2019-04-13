using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangePowerUp : PowerUp
{
    public override void Buff(Stats stats)
    {
        stats.setRadius(stats.getRadius() + 1);
    }

   
}
