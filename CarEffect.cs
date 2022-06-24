using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEffect : MonoBehaviour
{
    //�^�C����
    public TrailRenderer[] tireMarks;
    //��
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
        //�h���t�g
        Drift();
        //�X���[�N
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
    //�X���[�N���o��
    private void startSmoke()
    {
        if (smokeFlag) return;
        for (int i = 0; i < smoke.Length; i++)
        {
            smoke[i].Play();
        }
        smokeFlag = true;
    }
    //�X���[�N���~�߂�
    private void stopSmoke()
    {
        if (!smokeFlag) return;
        for (int i = 0; i < smoke.Length; i++)
        {
            smoke[i].Stop();
        }
        smokeFlag = false;
    }
    //�^�C�������o��
    private void startEmitter()
    {
        foreach (TrailRenderer T in tireMarks)
        {
            T.emitting = true;
        }
    }
    //�^�C�������~�߂�
    private void stopEmitter()
    {
        foreach (TrailRenderer T in tireMarks)
        {
            T.emitting = false;
        }
    }
}
