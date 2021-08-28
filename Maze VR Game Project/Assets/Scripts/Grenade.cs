using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grenade : MonoBehaviour
{
    public LayerMask m_TargetLayer;         // 폭발 감지를 적용할 대상의 필터

    public ParticleSystem m_ExplosionEffectPrefab;          // 폭발할 순간에 생성할 이펙트의 원본 프리팹.

    public float m_Damage = 100;
    public float m_ExplosionRadius = 5f;         // 폭발 반경.
    public float m_TimeToExploade = 5f;         // 설정한 시간 후에 폭발.
    public Text m_GrenadeText;

    // 폭발 카운드 다운이 시작된 후
    private bool m_Cooking = false;              // true가 될시 지연 시간 후에 폭발.


    // 수류탄 Set (카운트 다운 시작) , Trigger 버튼 클릭 후 나타남.
    public void CookGrenade()
    {
        // 이미 수류탄이 Cooking 이라면 , 처리를 종료
        if (m_Cooking)
        {
            return;
        }

        m_Cooking = true;
        m_GrenadeText.text = "Cooking";
        // 지연 시간뒤에 Explode를 실행.
        Invoke("Explode",m_TimeToExploade);

    }

    // 실제 폭발 처리를 하는 부분.
    private void Explode()
    {
        // 입력한 position 기준으로 입력한 Radius만큼 반지름을 가진 구를 그린 후 거기에 겹치는 모든 충돌체들을 가져온다.
        // m_TargetLayer을 입력함으로써 설정한 Layer에 해당하는 충돌체들을 가져온다.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius,m_TargetLayer);


        // 감지된 충돌체들중 IDamageable을 가지고 있다면 데미지를 실제로 준다.
        for(int i = 0; i < colliders.Length; i++)
        {
            IDamageable target = colliders[i].GetComponent<IDamageable>();

            if(target != null)
            {
                target.OnDamage(m_Damage);
            }
        }

        // 파티클 효과를 생성 재생
        ParticleSystem explosionEffect = Instantiate(m_ExplosionEffectPrefab, transform.position, transform.rotation);
        explosionEffect.Play();

        // ParticleSystem.main.duration 파티클이 가지고 있는 지속 유지시간. 후 파괴
        Destroy(explosionEffect.gameObject, explosionEffect.main.duration);

        // 수류탄 자시 자신을 파괴
        Destroy(gameObject);
    }


}
