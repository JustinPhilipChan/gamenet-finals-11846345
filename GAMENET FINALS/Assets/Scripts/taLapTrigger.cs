using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class taLapTrigger : MonoBehaviourPunCallbacks
{
    //awake set timeleft to 35 so that once the game starts u end up with 30

    public List<GameObject> lapTriggers = new List<GameObject>();
    public Text countDownText;
    public float timeLeft = 30f;
    public bool canStart = false;

    public enum RaiseEventsCode
    {
        WhoFinishedEventCode = 0,
        WhoDiedEventCode = 0

    }

    private int finishOrder = 0;

    private void OnEnable() 
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }
    
    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent) 
    {
        if(photonEvent.Code == (byte) RaiseEventsCode.WhoFinishedEventCode) {
            object[] data = (object[]) photonEvent.CustomData;

            string nickNameOfFinishedPlayer = (string) data[0];
            finishOrder = (int) data[1];
            int viewId = (int) data[2];

            Debug.Log(nickNameOfFinishedPlayer + " " + finishOrder);

            GameObject orderUiText = TimeAttackManager.TAinstance.finisherTextsUi[finishOrder - 1];
            orderUiText.SetActive(true);

            if (viewId == photonView.ViewID) { // this is u
                orderUiText.GetComponent<Text>().text = finishOrder + " " + nickNameOfFinishedPlayer + "(YOU)";
                orderUiText.GetComponent<Text>().color = Color.red;
            } 
            
            else if (viewId != photonView.ViewID) {
                orderUiText.GetComponent<Text>().text = finishOrder + " " + nickNameOfFinishedPlayer;
            } 
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject go in TimeAttackManager.TAinstance.lapTriggers) {
            lapTriggers.Add(go);
        }
        canStart = false;
        countDownText.text = "";
    }

    void Update()
    {
        if(photonView.IsMine) {
            if(canStart) {
                timeLeft -= Time.deltaTime;
                photonView.RPC("SetCounterTime", RpcTarget.AllBuffered, timeLeft);
            }

            if (timeLeft <= 0) {
                GetComponent<PlayerSetup>().camera.transform.parent = null;
                GetComponent<DroneMovement>().canControl = false;
            }

            canStart = GetComponent<CountdownManager>().gameStart;
        }   
    }

    [PunRPC]
    public void SetCounterTime(float time) 
    {
        if(photonView.IsMine) {
            if(time > 0) {
                countDownText.text = time.ToString("F1");
            } else {
                countDownText.text = "";
            }
        } else {
            countDownText.enabled = false; //if photonview is not mine set text to false
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if(lapTriggers.Contains(col.gameObject)) {
            int indexOfTrigger = lapTriggers.IndexOf(col.gameObject);

            lapTriggers[indexOfTrigger].SetActive(false);
            timeLeft += 10f;
        }

        if(col.gameObject.tag == "FinishTrigger") {
            GameFinish();
        }
    }

    public void GameFinish()
    {
            GetComponent<PlayerSetup>().camera.transform.parent = null;
            GetComponent<DroneMovement>().canControl = false;
            
            finishOrder++;

            string nickName = photonView.Owner.NickName;
            int viewId = photonView.ViewID;

            //event data
            object[] data = new object[] { nickName, finishOrder, viewId };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.All,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions {
                Reliability = false
            };
            PhotonNetwork.RaiseEvent((byte) RaiseEventsCode.WhoFinishedEventCode, data, raiseEventOptions, sendOptions );
        
    }

}
