﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTag : NetworkBehaviour
{
    private bool isTarget = false;

    public bool IsTarget { get => isTarget; private set => isTarget = value; }

    private TagSystem tagSystem;

    private void Start()
    {
        tagSystem = TagSystem.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Only perform on local client
        if (!isLocalPlayer)
            return;

        // if character is chaser and other object is a character
        if (!isTarget && other.CompareTag("Player")) 
        {
            CharacterTag otherTag = other.GetComponent<CharacterTag>();
            if(otherTag != null && otherTag.IsTarget)
            {
                // TODO: Let TagSystem know otherTag has been caught.
                Debug.Log($"Chaser: {gameObject.name} caught target: {other.gameObject.name}!");

                uint id = netId;
                uint otherId = other.GetComponent<NetworkIdentity>().netId;

                tagSystem.CmdCaughtTag(id, otherId);
            }
        }
    }

    [ClientRpc]
    public void RpcWasCaught(uint targetId)
    {
        if (!IsTarget)
        {
            Debug.Log($"Your team caught Player {targetId}!");
            return;
        }
        else if(targetId == netId)
        {
            Debug.Log("You were caught!");
        }
    }

    [TargetRpc]
    public void TargetSetTag(NetworkConnection target, bool _isTarget)
    {
        Debug.Log(gameObject.name + "became a " + (_isTarget?"target!":"chaser!"));
        isTarget = _isTarget;
    }
}
