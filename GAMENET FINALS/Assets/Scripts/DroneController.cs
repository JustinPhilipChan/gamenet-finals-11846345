using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    public Rigidbody droneRb;
    [Header("Stats")]
    public float startHp = 100f;
    public float currentHp;
    [Header("Movement Variables")] 
    //default to 50 for testing purposes
    //public float maximumSpeed; not applied yet
    public float movementSpeed; //forward backward
    public float tilt; //rotate left right
    public float verticalSpeed; //up down
    public float horizontalSpeed; //left right

    private Vector3 DroneRotation;

    void Start()
    {
        currentHp = startHp;
    }


    void FixedUpdate()
    {
        //ALL INPUT
        if(Input.GetKey(KeyCode.W)){droneRb.AddRelativeForce(0,0,movementSpeed);droneRb.AddRelativeTorque (10, 0, 0);}//drone fly forward

		if(Input.GetKey(KeyCode.S)){droneRb.AddRelativeForce(0,0,movementSpeed/-1);droneRb.AddRelativeTorque (-10, 0, 0);}//drone fly backward

        if(Input.GetKey(KeyCode.A)) {droneRb.AddRelativeTorque(0,tilt/-1,0);}//tilt drone left

		if(Input.GetKey(KeyCode.D)){droneRb.AddRelativeTorque(0,tilt,0);}//tilt drone right
        
		if(Input.GetKey(KeyCode.I)){droneRb.AddRelativeForce(0,verticalSpeed,0);}//drone fly up

		if(Input.GetKey(KeyCode.K)){droneRb.AddRelativeForce(0,verticalSpeed/-1,0);}//drone fly down

        if(Input.GetKey(KeyCode.J)){droneRb.AddRelativeForce(horizontalSpeed/-1,0,0);droneRb.AddRelativeTorque (0, 0, 10);}//move drone left

		if(Input.GetKey(KeyCode.L)){droneRb.AddRelativeForce(horizontalSpeed,0,0);droneRb.AddRelativeTorque (0, 0, -10);}//move drone right

        //ROTATION
        DroneRotation = droneRb.transform.localEulerAngles;
		if(DroneRotation.z>10 && DroneRotation.z<=180){droneRb.AddRelativeTorque (0, 0, -10);}//if tilt too big(stabilizes drone on z-axis)
		if(DroneRotation.z>180 && DroneRotation.z<=350){droneRb.AddRelativeTorque (0, 0, 10);}//if tilt too big(stabilizes drone on z-axis)
		if(DroneRotation.z>1 && DroneRotation.z<=10){droneRb.AddRelativeTorque (0, 0, -3);}//if tilt not very big(stabilizes drone on z-axis)
		if(DroneRotation.z>350 && DroneRotation.z<359){droneRb.AddRelativeTorque (0, 0, 3);}//if tilt not very big(stabilizes drone on z-axis)

        //stabilizes in the X axis
        if(DroneRotation.x>10 && DroneRotation.x<=180){droneRb.AddRelativeTorque (-10, 0, 0);}//if tilt too big(stabilizes drone on x-axis)
		if(DroneRotation.x>180 && DroneRotation.x<=350){droneRb.AddRelativeTorque (10, 0, 0);}//if tilt too big(stabilizes drone on x-axis)
		if(DroneRotation.x>1 && DroneRotation.x<=10){droneRb.AddRelativeTorque (-3, 0, 0);}//if tilt not very big(stabilizes drone on x-axis)
		if(DroneRotation.x>350 && DroneRotation.x<359){droneRb.AddRelativeTorque (3, 0, 0);}//if tilt not very big(stabilizes drone on x-axis)

        droneRb.AddForce(0,9,0); //a bit of gravity
    }
}
