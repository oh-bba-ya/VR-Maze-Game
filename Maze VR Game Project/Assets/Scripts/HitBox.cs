using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour, IDamageable
{
    public float health = 100;

    private void OnEnable()
    {
        health = 100;
    }

    public void OnDamage(float damageAmount)
    {
        health -= damageAmount;

        if(health <= 0)
        {
            Debug.Log("Destory");
            EnemySpawn.instance.m_CurMonster--;
            EnemySpawn.instance.InsertQueue(gameObject);
        }
    }

}
