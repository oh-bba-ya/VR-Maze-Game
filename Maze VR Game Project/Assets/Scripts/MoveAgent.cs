using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAgent : MonoBehaviour
{
    // 순찰 지점 리스트
    public List<Transform> m_WayPoints;

    // 다음 순찰 지점.
    public int m_NextIdx = 0;

    // NavMesh 저장
    private NavMeshAgent m_Agent;
    private Transform m_EnemyTr;

    // 순찰 속도
    private readonly float m_PatrollerSpeed = 1.5f;
    // 추적 속도
    private readonly float m_TraceSpeed = 4.0f;
    // 회전 속도
    private float m_Damping = 1.0f;

    // 순찰 여부 판단
    private bool m_Patolling;

    



    // Patrolling 프로퍼티 정의
    public bool patrolling
    {
        get { return m_Patolling; }
        set
        {
            m_Patolling = value;
            if (m_Patolling)
            {
                m_Agent.speed = m_PatrollerSpeed;
                // 순찰 상태의 회전계수
                m_Damping = 1.0f;
                MoveWayPoint();
            }
        }
    }

    // 추적 대상의 위치를 저장하는 변수.
    private Vector3 m_TraceTarget;
    public Vector3 TraceTarget
    {
        get { return m_TraceTarget; }
        set
        {
            m_TraceTarget = value;
            m_Agent.speed = m_TraceSpeed;
            // 추적 상태의 회전 계수.
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
        // autoBraking : 목적지에 가까워지면 속도가 느려짐
        m_Agent.autoBraking = false;
        // 자동으로 회전하는 기능 비활성화
        m_Agent.updateRotation = false;
        m_Agent.speed = m_PatrollerSpeed;


        var group = GameObject.Find("WayPoints");

        if(group != null)
        {
            group.GetComponentsInChildren<Transform>(m_WayPoints);
            m_WayPoints.RemoveAt(0);            // 부모 Transform 삭제.
        }

        MoveWayPoint();
    }

    private void MoveWayPoint()
    {
        // 최단 거리 경로 계산이 끝나지 않았다면 다음을 수행하지 않는다.
        if (m_Agent.isPathStale)
        {
            return;
        }

        // 다음 목적지를 wayPoints 배열에서 가져와 설정
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

        // 목적지에 도착 판단.
        if(m_Agent.velocity.sqrMagnitude >= 0.2f * 0.2f && m_Agent.remainingDistance <= 0.5f)
        {
            m_NextIdx = ++m_NextIdx % m_WayPoints.Count;
            MoveWayPoint();
        }
    }
}
