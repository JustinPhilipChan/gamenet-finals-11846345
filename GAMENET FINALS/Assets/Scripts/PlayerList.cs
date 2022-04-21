﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerList : MonoBehaviour
{
    [Header("UI References")] 
    public Text PlayerNameText;
    public Button PlayerReadyButton;

    private bool isPlayerReady = false;

    public void Initialize(int playerId, string playerName)
    {
        PlayerNameText.text = playerName;

        if(PhotonNetwork.LocalPlayer.ActorNumber != playerId) {
            PlayerReadyButton.gameObject.SetActive(false);
        } else {
            //sets custom property for each player "isPlayerReady"
            ExitGames.Client.Photon.Hashtable initializedProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_READY, isPlayerReady } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initializedProperties);

            PlayerReadyButton.onClick.AddListener(() => 
            { 
                isPlayerReady = !isPlayerReady;
                SetPlayerReady(isPlayerReady);
                
                ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable() { { Constants.PLAYER_READY, isPlayerReady } };

                PhotonNetwork.LocalPlayer.SetCustomProperties(newProperties);
            });
        }
    }

    public void SetPlayerReady(bool playerReady)
    {
        if (playerReady == true) {
            PlayerReadyButton.GetComponentInChildren<Text>().text = "I'm G";
        } else {
            PlayerReadyButton.GetComponentInChildren<Text>().text = "Ready?";
        }
    }
}
