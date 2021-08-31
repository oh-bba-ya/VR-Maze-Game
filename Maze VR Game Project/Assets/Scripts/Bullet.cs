using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("Player Hit");
            EnemyFire.m_instance.InsertQueue(gameObject);
        }
        else if(other.tag == "Level")
        {
            Debug.Log("Bullet Destory");
            EnemyFire.m_instance.InsertQueue(gameObject);
        }

    }
}
