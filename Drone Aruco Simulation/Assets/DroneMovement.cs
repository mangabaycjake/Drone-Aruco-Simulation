using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.InputSystem;

using System.Net;
using System.Net.Sockets;
using System.Text;


public class DroneMovement : MonoBehaviour
{
    //using UnityEditor.VersionControl;
    public Rigidbody rbDrone;
    public Transform trDrone;
    public Transform trDroneBody;
    public Transform trPropeller1;
    public Transform trPropeller2;
    public Transform trPropeller3; 
    public Transform trPropeller4;
    public Transform trSteady;
    public Transform trCam1st;
    
    TextMeshProUGUI txtguiVelocity;

    
    //public AudioClip sfxDroneSound;

    float mXratio = 1;
    float mZratio = 1;
    float mYratio = 0.75f;
    float mRratio = (float)Math.PI / 2f;
    float maxturboRatio = 2;
    float tdeltaTime;
    
    bool flying = true;
    bool landing, takingoff;
    bool collided = false;

    float curPropRotSpeed;

    public AudioSource adsrcDrone;
    float msDelay;
    float[][] moveDelay;
    float delayTimer;
    float msintervalTime = 10f; //ms
    public Slider slUnstability;
    float unstabilityFactor;
    float unstabilityX, unstabilityY, unstabilityZ;
    float oldUnsX, oldUnsY, oldUnsZ;
    float unstabilityTime, unstabilityTimeHor;
    bool contraUnstability;
    float initialCam1stRotX;


    void Start()
    {    
        txtguiVelocity = GameObject.Find("TextVelocity").GetComponent<TextMeshProUGUI>();
        moveDelay = new float[1000][];
        for (int i = 0; i < 1000; i++)
        {
            moveDelay[i] = new float[4];
            for (int j = 0; j < 4; j++)
            {
                moveDelay[i][j] = 0;
            }
        }
        if (initialCam1stRotX == 0) { initialCam1stRotX = 12.8f; }
        UnityEngine.Debug.Log("init" + initialCam1stRotX);

    }

