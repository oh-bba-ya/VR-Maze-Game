using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{

    public Animator m_Animator;     // 총의 애니메이터
    public Transform m_FireTransform; // 총구의 위치를 나타내는 트랜스폼.
    public ParticleSystem m_MuzzleFlashEffect;          // 총구 화염 효과 재생기

    public AudioSource m_GunAudioPlayer;            // 총 소리 재생기
    public AudioClip m_ShotClip;        // 발사 소리
    public AudioClip m_ReloadClip;      // 재장전 소리.

    public LineRenderer m_BulletLineRenderer;           // 총알 궤적 렌더러.

    public Text m_AmmoText;         // 남은 탄환의 수를 표시할 UI Text.

    public int m_MaxAmmo = 13;       // 최대 탄약 수.
    public float m_TimeBetFire = 0.3f;      // 발사와 발사 사이의 시간 간격.
    public float m_Damage = 25;
    public float m_ReloadTime = 2.0f;
    public float m_FireDistance = 100f;     // 총의 사정거리

    private enum State { Ready, Empty, Reloading };

    private State m_CurrentState = State.Empty;

    private float m_LastFireTime;           // 총을 마지막으로 발사한 시점.
    private int m_CurrentAmmo = 0;          // 탄창에 남은 현재 탄약 갯수.

    // Start is called before the first frame update
    void Start()
    {
        m_CurrentState = State.Empty;           // 탄약이 빈 상태로 지정
        m_LastFireTime = 0;                     // 마지막으로 총을 쏜 시점을 초기화

        m_BulletLineRenderer.positionCount = 2;         // Line Renderer의 Positions의 size를 설정.
        m_BulletLineRenderer.enabled = false;

        UpdateUI();         // UI 갱신.
    }


    /// <summary>
    /// 발사 처리를 시도하는 함수.  
    /// </summary>
    public void Fire()
    {
        // 총이 준비된 상태 AND 현재 시간 >= 마지막 발사 시점 + 연사 간격
        if(m_CurrentState == State.Ready && Time.time >= m_LastFireTime + m_TimeBetFire)
        {
            m_LastFireTime = Time.time;             // 마지막으로 총은 쏜 시점이 현재 시점으로 갱신

            Shot();
            UpdateUI();
        }

        if(m_CurrentAmmo == 0)
        {
            Reload();
        }
    }

    /// <summary>
    /// 실제 발사 처리를 하는 부분.
    /// </summary>
    public void Shot()
    {
        RaycastHit hit;         // 레이 캐스트 정보를 저장하는, 충돌 정보 컨테이너

        // 총을 쏴서 총알이 맞은 곳 : 총구 위치 + 총구 위치 앞쪽 방향 * 사정 거리
        Vector3 hitPosition = m_FireTransform.position + m_FireTransform.forward * m_FireDistance;

        // 레이캐스트(시작지점,방향, 충돌 정보 컨테이너, 사정거리)
        if(Physics.Raycast(m_FireTransform.position, m_FireTransform.forward, out hit, m_FireDistance))             // 어떠한 물체와 충돌하게 되면 true값 리턴.
        {
            // 상대방의 컴포넌트 IDamageable을 가져온다. 없다면 null값이 들어간다.
            IDamageable target = hit.collider.GetComponent<IDamageable>();

            // 상대방이 IDamageable이 존재한다면.
            if (target != null)
            {
                target.OnDamage(m_Damage);
            }

            
        }

        // 발사 이펙트 재생 시작
        StartCoroutine(ShotEffect(hitPosition));

        m_CurrentAmmo--;

        if(m_CurrentAmmo <= 0)
        {
            m_CurrentState = State.Empty;
        }

    }


    /// <summary>
    /// 발사 이펙트를 재생하고 총알 궤적을 잠시 그렸다가 끄는 함수.
    /// </summary>
    /// <param name="hitPosition"></param>
    /// <returns></returns>
    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        m_Animator.SetTrigger("Fire");          // Fire 트리거를 당김.

        // 총알 궤적 렌더러를 켬
        m_BulletLineRenderer.enabled = true;

        // 선분의 첫번째 점은 총구의 위치.
        m_BulletLineRenderer.SetPosition(0, m_FireTransform.position);

        // 선분의 두번쨰 점 위치는 충돌한 곳.
        m_BulletLineRenderer.SetPosition(1, hitPosition);

        // 총구 화염 이펙트를 재생
        m_MuzzleFlashEffect.Play();

        // 현재 들어가 있는 소리가 발사 소리가 아니라면.
        if(m_GunAudioPlayer.clip != m_ShotClip)
        {
            m_GunAudioPlayer.clip = m_ShotClip;         // 총 발사 소리 넣기.
        }
        

        // 총격 소리 재생
        m_GunAudioPlayer.Play();

        yield return new WaitForSeconds(0.07f);         // 처리를 '잠시' 쉬는 시간.

        // 0.07초 후 실행됌
        m_BulletLineRenderer.enabled = false;

    }


    /// <summary>
    /// 총의 탄약 UI에 남은 탄약을 갱신해준다.
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
    /// 재장전을 시도.
    /// </summary>
    public void Reload()
    {
        if(m_CurrentState != State.Reloading )
        {
            StartCoroutine(ReloadRoutine());
        }
    }


    /// <summary>
    /// 실제 재장전 처리가 진행되는 곳.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReloadRoutine()
    {
        m_Animator.SetTrigger("Reloading");            // Reload 파라미터 트리거를 당김.
        m_CurrentState = State.Reloading;

        m_GunAudioPlayer.clip = m_ReloadClip;           // 재장전 소리 삽입.

        m_GunAudioPlayer.Play();            // 소리 재생.

        UpdateUI();

        yield return new WaitForSeconds(m_ReloadTime);              // 재장전 시간 만큼 쉰다.

        m_CurrentAmmo = m_MaxAmmo;         // 탄약 최대 충전.
        m_CurrentState = State.Ready;
        UpdateUI();

    }
}
