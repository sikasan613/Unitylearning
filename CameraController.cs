using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //車スクリプト
    public CarController2 Player;
    //車の座標
    public Transform Carpos;
    //カメラ向く座標
    public Transform Targetpos;
    //カメラの座標
    public Transform Camerapos;

    private float speed = 0;
    [Range(0, 50)] public float smothTime = 30;

  
    // Update is called once per frame
    void FixedUpdate()
    {
        follow();
    }
    //カメラ追尾
    private void follow()
    {
        if (Player.isHitGround)
        {
            speed = Player.KPH / smothTime;
            this.transform.position = Vector3.Lerp(this.transform.position, Camerapos.transform.position, Time.deltaTime * speed);
            this.transform.LookAt(Targetpos.transform.position);
        }
    }
}
