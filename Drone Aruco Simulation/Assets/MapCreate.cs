using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class MapCreate : MonoBehaviour
{

    // Changeables
    int c_aruco_per_side = 3;
    float c_aruco_size_m = 0.25f;
    float c_aruco_dist_m = 5;
    float c_floor_side_m = 5; //only used if aruco_dist = 0
    float c_bound_width_m = 20;
    int c_DummyID = 149;
    float c_DummySize = 0.2f;

    public int aruco_per_side; 
    public float aruco_size_m; 
    public float aruco_dist_m; 
    public float floor_size_m; 
    public float bound_width_m;
    public int DummyID;
    public float DummySize;
    public Dropdown ddField;

    [SerializeField] UpdateInput inputupdate;
    


    // Start is called before the first frame update
    void Start()
    {
        // default values
        aruco_per_side = c_aruco_per_side;
        aruco_size_m = c_aruco_size_m;
        aruco_dist_m = c_aruco_dist_m;
        floor_size_m = c_floor_side_m;
        bound_width_m = c_bound_width_m;
        DummyID = c_DummyID;
        DummySize = c_DummySize;

        //Manage dimensions
        if (aruco_dist_m == 0)
        {
            aruco_dist_m = (floor_size_m - aruco_size_m) / (aruco_per_side - 1);
        }
        else
        {
            floor_size_m = aruco_dist_m * (aruco_per_side - 1) + aruco_size_m;
        }
        startPanel();
        startMap();
        GameObject.Find("Dummy").transform.localScale = new Vector3(DummySize, DummySize, DummySize);
    }

    public void startMap()
    {

        Debug.Log("Creating the Map");
        GameObject FindO = GameObject.Find("TempMap");
        if (FindO != null) { Debug.Log(FindO.name); Object.Destroy(FindO.gameObject); }

        GameObject tempmap = new GameObject("TempMap");
        tempmap.transform.parent = GameObject.Find("MapCreate").transform;

        float aruco_size_scale = (float)0.1 * aruco_size_m;
        float aruco_0_loc = (aruco_dist_m * (aruco_per_side - 1)) / 2;
        for (int i = 0; i < aruco_per_side * aruco_per_side; i++)
        {
            // Set the position and scale of the plane for Aruco.
            float posX = 0 - aruco_0_loc + (aruco_dist_m * (i % aruco_per_side));
            float posZ = aruco_0_loc - (aruco_dist_m * ((i - (i % aruco_per_side)) / aruco_per_side));
            Vector3 position = new Vector3(posX, 0.01f, posZ);

            GameObject planeObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            planeObject.transform.parent = tempmap.transform;
            planeObject.transform.position = position;
            planeObject.transform.localScale = new Vector3(aruco_size_scale, 1, aruco_size_scale);
            planeObject.name = "m" + i.ToString();

            // ArUco materials
            string textureAssetPath = "Markers/marker_" + i.ToString();
            Texture2D markerTexture = Resources.Load<Texture2D>(textureAssetPath);
            MeshRenderer meshRenderer = planeObject.GetComponent<MeshRenderer>();

            Material newMaterial = new Material(Shader.Find("Unlit/Texture")); 
            newMaterial.mainTexture = markerTexture;
            meshRenderer.material = newMaterial;

            // ArUco border
            GameObject markerBorder = GameObject.CreatePrimitive(PrimitiveType.Plane);
            markerBorder.transform.parent = tempmap.transform;
            markerBorder.transform.position = new Vector3(posX, 0.008f, posZ); ;
            markerBorder.transform.localScale = new Vector3(1.2f * aruco_size_scale, 1, 1.2f * aruco_size_scale);
            MeshRenderer mrMarkerBoarder = markerBorder.GetComponent<MeshRenderer>();
            mrMarkerBoarder.material = new Material(Shader.Find("Unlit/Texture"));
        }
        // Base Ground
        GameObject baseGround = GameObject.CreatePrimitive(PrimitiveType.Cube);
        baseGround.transform.localScale = new Vector3(floor_size_m, 0.01f, floor_size_m);
        baseGround.transform.position = new Vector3(0, -0.005f, 0);
        Renderer baseGroundRenderer = baseGround.GetComponent<Renderer>();
        
        Rigidbody rbBase = baseGround.AddComponent<Rigidbody>();
        baseGround.transform.parent = tempmap.transform;
        baseGround.name = "BaseGround";
        rbBase.useGravity = false;
        rbBase.isKinematic = true;

        string mapFileName = ddField.options[ddField.value].text;
        if (ddField == null) {
            mapFileName = "Green_Field";
        }
        
        if (mapFileName == "Green_Field")
        {
            Material baseGroundMaterial = new Material(Shader.Find("Standard"));
            baseGroundMaterial.color = Color.green;
            baseGroundRenderer.material = baseGroundMaterial;
        }
        else
        {
            Texture2D fieldMapTexture = Resources.Load<Texture2D>("FieldMaps/" + mapFileName);
            MeshFilter meshFilter = baseGround.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = baseGround.GetComponent<MeshRenderer>();
            Material newMaterial = new Material(Shader.Find("Unlit/Texture")); // You can use a different shader if needed.
            newMaterial.mainTexture = fieldMapTexture;
            meshRenderer.material = newMaterial;
        }

      

        // Boundary Ground
        GameObject boundGround = GameObject.CreatePrimitive(PrimitiveType.Cube);
        float bound_side = floor_size_m + bound_width_m;
        boundGround.transform.localScale = new Vector3(bound_side, 1, bound_side);
        boundGround.transform.position = new Vector3(0, -0.51f, 0);
        Renderer boundGroundRenderer = boundGround.GetComponent<Renderer>();
        Material blueMaterial = new Material(Shader.Find("Standard"));
        blueMaterial.color = Color.blue;
        boundGroundRenderer.material = blueMaterial;
        Rigidbody rbBound = boundGround.AddComponent<Rigidbody>();
        boundGround.transform.parent = tempmap.transform;
        boundGround.name = "BoundGround";
        rbBound.useGravity = false;
        rbBound.isKinematic = true;

        // Custom Objects
        GameObject.Find("SUV").transform.position = new Vector3(0, 0, 4.5f + floor_size_m / 2f);
        GameObject.Find("People").transform.position = new Vector3(0, 0, -1f - floor_size_m / 2f);
        GameObject.Find("Bench").transform.position = new Vector3(-1f - floor_size_m / 2f, 0, 0);
        GameObject.Find("Furnitures").transform.position = new Vector3(1f +  floor_size_m / 2f, 0, 0);


    }

    public InputField IFLMarkerSize;
    public InputField IFLMarkerPerSide;
    public InputField IFLDistance;
    public InputField IFLFloorSize;
    public InputField IFLBoundary;
    public InputField IFLDummyID;
    public InputField IFLDummySize;
    void startPanel()
    {
        IFLMarkerSize.text = aruco_size_m.ToString();
        IFLMarkerPerSide.text = aruco_per_side.ToString();
        IFLDistance.text = aruco_dist_m.ToString();
        IFLFloorSize.text = floor_size_m.ToString();
        IFLBoundary.text = bound_width_m.ToString();
        IFLDummyID.text = DummyID.ToString();
        IFLDummySize.text = DummySize.ToString();
        Texture2D markerTexture = Resources.Load<Texture2D>("Markers/marker_" + DummyID.ToString());
        GameObject.Find("DummyPlane").GetComponent<MeshRenderer>().material.mainTexture = markerTexture;
    }

    void StartButtons()
    {

    }
}
