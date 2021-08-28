using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grenade : MonoBehaviour
{
    public LayerMask m_TargetLayer;         // ���� ������ ������ ����� ����

    public ParticleSystem m_ExplosionEffectPrefab;          // ������ ������ ������ ����Ʈ�� ���� ������.

    public float m_Damage = 100;
    public float m_ExplosionRadius = 5f;         // ���� �ݰ�.
    public float m_TimeToExploade = 5f;         // ������ �ð� �Ŀ� ����.
    public Text m_GrenadeText;

    // ���� ī��� �ٿ��� ���۵� ��
    private bool m_Cooking = false;              // true�� �ɽ� ���� �ð� �Ŀ� ����.


    // ����ź Set (ī��Ʈ �ٿ� ����) , Trigger ��ư Ŭ�� �� ��Ÿ��.
    public void CookGrenade()
    {
        // �̹� ����ź�� Cooking �̶�� , ó���� ����
        if (m_Cooking)
        {
            return;
        }

        m_Cooking = true;
        m_GrenadeText.text = "Cooking";
        // ���� �ð��ڿ� Explode�� ����.
        Invoke("Explode",m_TimeToExploade);

    }

    // ���� ���� ó���� �ϴ� �κ�.
    private void Explode()
    {
        // �Է��� position �������� �Է��� Radius��ŭ �������� ���� ���� �׸� �� �ű⿡ ��ġ�� ��� �浹ü���� �����´�.
        // m_TargetLayer�� �Է������ν� ������ Layer�� �ش��ϴ� �浹ü���� �����´�.
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_ExplosionRadius,m_TargetLayer);


        // ������ �浹ü���� IDamageable�� ������ �ִٸ� �������� ������ �ش�.
        for(int i = 0; i < colliders.Length; i++)
        {
            IDamageable target = colliders[i].GetComponent<IDamageable>();

            if(target != null)
            {
                target.OnDamage(m_Damage);
            }
        }

        // ��ƼŬ ȿ���� ���� ���
        ParticleSystem explosionEffect = Instantiate(m_ExplosionEffectPrefab, transform.position, transform.rotation);
        explosionEffect.Play();

        // ParticleSystem.main.duration ��ƼŬ�� ������ �ִ� ���� �����ð�. �� �ı�
        Destroy(explosionEffect.gameObject, explosionEffect.main.duration);

        // ����ź �ڽ� �ڽ��� �ı�
        Destroy(gameObject);
    }


}
