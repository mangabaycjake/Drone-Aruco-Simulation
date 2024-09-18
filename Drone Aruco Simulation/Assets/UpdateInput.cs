using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateInput : MonoBehaviour
{
    string TheString;
    public InputField IFLMarkerSize;
    public InputField IFLMarkerPerSide;
    public InputField IFLDistance;
    public InputField IFLFloorSize;
    public InputField IFLBoundary;
    public InputField IFLDummyID;
    public InputField IFLDummySize;

    [SerializeField] MapCreate createdmap;
    [SerializeField] DummyMovement createdDummy;

    public void UpdatePanelInputs(string goname)
    {
        string val = "";
        float valf = 0;
        int vali = 0;
        bool validIn = true;

        float buIFLMarkerSize = createdmap.aruco_size_m;
        int buIFLMarkerPerSide = createdmap.aruco_per_side;
        float buIFLDistance = createdmap.aruco_dist_m;
        float buIFLFloorSize = createdmap.floor_size_m;
        float buIFLBoundary = createdmap.bound_width_m;
        int buIFLDummyID = createdmap.DummyID;
        float buIFLDummySize = createdmap.DummySize;

        if (goname == "MarkerSize") {
            val = IFLMarkerSize.text;
            if (float.TryParse(val, out valf)){createdmap.aruco_size_m = valf;}
            else { Debug.Log("Invalid input for " + goname); validIn = false; }
        }
        else if (goname == "MarkerPerSide") { 
            val = IFLMarkerPerSide.text;
            if (int.TryParse(val, out vali)){createdmap.aruco_per_side = vali;}
            else { Debug.Log("Invalid input for " + goname); validIn = false; }
        }
        else if (goname == "Distance") {
            val = IFLDistance.text;
            if (float.TryParse(val, out valf)) { createdmap.aruco_dist_m = valf;}
            else { Debug.Log("Invalid input for " + goname); validIn = false; }
        }
        else if (goname == "FloorSize") {
            val = IFLFloorSize.text;
            if (float.TryParse(val, out valf)) { createdmap.floor_size_m = valf;}
            else { Debug.Log("Invalid input for " + goname); validIn = false; }
        }
        else if (goname == "Boundary") {
            val = IFLBoundary.text;
            if (float.TryParse(val, out valf)) { createdmap.bound_width_m = valf; }
            else { Debug.Log("Invalid input for " + goname); validIn = false; }
        }
        else if (goname == "DummyID") {
            val = IFLDummyID.text;
            if (int.TryParse(val, out vali)) { createdmap.DummyID = vali;
                Texture2D markerTexture = Resources.Load<Texture2D>
                    ("Markers/marker_" + vali.ToString());
                GameObject.Find("DummyPlane").GetComponent<MeshRenderer>().
                    material.mainTexture = markerTexture;
            }
            else { Debug.Log("Invalid input for " + goname); validIn = false; }
        }
        else if (goname == "DummySize") { 
            val = IFLDummySize.text;
            if (float.TryParse(val, out valf)){createdmap.DummySize = valf; 
                GameObject.Find("Dummy").transform.localScale = new Vector3(valf, valf, valf); }
            else { Debug.Log("Invalid input for " + goname); validIn = false; }
        }
        else {validIn = false;}

        if (goname == "MarkerSize" || goname == "Distance")
        {
            createdmap.floor_size_m = createdmap.aruco_dist_m * 
                (createdmap.aruco_per_side - 1) + createdmap.aruco_size_m;
        }
        else if (goname == "MarkerPerSide" || goname == "FloorSize")
        {
            createdmap.aruco_dist_m = (createdmap.floor_size_m - createdmap.aruco_size_m)
                / (createdmap.aruco_per_side - 1);
        }
        /*
        if (createdmap.floor_size_m < (createdmap.aruco_dist_m * (createdmap.aruco_per_side - 1))
            + createdmap.aruco_size_m)
        {
            validIn = false;
        }
        */
        if (validIn)
        {
            Debug.Log(goname + " changed to: " + val);
            createdmap.startMap();
        }else{
            Debug.Log("Returning values.");
            createdmap.aruco_size_m = buIFLMarkerSize;
            createdmap.aruco_per_side = buIFLMarkerPerSide;
            createdmap.aruco_dist_m = buIFLDistance;
            createdmap.floor_size_m = buIFLFloorSize;
            createdmap.bound_width_m = buIFLBoundary;
            createdmap.DummyID = buIFLDummyID;
            createdmap.DummySize = buIFLDummySize;
        }        
        IFLMarkerSize.text = createdmap.aruco_size_m.ToString();
        IFLMarkerPerSide.text = createdmap.aruco_per_side.ToString();
        IFLDistance.text = createdmap.aruco_dist_m.ToString();
        IFLFloorSize.text = createdmap.floor_size_m.ToString();
        IFLBoundary.text = createdmap.bound_width_m.ToString();
        IFLDummyID.text = createdmap.DummyID.ToString();
        IFLDummySize.text = createdmap.DummySize.ToString();    
    }
    
    /*public void startPanel()
    {   
        IFLMarkerPerSide.text = createdmap.aruco_per_side.ToString();
        IFLDistance.text = createdmap.aruco_dist_m.ToString();
        IFLFloorSize.text = createdmap.floor_size_m.ToString();
        IFLBoundary.text = createdmap.bound_width_m.ToString();
        IFLDummyID.text = createdmap.DummyID.ToString();
        IFLDummySize.text = createdmap.DummySize.ToString();    
    }*/

}
