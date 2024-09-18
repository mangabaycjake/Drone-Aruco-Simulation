using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEditor;
using System.IO;
using Unity.VisualScripting;
using TMPro;
using System;
using Unity.VisualScripting.Generated.PropertyProviders;
using JetBrains.Annotations;
using System.Runtime.InteropServices;
public class buttonEvents : MonoBehaviour
{
    //panel
    public GameObject goOptionPanel;
    public GameObject goButtonOptions;
    public InputField ifHeight;
    public InputField ifWidth;


    //camera
    int displayIndex = 0;
    Camera[] allCamera;


    //position
    GameObject goDrone;
    GameObject goDummy;
    public Vector3 posDroneDefault;
    public Vector3 posDummyDefault;
    public Quaternion rotDroneDefault;
    public Quaternion rotDummyDefault;
    public Dropdown ddField;

    float msDelay;
    float unstabilityFactor;

    // cams
     Camera cam1;
     Camera cam2; 
     Camera cam3; 
     Camera cam4; 
     Camera cam5;
    AudioListener audlCam1;
    AudioListener audlCam2;
    AudioListener audlCam3;
    AudioListener audlCam4;
    AudioListener audlCam5;

    void Start()
    {

        //camera
        allCamera = Camera.allCameras;
        displayIndex = 0;
        cam1 = allCamera[0];
        cam2 = allCamera[1];
        cam3 = allCamera[2];
        cam4 = allCamera[3];
        cam5 = allCamera[4];
        audlCam1 = cam1.GetComponent<AudioListener>();
        audlCam2 = cam2.GetComponent<AudioListener>();
        audlCam3 = cam3.GetComponent<AudioListener>();
        audlCam4 = cam4.GetComponent<AudioListener>();
        audlCam5 = cam5.GetComponent<AudioListener>();
        SwitchDisplay();

        HidePanel();

        //position
        goDrone = GameObject.Find("Drone");
        goDummy = GameObject.Find("Dummy");
        posDroneDefault = goDrone.GetComponent<Transform>().position;
        posDummyDefault = goDummy.GetComponent<Transform>().position;
        rotDroneDefault = goDrone.GetComponent<Transform>().rotation;
        rotDummyDefault = goDummy.GetComponent<Transform>().rotation;

        List<string> FieldList = new List<string> { "Green_Field"};
        string assetDirectory ="";
        string[] fileNames;
        string rawDir = Directory.GetCurrentDirectory();

       
        if (rawDir.Contains("Build")) { rawDir = Directory.GetParent(rawDir).FullName; }

        assetDirectory = rawDir + "\\Assets\\Resources\\FieldMaps";
        fileNames = Directory.GetFiles(assetDirectory);

        foreach (string filePath in fileNames)
        {   
            if (Path.GetExtension(filePath) != ".meta"){
                FieldList.Add(Path.GetFileNameWithoutExtension(filePath));
            }
        }     
        ddField.ClearOptions();
        ddField.AddOptions(FieldList);
        SoundVolume();
        ControlDelay();
        UnstabilityNoise();

        // Window
        ifHeight.text = Screen.height.ToString();
        ifWidth.text = Screen.width.ToString();
    }


    bool sdUp = true;
    bool selUp = true;

    int prevScreenWidth = Screen.width;
    int prevScreenHeight = Screen.height;


    private void Update()
    {   

        if (Input.GetKeyUp(KeyCode.F1)) { SwitchDisplay(); }

        if (Input.GetAxis("jsR2") > 0 & Input.GetAxis("jsR1") > 0) {
            if (sdUp) { SwitchDisplay();sdUp = false; }
        }
        else { sdUp = true; }

        if (Input.GetAxis("jsStart") > 0)
        {      
            if (selUp) {
                if (goOptionPanel.activeSelf) {HidePanel();  }
                else { ShowPanel(); }
                selUp = false;
            }
        }
        else { selUp = true; }

        if (prevScreenHeight != Screen.height || prevScreenWidth != Screen.width)
        {
            ifHeight.text = Screen.height.ToString();
            ifWidth.text = Screen.width.ToString();
        }
        prevScreenHeight = Screen.height;
        prevScreenWidth = Screen.width;
        
    }
    public void HidePanel()
    {
        goOptionPanel.SetActive(false);
        goButtonOptions.SetActive(true);
    }

