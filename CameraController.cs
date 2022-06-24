using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //�ԃX�N���v�g
    public CarController2 Player;
    //�Ԃ̍��W
    public Transform Carpos;
    //�J�����������W
    public Transform Targetpos;
    //�J�����̍��W
    public Transform Camerapos;

    private float speed = 0;
    [Range(0, 50)] public float smothTime = 30;

  
    // Update is called once per frame
    void FixedUpdate()
    {
        follow();
    }
    //�J�����ǔ�
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
