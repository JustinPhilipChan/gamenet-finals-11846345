Drone Explorer

Project:
This project enables the exploration of 3D space by way of a quad-copter drone. 
The intent is to provide a quick drop in package to allow a user to explore a 
scene in the Unity Play engine. The code is written to be modular and can incorporate 
other gameplay elements with the addition of more scripts.

Author: Wilson Sauders 
Email:  HamsterUnity@gmail.com
Date: 03/14/2022 (Happy Pie day)

Versions
1.0.0 - 03/14/2022 
* Initial Release
1.0.1 - 03/20/2022 
* Added colors to sample scene
* Replaced spheres and cube with a drone mesh
* Added bullet and bullet Pooling
* Added gun object and link to CmnInterrface
* Added Popup targets and controller scripts to pop them up in groups
* Created combat and noncombat prefabs and demo scenes


Importing:
The package should unpack to folder:
Assets\Packages\DroneExplorer\
A sample scene can be interacted with at:
Assets\Packages\DroneExplorer\Scene\DemoDrone

The Demo Scenes should be playable with the mouse keyboard interface with the 
standard unity project. If the user wants to import the code into a Demo Scene 
they can drag the DroneContainer prefab from the prefabs folder into the scene. 
The user must then link the Scene's Main Camera into the DroneLook Object's "CamObj" 
variable to bind the camera to the interface.

If the user wants to import the Combat system into a custom scene import the 
DemoCombat prefab then linke the Scene's Main Camera into the DroneLook Object's "CamObj" 
variable to bind the camera to the interface.

Controls: 
With the Default Mouse Keyboard interface the controls are as follows:
Forward / Back:				W / S
Left / Right: 				A / D
Go Up: 					Space
Go Down: 				Left Shift
Pan Camera Up / Down / Left / Right: 	Mouse Movement

With Gamepad:
The user must add the following joystick axes to the 
Project Setting -> Input Manager Axes List:

Thrust:  				Joystick Axis 3rd Axis
LookX:  				Joystick Axis 4th Axis
LookY:  				Joystick Axis 5th Axis

Once these parameters are set the user must disable the 
DroneContainer->CommonInterface->PlayerMouseKeyboard object
and enable the 
DroneContainer->CommonInterface->PlayerXbox object.

With the XBox Gamepad:

Forward / Back / Left / Right:  	Left Stick
Pan Camera Up / Down / Left / Right: 	Right Stick
Go Up:  				Right Trigger
Go Down: 				Left Trigger

Prefabs:
Bullet - This contains a single bullet with a trail and particle system for impact explosion. THe 
prfab should not be placed in the scene but rather referenced by BulletPool.

BulletPool - This instantiates multiple bullet objects and will create a static Instance reference 
to itself which all other objects in the game can reference to spawn bullets. Import this prefab into 
scene if you want to shoot bullets in it.

Licencing:
This package is provided as is. The original developer (Wilson Saunders) is under no obligation to support or maintain it. The code associated prefab files are free to include in Games and Unity Asset Packs with or without attribution to the original developer.