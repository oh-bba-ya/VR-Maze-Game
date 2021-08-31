using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    public enum State
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    public State state = State.PATROL;

    private Transform m_PlayerTr;
    private Transform m_EnemyTr;

    // Animator 컴포넌트 저장
    private Animator m_Animator;

    public Transform m_FireTransform;

    public float m_AttackDis = 5.0f;
    public float m_TraceDis = 10.0f;

    public bool m_IsDie = false;

    private WaitForSeconds m_WaitSecond;

    private MoveAgent m_MoveAgent;

    private EnemyFire m_EnemyFire;

    private readonly int hashRun = Animator.StringToHash("IsRun");
    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("Speed");

    private void Awake()
    {
        var player = GameObject.FindGameObjectWithTag("Player");

        if (player != null) m_PlayerTr = player.GetComponent<Transform>();

        m_EnemyTr = GetComponent<Transform>();

        m_MoveAgent = GetComponent<MoveAgent>();

        m_WaitSecond = new WaitForSeconds(0.3f);

        m_Animator = GetComponent<Animator>();

        m_EnemyFire = GetComponent<EnemyFire>();
    }

    private void OnEnable()
    {
        StartCoroutine(CheckState());
        StartCoroutine(Action());
    }

    IEnumerator Action()
    {
        // 적 캐릭터가 사망할 때까지 무한 루프.
        while (!m_IsDie)
        {
            yield return m_WaitSecond;

            switch (state)
            {
                case State.PATROL:
                    // 총알 발사 정지
                    m_EnemyFire.isFire = false;
                    m_MoveAgent.patrolling = true;
                    m_Animator.SetBool(hashMove, true);
                    break;

                case State.TRACE:
                    m_EnemyFire.isFire = false;
                    m_MoveAgent.TraceTarget = m_PlayerTr.position;
                    m_Animator.SetBool(hashMove, false);
                    m_Animator.SetBool(hashRun, true);
                    break;

                case State.ATTACK:
                    m_MoveAgent.Stop();
                    m_Animator.SetBool(hashMove, false);
                    m_Animator.SetBool(hashRun, false);
                    if (m_EnemyFire.isFire == false) m_EnemyFire.isFire = true;
                    break;

                case State.DIE:
                    m_MoveAgent.Stop();
                    m_EnemyFire.isFire = false;
                    break;
            }
        }
    }

    IEnumerator CheckState()
    {
        while (!m_IsDie)
        {
            if (state == State.DIE) yield break;

            float dis = Vector3.Distance(m_PlayerTr.position, m_EnemyTr.position);

            if (dis <= m_AttackDis)
            {
                RaycastHit hit;
                Debug.DrawRay(m_FireTransform.position, m_FireTransform.forward * m_AttackDis, Color.blue, 0.3f);
                
                if(Physics.Raycast(m_FireTransform.position,m_FireTransform.forward, out hit, m_AttackDis))
                {
                    state = State.ATTACK;

                }          
            }
            else if (dis <= m_TraceDis)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }

            yield return m_WaitSecond;
        }
    }



    // Update is called once per frame
    void Update()
    {
        m_Animator.SetFloat(hashSpeed, m_MoveAgent.speed);
    }
}
