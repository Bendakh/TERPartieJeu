using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePowerUp : PowerUp
{
    public override void Buff(Player player)
    {
        player.setDamage(player.getDamage() + 5);
    }
}