    public void ShowPanel()
    {
        try{
            if (goOptionPanel == null) { Debug.Log("walanya"); }
            goOptionPanel.SetActive(true);
            goButtonOptions.SetActive(false);
        } catch
        {

        }
           
    }
    //public Camera cam1;
    //public Camera cam2;
    //public Camera cam3;
    //public Camera cam4;
    //public Camera cam5;
    public void SwitchDisplay()
    {
        
        if (displayIndex == 0) { cam1.enabled = true; audlCam1.enabled = true; } else { audlCam1.enabled = false; cam1.enabled = false; }
        if (displayIndex == 1) { cam2.enabled = true; audlCam2.enabled = true; } else { audlCam2.enabled = false; cam2.enabled = false; }
        if (displayIndex == 2) { cam3.enabled = true; audlCam3.enabled = true; } else { audlCam3.enabled = false; cam3.enabled = false; }
        if (displayIndex == 3) { cam4.enabled = true; audlCam4.enabled = true; } else { audlCam4.enabled = false; cam4.enabled = false; }
        if (displayIndex == 4) { cam5.enabled = true; audlCam5.enabled = true; } else { audlCam5.enabled = false; cam5.enabled = false; }

        if (allCamera[displayIndex].name == "CamDum")
        {
            goDummy.transform.Rotate(0, 180, 0);
        }

        int lastDispIndex = displayIndex - 1;
        if (lastDispIndex == -1) { lastDispIndex = allCamera.Length - 1; }
        if (allCamera[lastDispIndex].name == "CamDum") { goDummy.transform.Rotate(0, 180, 0); }

        //allCamera[0].enabled = true;
        displayIndex += 1;
        if (displayIndex == allCamera.Length) { displayIndex = 0; }
    }

    public void ResetPosition()
    {        
        goDrone.transform.position = posDroneDefault;
        goDummy.transform.position = posDummyDefault;
        goDrone.transform.rotation = rotDroneDefault;
        goDummy.transform.rotation = rotDummyDefault;
        

    }

    public void QuitDroneSim()
    {
        Application.Quit();
    }
    public void SelectField()
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        string assetDirectory = currentDirectory + "/Assets/Resources/FieldMaps";
             
        string fileName = ddField.options[ddField.value].text;
        Debug.Log("Selected file: " + fileName);
        GameObject baseGround = GameObject.Find("BaseGround");
        if (fileName == "Green_Field")
        {
            Material baseGroundMaterial = new Material(Shader.Find("Standard"));
            baseGroundMaterial.color = Color.green;
            Renderer baseGroundRenderer = baseGround.GetComponent<Renderer>();
            baseGroundRenderer.material = baseGroundMaterial;
        }
        else
        {
            Texture2D fieldMapTexture = Resources.Load<Texture2D>("FieldMaps/" + fileName);
            MeshFilter meshFilter = baseGround.GetComponent<MeshFilter>();
            MeshRenderer meshRenderer = baseGround.GetComponent<MeshRenderer>();
            Material newMaterial = new Material(Shader.Find("Unlit/Texture")); 
            newMaterial.mainTexture = fieldMapTexture;
            meshRenderer.material = newMaterial;
        }
           
      
    }
    public UnityEngine.UI.Slider slSoundVolume;
    public AudioSource adsrcDrone;
    public InputField ifSound;
    public void SoundVolume()
    {
        adsrcDrone.volume = slSoundVolume.value / 100f;
        ifSound.text = (adsrcDrone.volume * 100f).ToString();
    }

    public void SoundVolume_InF()
    {
        int val;
        if (int.TryParse(ifSound.text, out val)) { 
            adsrcDrone.volume = val / 100f;
            slSoundVolume.value = val;
        }
        else { ifSound.text = slSoundVolume.value.ToString(); }
    }
    public UnityEngine.UI.Slider slDelay;
    public InputField ifDelay;
    public void ControlDelay()
    {
        msDelay = slDelay.value;
        ifDelay.text = msDelay.ToString();
    }

    public void ControlDelay_IF()
    {
        int val;
        if (int.TryParse(ifDelay.text, out val))
        {
            msDelay = val;
            slDelay.value = val;
        }
        else { ifDelay.text = slDelay.value.ToString(); }
    }

    public UnityEngine.UI.Slider slUnstability;
    public InputField ifUnstability;
    public void UnstabilityNoise()
    {
        unstabilityFactor = slUnstability.value;
        ifUnstability.text = unstabilityFactor.ToString();
    }

    public void UnstabilityNoise_IF()
    {
        int val;
        if (int.TryParse(ifUnstability.text, out val))
        {
            unstabilityFactor = val;
            slUnstability.value = val;
        }
        else { ifUnstability.text = slUnstability.value.ToString(); }
    }

    
    public void ResolutionUpdate()
    {
        int width;
        int height;

        if (int.TryParse(ifWidth.text, out width) && int.TryParse(ifHeight.text, out height))
        {
            Screen.SetResolution(width, height, Screen.fullScreen);
        }
        else
        {
            ifWidth.text = Screen.width.ToString();
            ifHeight.text = Screen.height.ToString();   
        }
        
    }
   
}
