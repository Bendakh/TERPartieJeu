using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePowerUp : PowerUp
{
    public override void Buff(Stats stats)
    {
        stats.setDamage(stats.getDamage() + 5);
    }
}
