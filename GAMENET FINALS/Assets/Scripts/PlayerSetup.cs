using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public Camera camera;
    public Canvas myCanvas;

    void Start()
    {
        this.camera = transform.Find("Camera").GetComponent<Camera>();
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc")) {
            GetComponent<DroneMovement>().enabled = photonView.IsMine;
            GetComponent<rcLapTrigger>().enabled = photonView.IsMine;
            GetComponent<CountdownManager>().raceGame = true;
            GetComponent<taLapTrigger>().enabled = false;
            camera.enabled = photonView.IsMine;
        } 
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("ta")) {
            GetComponent<CountdownManager>().raceGame = false;
            GetComponent<DroneMovement>().enabled = photonView.IsMine;
            GetComponent<rcLapTrigger>().enabled = false;
            GetComponent<taLapTrigger>().enabled = photonView.IsMine;
            camera.enabled = photonView.IsMine;
        }
    }

    
}