    void Update()
    {
       tdeltaTime = Time.deltaTime;

        float xMove = 0;
        float yMove = 0;
        float zMove = 0;
        float rMove = 0;

        if (Input.GetKey(KeyCode.W)) { zMove = 1; }
        if (Input.GetKey(KeyCode.S)) { zMove = -1; }
        if (Input.GetKey(KeyCode.D)) { xMove = 1; }
        if (Input.GetKey(KeyCode.A)) { xMove = -1; }
        if (Input.GetKey(KeyCode.UpArrow)) { yMove = 1; }
        if (Input.GetKey(KeyCode.DownArrow)) { yMove = -1; }
        if (Input.GetKey(KeyCode.RightArrow)) { rMove = 1; }
        if (Input.GetKey(KeyCode.LeftArrow)) { rMove = -1; }

      

        //Joystick Controls
        float jsRightLeft = Input.GetAxis("jsMoveRightLeft");
        float jsForeBack = Input.GetAxis("jsMoveForeBack");
        float jsUpDown = Input.GetAxis("jsMoveUpDown");
        float jsRotate = Input.GetAxis("jsRotateRightLeft");
        float jsTurbo = Input.GetAxis("jsR1");

        if (jsRightLeft != 0) { xMove = jsRightLeft; }
        if (jsForeBack != 0) { zMove = jsForeBack; }
        if (jsUpDown != 0) { yMove = jsUpDown; }
        if (jsRotate != 0) { rMove = jsRotate; }

        float turboMove = maxturboRatio;       
        if (jsTurbo == 0 && !Input.GetKey(KeyCode.LeftShift)) { turboMove = 1; }

        xMove = xMove * mXratio * turboMove;
        zMove = zMove * mZratio * turboMove;
        yMove = yMove * mYratio * turboMove;
        rMove = rMove * mRratio * turboMove;

        if (!landing && !takingoff) { ControlDelay(xMove, yMove, zMove, rMove, out xMove, out yMove, out zMove, out rMove); }

        // Landing / TakeOff Override
        if (Input.GetKey("z") || (Input.GetAxis("jsL2") + Input.GetAxis("jsR2") == 2)) {
            if (flying && !takingoff) { landing = true; } else if (!flying && !landing) { takingoff = true; }
        }
        if (!flying ) { xMove = 0f; yMove = 0f; zMove =  0f; rMove = 0f; }
        if (landing) { Landing(out yMove); xMove = 0f; zMove = 0f; rMove = 0f; }
        if (takingoff) { TakingOff(out yMove); xMove = 0f; zMove = 0f; rMove = 0f; }

        //Inertia
        float lerpFactorX = 0.05f; //lower means slower response, less inertia
        float lerpFactorZ = 0.05f; //lower means slower response, less inertia
        float lerpFactorVer = 0.08f; //hifger neans faster response, more inertia

        if (flying && !takingoff && rbDrone.position.y < 0.12) { yMove = 0.1f * mYratio; lerpFactorVer = 1f; } //minimum altitude

        if (xMove != 0f) { lerpFactorX /= 2; } // rest to move is faster than resting from movement
        if (zMove != 0f) { lerpFactorZ /= 2; } // rest to move
        if (yMove != 0f) { lerpFactorVer /= 4; } // rest to move


        // Unstability
        unstabilityFactor = slUnstability.value / 1000f;
        if (unstabilityTime >= 1.5f + UnityEngine.Random.value * 3f) // Change unstability period in seconds
        {
            unstabilityTime = 0f;        
            unstabilityY = (UnityEngine.Random.value - 0.5f) * 2 * unstabilityFactor * 1.5f;
            unstabilityY = Mathf.Lerp(oldUnsY, unstabilityY, 0.8f);
        }
        else {unstabilityTime += tdeltaTime;}
        if (unstabilityTimeHor >= 0.25f + UnityEngine.Random.value * 2f)
        {
            unstabilityTimeHor = 0f;
            contraUnstability = !contraUnstability;
            if (contraUnstability) { unstabilityX *= -1; unstabilityZ *= 1;
            } else { 
                unstabilityX = (UnityEngine.Random.value - 0.5f) * 2 * unstabilityFactor;
                unstabilityZ = (UnityEngine.Random.value - 0.5f) * 2 * unstabilityFactor;
                unstabilityX = Mathf.Lerp(oldUnsX, unstabilityX, 0.9f);
                unstabilityZ = Mathf.Lerp(oldUnsZ, unstabilityZ, 0.9f);
            }
        } else { unstabilityTimeHor += tdeltaTime; }

        if (!flying || (landing && rbDrone.position.y < 0.75f)) { unstabilityX /= 5f; unstabilityY = 0f; unstabilityZ /= 5f; } // low altitude
 
        // velocity with respect to map axis
        Vector3 targetVelocity = trDrone.forward * zMove + rbDrone.transform.right * xMove + rbDrone.transform.up * yMove;
        float newVelocityX = Mathf.Lerp(rbDrone.velocity.x - oldUnsX, targetVelocity.x, lerpFactorX);
        float newVelocityZ = Mathf.Lerp(rbDrone.velocity.z - oldUnsZ, targetVelocity.z, lerpFactorZ);
        float newVelocityY = Mathf.Lerp(rbDrone.velocity.y - oldUnsY, targetVelocity.y, lerpFactorVer);

        rbDrone.velocity = new Vector3(newVelocityX + unstabilityX, newVelocityY + unstabilityY, newVelocityZ + unstabilityZ);
        rbDrone.angularVelocity = new Vector3(trDrone.right.y * rMove, rMove, trDrone.forward.y * rMove);

        oldUnsX = unstabilityX;
        oldUnsY = unstabilityY;
        oldUnsZ = unstabilityZ;

        // velocity with respect to drone's axis 
        //float velX = rbDrone.velocity.x * rbDrone.transform.forward.z - rbDrone.velocity.z  * rbDrone.transform.forward.x;
        //float velY = rbDrone.velocity.y;
        //float velZ = rbDrone.velocity.z * rbDrone.transform.forward.z + rbDrone.velocity.x * rbDrone.transform.forward.x;

        // velocity with respect to drone's axis minus the unstability
        float velX = newVelocityX * rbDrone.transform.forward.z - newVelocityZ * rbDrone.transform.forward.x;
        float velY = newVelocityY;
        float velZ = newVelocityZ * rbDrone.transform.forward.z + newVelocityX * rbDrone.transform.forward.x;

        //Update Movement Text
        txtguiVelocity.text = "Velocity(m / s):  X: " + velX.ToString("0.000") + " Y: " + velY.ToString("0.000") + " Z: " + velZ.ToString("0.000");



        // ANIMATIONS

        // Rotate the DroneBody's Transform when doing controls
        float incRotSpeed = 23f;
        float incAngle = 8f; //* (0.5f + 0.5f  * turboMove) ;
        Quaternion targetRotation = Quaternion.Euler(incAngle * zMove, 0, -incAngle * xMove);
        trDroneBody.localRotation = Quaternion.Slerp(trDroneBody.localRotation, targetRotation, incRotSpeed * tdeltaTime);


        // Camera rotation
        float zCam, xCam;
        // Do this if based on speed
        //if (Math.Abs(xMove) > mXratio) { zCam = -(xMove - Math.Sign(xMove) * mXratio) / 1.5f; } else { zCam = 0f; }
        //if (Math.Abs(zMove) > mZratio) { xCam = (zMove - Math.Sign(zMove) * mZratio) / 1.5f; } else { xCam = 0f; }
        //Do this if based on speed mode
        if (turboMove > 1)
        {   zCam = -(xMove - Math.Sign(xMove) * mXratio) / maxturboRatio;
            xCam = (zMove - Math.Sign(zMove) * mZratio) / maxturboRatio;}
        else{
            zCam = 0f;
            xCam = 0f; }

        Quaternion camTargetRotation = Quaternion.Euler(initialCam1stRotX + xCam * incAngle, 0, zCam * incAngle);
        trCam1st.localRotation = Quaternion.Slerp(trCam1st.localRotation, camTargetRotation, incRotSpeed * tdeltaTime);

        // Propeller
        float propSpeedY = 360f * 100f;
        float propInertia = 0.04f;
        if (!flying && !landing && !takingoff) //has landed
        {
            if (curPropRotSpeed > 3600f) { curPropRotSpeed = 3600f;  }
            curPropRotSpeed = Mathf.Lerp(curPropRotSpeed, 0, propInertia);
            propSpeedY = curPropRotSpeed;
        }
        else if (takingoff)
        {
            curPropRotSpeed = Mathf.Lerp(curPropRotSpeed, propSpeedY, propInertia / 2);
            propSpeedY = curPropRotSpeed;
        }
        Vector3 propellerSpeed = new Vector3(0, propSpeedY, 0);

        curPropRotSpeed = propSpeedY;

        trPropeller1.Rotate(-propellerSpeed * tdeltaTime);
        trPropeller2.Rotate(-propellerSpeed * tdeltaTime);
        trPropeller3.Rotate(propellerSpeed * tdeltaTime);
        trPropeller4.Rotate(propellerSpeed * tdeltaTime);

        // sounds
        SoundDrone(xMove, yMove, zMove, rMove);


        // Get BAck After Collision
        collided = true;
        if (Math.Round(newVelocityX, 2) != 0) { collided = false; }
        if (Math.Round(newVelocityY, 2) != 0) { collided = false; }
        if (Math.Round(newVelocityZ, 2) != 0) { collided = false; }
        if (rMove != 0) { collided = false; }
        if (trDrone.eulerAngles.x == 0 && trDrone.eulerAngles.y == 0) { collided = false; }
        if (collided)
        {
            Quaternion recoverAngles = Quaternion.Euler(0, trDrone.eulerAngles.y, 0);
            trDrone.rotation = Quaternion.Slerp(trDrone.rotation, recoverAngles, tdeltaTime * 2.5f);
            adsrcDrone.pitch = 1 + 5f * Math.Abs(trDrone.rotation.x) + 5f * Math.Abs(trDrone.rotation.z);
            if (adsrcDrone.pitch > 1.18f) { adsrcDrone.pitch = 1.18f; }
        }
        // Reset Position
        if (!flying && !landing && !takingoff && rbDrone.position.y > 0.20)
        {
            flying = true;
        }

        // SteadyCam
        trSteady.rotation = Quaternion.Euler(0, - trDrone.eulerAngles.y / 180, 0);

  
    }


