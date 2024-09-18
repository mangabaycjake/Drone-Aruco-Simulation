using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateText : MonoBehaviour
{
    [SerializeField] MapCreate createdmap;
    TextMeshProUGUI txtMap;
    GameObject goTxtMap;
    TextMeshProUGUI txtguiAltitude;
    
    GameObject goTxtAltitude;
    GameObject goDrone;
    Rigidbody rbDrone;
    float altitude;

    int count_aruco;
    string strTextMap;
    
    void Start()
    {
        goTxtMap = GameObject.Find("TextMap");
        txtMap = goTxtMap.GetComponent<TextMeshProUGUI>();

        goTxtAltitude = GameObject.Find("TextAltitude");
        txtguiAltitude = goTxtAltitude.GetComponent<TextMeshProUGUI>();
        
        goDrone = GameObject.Find("Drone");
        rbDrone = goDrone.GetComponent<Rigidbody>();

    }

    
    void Update()
    {
        count_aruco = createdmap.aruco_per_side;
        count_aruco *= count_aruco;
        strTextMap = "ArUco Size: " + createdmap.aruco_size_m +
            "  No.AruCo: " + count_aruco +
            "  Floor Side: " + createdmap.floor_size_m.ToString("0.00") +
            "  Dist. ArUco: " + createdmap.aruco_dist_m.ToString("0.00");

        txtMap.text = strTextMap;

        Transform trDrone = rbDrone.GetComponent<Transform>();
        altitude = trDrone.position.y;
        txtguiAltitude.text = "Altitude: " + altitude.ToString("0.00");
    }
}
