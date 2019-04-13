using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombNumberPowerUp : PowerUp
{
    public override void Buff(Stats stats)
    {
        stats.setBombNumber(stats.getBombNumber() + 1);
    }

    
}
