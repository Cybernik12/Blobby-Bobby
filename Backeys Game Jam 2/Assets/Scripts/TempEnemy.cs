using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TempEnemy : LivingEntity
{
    private float e_maxHealth;
    private float e_currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        e_maxHealth = 3;
        e_currentHealth = e_maxHealth;
    }

    public void E_Damage(int damage)
    {
        e_currentHealth -= damage;

        // Play hurt animation

        if (e_currentHealth <= 0)
        {
            Death();
        }
    }

    public override void Death()
    {
        base.Death();

        Debug.Log("Enemy Defeated!");

        Destroy(this.gameObject);
    }
}
