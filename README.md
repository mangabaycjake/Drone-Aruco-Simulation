# Drone-Aruco-Simulation
### This is a simple simulation of drone for ArUco markers intended for autonomous navigation.
![Video](https://github.com/mangabaycjake/Drone-Aruco-Simulation/blob/main/Drone%20Aruco%20Simulation/Documentation/Images/Video.gif)

## Drone
![Drone](https://github.com/mangabaycjake/Drone-Aruco-Simulation/blob/main/Drone%20Aruco%20Simulation/Documentation/Images/Drone.png)
- Model based on Ryze Tello
- Estimated speed on meter scale
- Estimated FOV

## ArUco Markers
![Markers](https://github.com/mangabaycjake/Drone-Aruco-Simulation/blob/main/Drone%20Aruco%20Simulation/Documentation/Images/Markers.png)
- Arranged flat in a grid pattern with an equal number on each side.
- Library: 4x4
- Editable size
- Editable distances in between

## Maps
![Maps](https://github.com/mangabaycjake/Drone-Aruco-Simulation/blob/main/Drone%20Aruco%20Simulation/Documentation/Images/Map.png)
- Square
- Editable 2D image
- Editable size
- Markers fitting corner-to-corner

## Dummy Marker
![Dummy](https://github.com/mangabaycjake/Drone-Aruco-Simulation/blob/main/Drone%20Aruco%20Simulation/Documentation/Images/Dummy.png)
- Movable horizontal marker
- Editable size
- Editable marker ID

## Controls
- W, A, S, D for horizontal translation
- Up, Down for vertical translation
- Left, Right for left-right rotation
- T, F, G, H | I, J, K, L for dummy
- Drone control supports joystick
- Taking off and Landing (Z)

## Connectivity to Control Program
- Communication with a separate program using socket
- Exchange command and feedback

## More Features
![Menu](https://github.com/mangabaycjake/Drone-Aruco-Simulation/blob/main/Drone%20Aruco%20Simulation/Documentation/Images/Menu.png)
### Switching Display
- Several cameras available
### Instability
- Lowered stability of the drone
- Adjustable magnitude
### Delay
- Simulate the delay between controls and visual feedaback
### Fixing Position
- Drone might be disaligned after collision


