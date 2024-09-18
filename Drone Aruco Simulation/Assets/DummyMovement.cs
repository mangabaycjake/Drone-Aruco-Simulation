using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyMovement : MonoBehaviour
{
    public Rigidbody rbDummy;
    public Transform trDummy;
    public float DummyID;
    public float DummySize;

    float mScaleSpeed = 1f;
    float mXratio = 1f;
    float mZratio = 1f;
    float mYratio = 0.75f;
    float mRratio = (float)Math.PI / 2f;
    float maxAnalogButton;

    void Start()
    {
        rbDummy = GetComponent<Rigidbody>();
        trDummy = GetComponent<Transform>();
    }

    void Update()
    {
        float xMove = 0;
        float yMove = 0;
        float zMove = 0;
        float rMove = 0;

        if (Input.GetKey("t")) { zMove = 1; }
        if (Input.GetKey("g")) { zMove = -1; }
        if (Input.GetKey("h")) { xMove = 1; }
        if (Input.GetKey("f")) { xMove = -1; }
        if (Input.GetKey("i")) { yMove = 1; }
        if (Input.GetKey("k")) { yMove = -1; }
        if (Input.GetKey("l")) { rMove = 1; }
        if (Input.GetKey("j")) { rMove = -1; }

        //Joystick Controls
        /*float jsRightLeft = Input.GetAxis("jsMoveRightLeft");
        float jsForeBack = Input.GetAxis("jsMoveForeBack");
        float jsUpDown = Input.GetAxis("jsMoveUpDown");
        float jsRotate = Input.GetAxis("jsRotateRightLeft");
        if (jsRightLeft != 0) { xMove = jsRightLeft; }
        if (jsForeBack != 0) { zMove = jsForeBack; }
        if (jsUpDown != 0) { yMove = jsUpDown; }
        if (jsRotate != 0) { rMove = jsRotate; }*/

        xMove = xMove * mXratio * mScaleSpeed;
        zMove = zMove * mZratio * mScaleSpeed;
        yMove = yMove * mYratio * mScaleSpeed;
        rMove = rMove * mRratio * mScaleSpeed;

        rbDummy.velocity = transform.forward * zMove + transform.right * xMove + transform.up * yMove;
        rbDummy.angularVelocity = new Vector3(0, rMove, 0);
    }

}
