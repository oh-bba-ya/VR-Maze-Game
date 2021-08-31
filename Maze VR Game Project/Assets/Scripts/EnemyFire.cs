using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    private AudioSource m_Audio;
    private Animator m_Animator;
    private Transform m_PlayerTr;
    private Transform m_EnemyTr;
    public LineRenderer m_BulletLineRenderer;           // ÃÑ¾Ë ±ËÀû ·»´õ·¯.

    private readonly int hashFire = Animator.StringToHash("Fire");
    private readonly int hashReload = Animator.StringToHash("Reload");

    private float m_NextFire = 0.0f;
    public int m_Damage = 25;


    private readonly float m_ReloadTime = 2.0f;
    private readonly int m_MaxBullet = 10;
    private int m_CurBullet = 10;
    private float m_BulletSpeed = 40f;
    private bool isReload = false;
    public AudioClip m_ReloadClip;

    private WaitForSeconds m_waitReload;

    private readonly float m_FireRate = 0.1f;
    private readonly float damping = 10.0f;

    public bool isFire = false;
    public AudioClip m_FireClip;


    public Transform m_FireTransform;

    public float m_FireDistance;


    public Queue<GameObject> m_Queue = new Queue<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        m_PlayerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        m_EnemyTr = GetComponent<Transform>();
        m_Animator = GetComponent<Animator>();
        m_Audio = GetComponent<AudioSource>();

        m_waitReload = new WaitForSeconds(m_ReloadTime);



    }

    // Update is called once per frame
    void Update()
    {
        if (!isReload && isFire)
        {
            if (Time.time >=  m_NextFire )
            {
                Fire();
                m_NextFire = Time.time + m_FireRate + Random.Range(0f, 0.3f);
                
            }
        }


        Quaternion rot = Quaternion.LookRotation(m_PlayerTr.position - m_EnemyTr.position);
        Quaternion rot2 = Quaternion.LookRotation(m_PlayerTr.position - m_FireTransform.position);
        m_EnemyTr.rotation = Quaternion.Slerp(m_EnemyTr.rotation, rot, Time.deltaTime * damping);
        m_FireTransform.rotation = Quaternion.Slerp(m_FireTransform.rotation, rot, Time.deltaTime * damping);
    }

    void Fire()
    {
        m_Animator.SetTrigger(hashFire);
        m_Audio.PlayOneShot(m_FireClip, 1.0f);

        

        isReload = (--m_CurBullet % m_MaxBullet == 0);

        if (isReload)
        {
            StartCoroutine(Reloading());
        }
    }

    IEnumerator Reloading()
    {
        m_Animator.SetTrigger(hashReload);
        m_Audio.PlayOneShot(m_ReloadClip, 1.0f);

        yield return m_waitReload;

        m_CurBullet = m_MaxBullet;
        isReload = false;
    }




}
