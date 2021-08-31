using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public float slowdownFactor = 0.05f;
    public float slowdownLength = 2f;
    public void SetTimeScale()
    {
        Time.timeScale = slowdownFactor;

    }

    public void ResetTimeScale()
    {
        Time.timeScale = 1;
    }

}
