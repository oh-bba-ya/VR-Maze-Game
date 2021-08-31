using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFire : MonoBehaviour
{
    public static EnemyFire m_instance;

    private AudioSource m_Audio;
    private Animator m_Animator;
    private Transform m_PlayerTr;
    private Transform m_EnemyTr;

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

    private readonly float m_FireRate = 0.5f;
    private readonly float damping = 10.0f;

    public bool isFire = false;
    public AudioClip m_FireClip;


    public Transform m_FireTransform;

    public GameObject m_Bullet;


    public Queue<GameObject> m_BulletQueue = new Queue<GameObject>();



    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        m_instance = this;

        m_PlayerTr = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        m_EnemyTr = GetComponent<Transform>();
        m_Animator = GetComponent<Animator>();
        m_Audio = GetComponent<AudioSource>();

        m_waitReload = new WaitForSeconds(m_ReloadTime);

        for(int i = 0; i < m_MaxBullet; i++)
        {
            GameObject t_Object = Instantiate(m_Bullet, this.gameObject.transform);
            Debug.Log("Insert Queue Bullet");
            m_BulletQueue.Enqueue(t_Object);
            t_Object.SetActive(false);

        }



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





        //Quaternion rot = Quaternion.LookRotation(m_PlayerTr.position - m_EnemyTr.position);
        Quaternion rot2 = Quaternion.LookRotation(m_PlayerTr.position - m_FireTransform.position);
        m_EnemyTr.rotation = Quaternion.Slerp(m_EnemyTr.rotation, rot2, Time.deltaTime * damping);
        m_FireTransform.rotation = Quaternion.Slerp(m_FireTransform.rotation, rot2, Time.deltaTime * damping);
    }

    public void InsertQueue(GameObject p_Object)
    {
        m_BulletQueue.Enqueue(p_Object);
        p_Object.SetActive(false);
    }

    public GameObject GetQueue()
    {
        GameObject t_Object = m_BulletQueue.Dequeue();
        t_Object.SetActive(true);
        return t_Object;
    }

    void Fire()
    {
        m_Animator.SetTrigger(hashFire);
        m_Audio.PlayOneShot(m_FireClip, 1.0f);

        

        GameObject t_Object = GetQueue();
        t_Object.GetComponent<Rigidbody>().velocity = Vector3.zero;
        t_Object.transform.position = m_FireTransform.position;
        t_Object.transform.rotation = m_FireTransform.rotation;

        t_Object.GetComponent<Rigidbody>().velocity = m_BulletSpeed * m_FireTransform.forward;

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
