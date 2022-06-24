using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEffect : MonoBehaviour
{
    //タイヤ痕
    public TrailRenderer[] tireMarks;
    //煙
    public ParticleSystem[] smoke;
    private CarController2 CarController2;
    private bool smokeFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        CarController2 = GetComponent<CarController2>();
    }

    // Update is called once per frame
    void Update()
    {
        //ドリフト
        Drift();
        //スモーク
        activateSmoke();
    }

    private void activateSmoke()
    {
        if (CarController2.playPauseSmoke)
        {
            startSmoke();
        }
        else
        {
            stopSmoke();
        }
    }
    private void Drift()
    {
        if (CarController2.playPauseSmoke)
        {
            startEmitter();
        }
        else
        {
            stopEmitter();
        }
    }
    //スモークを出す
    private void startSmoke()
    {
        if (smokeFlag) return;
        for (int i = 0; i < smoke.Length; i++)
        {
            smoke[i].Play();
        }
        smokeFlag = true;
    }
    //スモークを止める
    private void stopSmoke()
    {
        if (!smokeFlag) return;
        for (int i = 0; i < smoke.Length; i++)
        {
            smoke[i].Stop();
        }
        smokeFlag = false;
    }
    //タイヤ根を出す
    private void startEmitter()
    {
        foreach (TrailRenderer T in tireMarks)
        {
            T.emitting = true;
        }
    }
    //タイヤ根を止める
    private void stopEmitter()
    {
        foreach (TrailRenderer T in tireMarks)
        {
            T.emitting = false;
        }
    }
}
