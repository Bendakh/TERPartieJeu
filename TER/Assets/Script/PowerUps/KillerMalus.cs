using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillerMalus : PowerUp
{
    public override void Buff(Stats stats)
    {
        stats.setHp(stats.getHp() - stats.getMaxHp());
    }
}
