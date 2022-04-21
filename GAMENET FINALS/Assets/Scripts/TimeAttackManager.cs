using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class TimeAttackManager : MonoBehaviour
{
    public GameObject[] vehiclePrefabs;
    public GameObject[] finisherTextsUi;
    public Transform[] startingPositions;
    public Text timeText;
    //public Text countDownText;

    public List<GameObject> lapTriggers = new List<GameObject>();

    public static TimeAttackManager TAinstance = null;

    void Awake()
    {
        if (TAinstance != null) {
            Destroy(this.gameObject);
        } else {
            TAinstance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsConnectedAndReady) {
            object playerSelectionNumber;

            if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(Constants.PLAYER_SELECTION_NUMBER, out playerSelectionNumber)) {
                Debug.Log((int)playerSelectionNumber);

                int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
                Vector3 instantiatePosition = startingPositions[actorNumber - 1].position;
                PhotonNetwork.Instantiate(vehiclePrefabs[(int)playerSelectionNumber].name, instantiatePosition, Quaternion.identity);
            }
        }

        foreach (GameObject go in finisherTextsUi) {
            go.SetActive(false);
        }

    }
}
