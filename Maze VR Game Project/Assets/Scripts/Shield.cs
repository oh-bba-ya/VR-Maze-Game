using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public AudioSource m_AudioSource;
    public AudioClip m_AudioClip;


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Bullet")
        {
            m_AudioSource.clip = m_AudioClip;
            m_AudioSource.Play();
        }
    }



}
