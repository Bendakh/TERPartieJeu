using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeTimePowerUp : PowerUp
{
    public override void Buff(Stats stats)
    {
        stats.setExplodeTime(stats.getExplodeTime() - 0.2f);
        
    }
}