    public void FixPosition()
    {
        trDrone.rotation = Quaternion.Euler(0, trDrone.eulerAngles.y, 0);
    }

    void SoundDrone(float xmove, float ymove, float zmove, float rmove)
    {
        
        if (ymove < 0) { ymove *= 0.5f; }
        if (ymove < -1) { ymove *= 0.5f; }
        float pastPitch = adsrcDrone.pitch;
        float targetPitch = 1 + 0.06f * Mathf.Max(Math.Abs(xmove), Math.Abs(zmove)) + 0.11f * ymove + 0.02f * Math.Abs(rmove);
        if (targetPitch > 1.20) { targetPitch = 1.2f; }
        if (flying) { adsrcDrone.pitch = Mathf.Lerp(pastPitch, targetPitch, 2.5f); }        
        if (!adsrcDrone.isPlaying && flying) {adsrcDrone.Play(); }
    }

    public AudioSource LandAudio;
    int LandingPhase;
    void Landing(out float ymove)
    {
        if (LandingPhase == 0) {
            if (trDrone.position.y > 3f) {
                ymove = -mYratio * maxturboRatio;
            }
            else if (trDrone.position.y > 1f) {
                ymove = -mYratio; 
            }
            else if (trDrone.position.y > 0.4) {
                ymove = -mYratio * 0.35f;
            }
            else if (trDrone.position.y > 0.125) {
                ymove = -mYratio * 0.15f;
          
            }
            else {
                LandingPhase = 1;
                ymove = -mYratio * 0.001f;
                if (!LandAudio.isPlaying) { LandAudio.PlayDelayed(0.001f); LandAudio.volume = adsrcDrone.volume * 0.9f; }
            }         
        }
        else if (LandingPhase == 1)
        {
            if (trDrone.position.y < 0.127)
            {
                ymove = mYratio * 0.0003f;
                
            }
            else {
                ymove = -mYratio * 0.01f;
                flying = false;
                adsrcDrone.Stop();
                LandingPhase = 2;
            }
        }
        else
        {
            if (trDrone.position.y > 0.05f)
            {
                ymove = -mYratio * 0.1f / trDrone.position.y;
            }
            else
            {
                ymove = 0f;
                LandingPhase = 0;
                landing = false;
            }
        }
     
    }

