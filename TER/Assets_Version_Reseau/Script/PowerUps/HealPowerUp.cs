using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPowerUp : PowerUp
{
    public override void Buff(Stats stats)
    {
        stats.setHp(stats.getHp() + 10);
        
    }
}
