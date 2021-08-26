using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector2Int index;

    public bool isForwardWall = true;
    public bool isBackWall = true;
    public bool isLeftWall = true;
    public bool isRightWall = true;

    public GameObject forwardWall;
    public GameObject BackWall;
    public GameObject leftWall;
    public GameObject rightWall;

    // Start is called before the first frame update
    void Start()
    {
        ShowWalls();
    }

    public void ShowWalls()
    {
        forwardWall.SetActive(isForwardWall);
        BackWall.SetActive(isBackWall);
        leftWall.SetActive(isLeftWall);
        rightWall.SetActive(isRightWall);
    }

    /// <summary>
    /// 모든 4면이 활성화 되어 있는지 체크.
    /// </summary>
    /// <returns></returns>
    public bool CheckAllWall()
    {
        return isForwardWall && isBackWall && isRightWall && isLeftWall;
    }
}
