using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAgent : MonoBehaviour
{
    // ���� ���� ����Ʈ
    public List<Transform> m_WayPoints;

    // ���� ���� ����.
    public int m_NextIdx = 0;

    // NavMesh ����
    private NavMeshAgent m_Agent;
    private Transform m_EnemyTr;

    // ���� �ӵ�
    private readonly float m_PatrollerSpeed = 1.5f;
    // ���� �ӵ�
    private readonly float m_TraceSpeed = 4.0f;
    // ȸ�� �ӵ�
    private float m_Damping = 1.0f;

    // ���� ���� �Ǵ�
    private bool m_Patolling;

    



    // Patrolling ������Ƽ ����
    public bool patrolling
    {
        get { return m_Patolling; }
        set
        {
            m_Patolling = value;
            if (m_Patolling)
            {
                m_Agent.speed = m_PatrollerSpeed;
                // ���� ������ ȸ�����
                m_Damping = 1.0f;
                MoveWayPoint();
            }
        }
    }

    // ���� ����� ��ġ�� �����ϴ� ����.
    private Vector3 m_TraceTarget;
    public Vector3 TraceTarget
    {
        get { return m_TraceTarget; }
        set
        {
            m_TraceTarget = value;
            m_Agent.speed = m_TraceSpeed;
            // ���� ������ ȸ�� ���.
            m_Damping = 7.0f;
            TraceTargetFunction(m_TraceTarget);
        }
    }

    public float speed
    {
        get { return m_Agent.velocity.magnitude;  }
    }




    void TraceTargetFunction(Vector3 pos)
    {
        if (m_Agent.isPathStale) return;

        m_Agent.destination = pos;
        m_Agent.isStopped = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_EnemyTr = GetComponent<Transform>();
        m_Agent = GetComponent<NavMeshAgent>();
        // autoBraking : �������� ��������� �ӵ��� ������
        m_Agent.autoBraking = false;
        // �ڵ����� ȸ���ϴ� ��� ��Ȱ��ȭ
        m_Agent.updateRotation = false;
        m_Agent.speed = m_PatrollerSpeed;


        var group = GameObject.Find("WayPoints");

        if(group != null)
        {
            group.GetComponentsInChildren<Transform>(m_WayPoints);
            m_WayPoints.RemoveAt(0);            // �θ� Transform ����.
        }

        MoveWayPoint();
    }

    private void MoveWayPoint()
    {
        // �ִ� �Ÿ� ��� ����� ������ �ʾҴٸ� ������ �������� �ʴ´�.
        if (m_Agent.isPathStale)
        {
            return;
        }

        // ���� �������� wayPoints �迭���� ������ ����
        m_Agent.destination = m_WayPoints[m_NextIdx].position;
        m_Agent.isStopped = false;
    }


    public void Stop()
    {
        m_Agent.isStopped = true;

        m_Agent.velocity = Vector3.zero;
        m_Patolling = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Agent.isStopped == false)
        {

            Quaternion rot = Quaternion.LookRotation(m_Agent.desiredVelocity);
            m_EnemyTr.rotation = Quaternion.Slerp(m_EnemyTr.rotation, rot, Time.deltaTime * m_Damping);
        }


        if (!m_Patolling)
        {
            return;
        }

        // �������� ���� �Ǵ�.
        if(m_Agent.velocity.sqrMagnitude >= 0.2f * 0.2f && m_Agent.remainingDistance <= 0.5f)
        {
            m_NextIdx = ++m_NextIdx % m_WayPoints.Count;
            MoveWayPoint();
        }
    }
}
