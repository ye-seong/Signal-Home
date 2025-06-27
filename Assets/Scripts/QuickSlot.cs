using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuickSlot : MonoBehaviour
{
    public int slotIndex;

    private GameObject player;
    private PlayerState playerState;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            playerState = player.GetComponent<PlayerState>();
        }
    }
}
