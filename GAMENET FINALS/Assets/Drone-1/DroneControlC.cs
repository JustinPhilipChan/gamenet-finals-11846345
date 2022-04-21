using UnityEngine;
using System.Collections;

public class DroneControlC : MonoBehaviour {
	public Rigidbody Drone;
	public GameObject RButton;
	public GameObject LButton;

		
	   /*Speed*/public int ForwardBackward = 50; 
	   /*Speed*/public int Tilt = 50; 
	   /*Speed*/public int FlyLeftRight = 50;  
	   /*Speed*/public int UpDown = 50; 

	private Vector3 DroneRotation;

	void Update () {
	}

	void FixedUpdate () {
		DroneRotation=Drone.transform.localEulerAngles;
		if(DroneRotation.z>10 && DroneRotation.z<=180){Drone.AddRelativeTorque (0, 0, -10);}//if tilt too big(stabilizes drone on z-axis)
		if(DroneRotation.z>180 && DroneRotation.z<=350){Drone.AddRelativeTorque (0, 0, 10);}//if tilt too big(stabilizes drone on z-axis)
		if(DroneRotation.z>1 && DroneRotation.z<=10){Drone.AddRelativeTorque (0, 0, -3);}//if tilt not very big(stabilizes drone on z-axis)
		if(DroneRotation.z>350 && DroneRotation.z<359){Drone.AddRelativeTorque (0, 0, 3);}//if tilt not very big(stabilizes drone on z-axis)

		if(Input.GetKey(KeyCode.A)) {Drone.AddRelativeTorque(0,Tilt/-1,0);}//tilt drone left
		if(Input.GetKey(KeyCode.D)){Drone.AddRelativeTorque(0,Tilt,0);}//tilt drone right

		if(DroneRotation.x>10 && DroneRotation.x<=180){Drone.AddRelativeTorque (-10, 0, 0);}//if tilt too big(stabilizes drone on x-axis)
		if(DroneRotation.x>180 && DroneRotation.x<=350){Drone.AddRelativeTorque (10, 0, 0);}//if tilt too big(stabilizes drone on x-axis)
		if(DroneRotation.x>1 && DroneRotation.x<=10){Drone.AddRelativeTorque (-3, 0, 0);}//if tilt not very big(stabilizes drone on x-axis)
		if(DroneRotation.x>350 && DroneRotation.x<359){Drone.AddRelativeTorque (3, 0, 0);}//if tilt not very big(stabilizes drone on x-axis)


		Drone.AddForce(0,9,0);//drone not lose height very fast, if you want not to lose height al all change 9 into 9.80665 
		if(Input.GetKey(KeyCode.W)){Drone.AddRelativeForce(0,0,ForwardBackward);Drone.AddRelativeTorque (10, 0, 0);}//drone fly forward

		if(Input.GetKey(KeyCode.LeftArrow)){Drone.AddRelativeForce(FlyLeftRight/-1,0,0);Drone.AddRelativeTorque (0, 0, 10);}//rotate drone left

		if(Input.GetKey(KeyCode.RightArrow)){Drone.AddRelativeForce(FlyLeftRight,0,0);Drone.AddRelativeTorque (0, 0, -10);}//rotate drone right

		if(Input.GetKey(KeyCode.S)){Drone.AddRelativeForce(0,0,ForwardBackward/-1);Drone.AddRelativeTorque (-10, 0, 0);}// drone fly backward

		if(Input.GetKey(KeyCode.UpArrow)){Drone.AddRelativeForce(0,UpDown,0);}//drone fly up

		if(Input.GetKey(KeyCode.DownArrow)){Drone.AddRelativeForce(0,UpDown/-1,0);}//drone fly down
		
	}

}