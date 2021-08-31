using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class test : MonoBehaviour
{
    NavMeshAgent agent;

    public Transform target;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);
    }
}
