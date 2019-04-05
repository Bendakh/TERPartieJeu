using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPowerUp : PowerUp
{
    public override void Buff(Player player)
    {
        player.setHp(player.getHp() + 10);
        
    }
}
