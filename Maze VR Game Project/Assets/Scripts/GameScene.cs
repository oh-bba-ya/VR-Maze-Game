using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("In Player");

        if(other.tag == "Player")
        {
            SceneManager.LoadScene("Create Maze");
        }
    }
}
