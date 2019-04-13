using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPowerUp : PowerUp
{
    public override void Buff(Stats stats)
    {
        stats.setMoveTime(stats.getMoveTime() - 0.05f);
    }
    
}
