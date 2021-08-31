using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{

    public static EnemySpawn instance;
    public GameObject m_SpawnPoints;
    public GameObject m_MonsterPrefab;


    public float m_SpanwTime = 2f;
    public int m_MaxMonster = 5;
    public int m_CurMonster = 0;
    public bool isGameOver = false;
    


    private Queue<GameObject> m_MonsterQueue = new Queue<GameObject>();

    void Start()
    {
        instance = this;

        for(int i = 0; i < m_MaxMonster; i++)
        {
            GameObject t_Object = Instantiate(m_MonsterPrefab, this.gameObject.transform);
            m_MonsterQueue.Enqueue(t_Object);
            t_Object.SetActive(false);
        }

        StartCoroutine(MonsterSpawn());
    }

    public void InsertQueue(GameObject p_Object)
    {
        m_MonsterQueue.Enqueue(p_Object);
        p_Object.SetActive(false);
    }

    public GameObject GetQueue()
    {
        GameObject t_Object = m_MonsterQueue.Dequeue();
        t_Object.SetActive(true);

        return t_Object;
    }


    IEnumerator  MonsterSpawn()
    {
        while (!isGameOver)
        {
            if(m_CurMonster < m_MaxMonster)
            {
                yield return new WaitForSeconds(m_SpanwTime);
                int idx = Random.Range(0, 6);
                Transform pos = m_SpawnPoints.transform.GetChild(idx);
                GameObject t_Object = GetQueue();
                t_Object.transform.position = pos.position;
                ++m_CurMonster;
                Debug.Log("Create");
            }
            else
            {
                yield return null;
            }
        }

        
    }

}
