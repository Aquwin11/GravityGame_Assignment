using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderScript : MonoBehaviour
{
    public GameManager _manager;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TriggerEnter");
        // Check if the collided object has the "Coin" tag
        if (other.CompareTag("Player"))
        {

            _manager.startTimer = false;
            _manager.gameTimer = 0;
            _manager.GameEndTime("You LOSE!!!");
        }
    }
}
