using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public EnemyFire m_enemyFire;

    private GameObject m_bullet;

    private void Start()
    {
        m_bullet = this.gameObject;
        m_enemyFire = m_enemyFire.GetComponent<EnemyFire>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Debug.Log("Player Hit");
        }
        else
        {
            Debug.Log("else Hit");
        }
    }
}
