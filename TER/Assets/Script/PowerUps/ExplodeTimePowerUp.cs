using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeTimePowerUp : PowerUp
{
    public override void Buff(Player player)
    {
        player.setExplodeTime(player.getExplodeTime() - 0.2f);
    }
}
