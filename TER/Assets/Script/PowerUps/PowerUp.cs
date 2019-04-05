using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public enum PowerUpType
    {
        RANGE,
        DAMAGE,
        HEALTH,
        SPEED,
        BOMB,
        KILLER
    }
    
    public PowerUpType type;

    public abstract void Buff(Player player);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Buff(collision.gameObject.GetComponent<Player>());
            Destroy(this.gameObject);
        }
    }
}
