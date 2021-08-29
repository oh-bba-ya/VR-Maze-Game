using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{

    public Animator m_Animator;     // ���� �ִϸ�����
    public Transform m_FireTransform; // �ѱ��� ��ġ�� ��Ÿ���� Ʈ������.
    public ParticleSystem m_MuzzleFlashEffect;          // �ѱ� ȭ�� ȿ�� �����

    public AudioSource m_GunAudioPlayer;            // �� �Ҹ� �����
    public AudioClip m_ShotClip;        // �߻� �Ҹ�
    public AudioClip m_ReloadClip;      // ������ �Ҹ�.

    public LineRenderer m_BulletLineRenderer;           // �Ѿ� ���� ������.

    public Text m_AmmoText;         // ���� źȯ�� ���� ǥ���� UI Text.

    public int m_MaxAmmo = 13;       // �ִ� ź�� ��.
    public float m_TimeBetFire = 0.3f;      // �߻�� �߻� ������ �ð� ����.
    public float m_Damage = 25;
    public float m_ReloadTime = 2.0f;
    public float m_FireDistance = 100f;     // ���� �����Ÿ�

    private enum State { Ready, Empty, Reloading };

    private State m_CurrentState = State.Empty;

    private float m_LastFireTime;           // ���� ���������� �߻��� ����.
    private int m_CurrentAmmo = 0;          // źâ�� ���� ���� ź�� ����.

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentState = State.Empty;           // ź���� �� ���·� ����
        m_LastFireTime = 0;                     // ���������� ���� �� ������ �ʱ�ȭ

        m_BulletLineRenderer.positionCount = 2;         // Line Renderer�� Positions�� size�� ����.
        m_BulletLineRenderer.enabled = false;

        UpdateUI();         // UI ����.
    }


    /// <summary>
    /// �߻� ó���� �õ��ϴ� �Լ�.  
    /// </summary>
    public void Fire()
    {
        // ���� �غ�� ���� AND ���� �ð� >= ������ �߻� ���� + ���� ����
        if(m_CurrentState == State.Ready && Time.time >= m_LastFireTime + m_TimeBetFire)
        {
            m_LastFireTime = Time.time;             // ���������� ���� �� ������ ���� �������� ����

            Shot();
            UpdateUI();
        }

        if(m_CurrentAmmo == 0)
        {
            Reload();
        }
    }

    /// <summary>
    /// ���� �߻� ó���� �ϴ� �κ�.
    /// </summary>
    public void Shot()
    {
        RaycastHit hit;         // ���� ĳ��Ʈ ������ �����ϴ�, �浹 ���� �����̳�

        // ���� ���� �Ѿ��� ���� �� : �ѱ� ��ġ + �ѱ� ��ġ ���� ���� * ���� �Ÿ�
        Vector3 hitPosition = m_FireTransform.position + m_FireTransform.forward * m_FireDistance;

        // ����ĳ��Ʈ(��������,����, �浹 ���� �����̳�, �����Ÿ�)
        if(Physics.Raycast(m_FireTransform.position, m_FireTransform.forward, out hit, m_FireDistance))             // ��� ��ü�� �浹�ϰ� �Ǹ� true�� ����.
        {
            // ������ ������Ʈ IDamageable�� �����´�. ���ٸ� null���� ����.
            IDamageable target = hit.collider.GetComponent<IDamageable>();

            // ������ IDamageable�� �����Ѵٸ�.
            if (target != null)
            {
                target.OnDamage(m_Damage);
            }

            
        }

        // �߻� ����Ʈ ��� ����
        StartCoroutine(ShotEffect(hitPosition));

        m_CurrentAmmo--;

        if(m_CurrentAmmo <= 0)
        {
            m_CurrentState = State.Empty;
        }

    }


    /// <summary>
    /// �߻� ����Ʈ�� ����ϰ� �Ѿ� ������ ��� �׷ȴٰ� ���� �Լ�.
    /// </summary>
    /// <param name="hitPosition"></param>
    /// <returns></returns>
    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        m_Animator.SetTrigger("Fire");          // Fire Ʈ���Ÿ� ���.

        // �Ѿ� ���� �������� ��
        m_BulletLineRenderer.enabled = true;

        // ������ ù��° ���� �ѱ��� ��ġ.
        m_BulletLineRenderer.SetPosition(0, m_FireTransform.position);

        // ������ �ι��� �� ��ġ�� �浹�� ��.
        m_BulletLineRenderer.SetPosition(1, hitPosition);

        // �ѱ� ȭ�� ����Ʈ�� ���
        m_MuzzleFlashEffect.Play();

        // ���� �� �ִ� �Ҹ��� �߻� �Ҹ��� �ƴ϶��.
        if(m_GunAudioPlayer.clip != m_ShotClip)
        {
            m_GunAudioPlayer.clip = m_ShotClip;         // �� �߻� �Ҹ� �ֱ�.
        }
        

        // �Ѱ� �Ҹ� ���
        m_GunAudioPlayer.Play();

        yield return new WaitForSeconds(0.07f);         // ó���� '���' ���� �ð�.

        // 0.07�� �� ������
        m_BulletLineRenderer.enabled = false;

    }


    /// <summary>
    /// ���� ź�� UI�� ���� ź���� �������ش�.
    /// </summary>
    private void UpdateUI()
    {
        if(m_CurrentState == State.Empty)
        {
            m_AmmoText.text = "Empty";
        }
        else if(m_CurrentState == State.Reloading)
        {
            m_AmmoText.text = "Reloading";
        }
        else
        {
            m_AmmoText.text = m_CurrentAmmo.ToString();
        }
    }


    /// <summary>
    /// �������� �õ�.
    /// </summary>
    public void Reload()
    {
        if(m_CurrentState != State.Reloading )
        {
            StartCoroutine(ReloadRoutine());
        }
    }


    /// <summary>
    /// ���� ������ ó���� ����Ǵ� ��.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReloadRoutine()
    {
        m_Animator.SetTrigger("Reloading");            // Reload �Ķ���� Ʈ���Ÿ� ���.
        m_CurrentState = State.Reloading;

        m_GunAudioPlayer.clip = m_ReloadClip;           // ������ �Ҹ� ����.

        m_GunAudioPlayer.Play();            // �Ҹ� ���.

        UpdateUI();

        yield return new WaitForSeconds(m_ReloadTime);              // ������ �ð� ��ŭ ����.

        m_CurrentAmmo = m_MaxAmmo;         // ź�� �ִ� ����.
        m_CurrentState = State.Ready;
        UpdateUI();

    }
}
