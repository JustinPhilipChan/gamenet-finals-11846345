using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DroneMovement : MonoBehaviourPunCallbacks
{
    /* 
        CONTROLS (tentative) 
        W = FORWARD
        S = BACKWARD
        A = ROTATE (turn) to the LEFT (counter-clockwise)
        D = ROTATE (turn) to the RIGHT (clockwise)

        I = move UP (Y)
        K = move DOWN (-Y)
        J = move LEFT (X)
        L = move RIGHT (-X)

        NO GRAVITY (easier)

        Whenever DRONE hits ANYTHING:
        step back a little
        probably a coroutine to stop multiple collision detections (i.e. stop player from receiving continous damage because of the step back movement)
        freeze input in step back coroutine or maybe do the whole thing in 1 coroutine
        reset moveSpeed to i'd say like 10?
    */

    public Rigidbody rb;

    public bool canControl;

    [Header("Stats")]
    public float startHealth = 100;
    public float currentHealth;
    public float maxSpeed;
    public float acceleration;
    public float moveSpeed;
    public float verticalMoveSpeed;
    public float horizontalMoveSpeed;

    //private rotation variables
    private float expectedYRotation;
    private float currentYRotation;
    private float rotateAmount = 1f;
    private float rotationYVelocity;
    Camera myCamera;
    float t = 0.7f;

    void Awake()
    {
        currentHealth = startHealth;
        canControl = false;
        myCamera = GetComponent<PlayerSetup>().camera;
    }
    
    void LateUpdate()
    {
        if (canControl) {
            rb.rotation = Quaternion.Euler(new Vector3(rb.rotation.x, currentYRotation, rb.rotation.z));

            //VERTICAL
            if (Input.GetKey(KeyCode.W)) { //UP
                rb.AddRelativeForce(Vector3.up * verticalMoveSpeed);
                //rb.velocity = transform.up * verticalMoveSpeed;
            } 
            if (Input.GetKey(KeyCode.S)) { //DOWN
                rb.AddRelativeForce(Vector3.up * -verticalMoveSpeed);
                //rb.velocity = -transform.up * verticalMoveSpeed;
            }
            if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S)) { //AUTO STOP
                rb.velocity = new Vector3 (rb.velocity.x, 0, rb.velocity.z);
            }

            //Horizontal
            if (Input.GetKey(KeyCode.D)) {
                rb.velocity = transform.right * horizontalMoveSpeed;
            }
            if (Input.GetKey(KeyCode.A)) {
                rb.velocity = -transform.right * horizontalMoveSpeed;
            }
            if(!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) { //AUTO STOP
                rb.velocity = new Vector3 (0, rb.velocity.y, 0);
            }

            //FORWARD
            if(Input.GetKey(KeyCode.Space)){
                if(moveSpeed < maxSpeed) {
                    moveSpeed += acceleration * Time.deltaTime;
                }
            }
            if (!Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift)) { //NO SPACEBAR = AUTO BRAKE or LEFT SHIFT = BRAKE
                if(moveSpeed > 0)
                    moveSpeed -= acceleration * Time.deltaTime;
                if (moveSpeed < 0) //clamp to 0 when braking
                    moveSpeed = 0;
            }

            if (Input.GetKey(KeyCode.Q)) { //TO THE LEFT
                expectedYRotation -= rotateAmount;
            }
            if (Input.GetKey(KeyCode.E)) { //TO THE RIGHT
                expectedYRotation += rotateAmount;
            }

            rb.AddRelativeForce(0,0,moveSpeed);
            currentYRotation = Mathf.SmoothDamp(currentYRotation, expectedYRotation, ref rotationYVelocity, 0.25f);
        }
    }

    [PunRPC]
    public void TakeDamage()
    {
        currentHealth -= 10;
        StartCoroutine("WasHit");

        if(currentHealth <= 0) {
            GetComponent<PlayerSetup>().camera.transform.parent = null;
            GetComponent<DroneMovement>().enabled = false;
        }
    }

    void OnCollisionEnter(Collision hit) 
    {
        photonView.RPC("TakeDamage", RpcTarget.AllBuffered);
    }

    void OnTriggerEnter(Collider col)
    {
        //BOOST
        if(col.CompareTag("SpeedBoost")) {
            StartCoroutine("Boost");
        }
    }

    public IEnumerator WasHit()
    {   
        canControl = false;
        rb.AddRelativeForce(0,0, -moveSpeed);
        yield return new WaitForSecondsRealtime(0.6f);
        canControl = true;
    }

    public IEnumerator Boost()
    {
        maxSpeed += 2500f;
        moveSpeed += 2500f;
        verticalMoveSpeed += 10;
        horizontalMoveSpeed += 10;

        myCamera.fieldOfView = Mathf.Lerp(myCamera.fieldOfView, 90, t);

        yield return new WaitForSecondsRealtime(0.8f);

        maxSpeed -= 2500f;
        moveSpeed -= 2500f;
        verticalMoveSpeed -= 10;
        horizontalMoveSpeed -= 10;

        myCamera.fieldOfView = Mathf.Lerp(myCamera.fieldOfView, 60, t);
    }

}