    public AudioSource TakeOffAudio;
    void TakingOff(out float ymove)
    {
        Quaternion recoverAngles = Quaternion.Euler(0, trDrone.eulerAngles.y, 0);
        trDrone.rotation = Quaternion.Slerp(trDrone.rotation, recoverAngles, tdeltaTime * 3.5f);
        adsrcDrone.pitch = 1 + 5f * Math.Abs(trDrone.rotation.x) + 5f * Math.Abs(trDrone.rotation.z);
        if (adsrcDrone.pitch > 1.18f) { adsrcDrone.pitch = 1.18f; }

        if (curPropRotSpeed < 360f * 35f) { ymove = 0.0f; if (!TakeOffAudio.isPlaying) { TakeOffAudio.PlayDelayed(0.05f); TakeOffAudio.volume = adsrcDrone.volume * 0.7f; } }
        else if (trDrone.position.y < 0.8) { ymove = mYratio; flying = true; }
        else if (trDrone.position.y < 1) { ymove = mYratio * 0.2f * (1.1f - trDrone.position.y) / (1.1f - 0.8f); }
        else { ymove = 0; takingoff = false; }
    }

    public Slider slDelay;
    void ControlDelay(float xmove, float ymove, float zmove, float rmove, out float xMove, out float yMove, out float zMove, out float rMove )
    {
        msDelay = slDelay.value;
        delayTimer += tdeltaTime;
        if (delayTimer >= msintervalTime / 1000f) { 
            for (int i = 999; i > 0; i--)
            {
                moveDelay[0][0] = xmove;
                moveDelay[0][1] = ymove;
                moveDelay[0][2] = zmove;
                moveDelay[0][3] = rmove;

                for (int j = 0; j < 4; j++)
                {
                    moveDelay[i][j] = moveDelay[i - 1][j];
                }
            }
            delayTimer = 0;
        }
        xMove = moveDelay[Mathf.FloorToInt(msDelay / msintervalTime)][0];
        yMove = moveDelay[Mathf.FloorToInt(msDelay / msintervalTime)][1];
        zMove = moveDelay[Mathf.FloorToInt(msDelay / msintervalTime)][2];
        rMove = moveDelay[Mathf.FloorToInt(msDelay / msintervalTime)][3];

        if (msDelay == 0) { xMove = xmove; yMove = ymove; zMove = zmove; rMove = rmove; }
    }

}
