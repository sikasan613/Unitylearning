using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    CarController2 carController2;
    //�`�F�b�N�|�C���g
    public Transform checkPoint;
    //�`�F�b�N�|�C���g�����X�g��
    public List<Transform> checkPositions = new List<Transform>();
    //���݂̃`�F�b�N�|�C���g���m�F���邽��
    public bool[] chekPointarray;
    //���݂̃`�F�b�N�|�C���g
    private int m_currentCheckPos = 0;
    //�`�F�b�N�|�C���g�̍ő吔
    private int m_maxCheckPoint;

    // Start is called before the first frame update
    void Start()
    {
        carController2 = GetComponent<CarController2>();
        Transform[] checkPosTransforms = checkPoint.GetComponentsInChildren<Transform>();
        chekPointarray = new bool[checkPosTransforms.Length-1];
        m_maxCheckPoint = checkPosTransforms.Length - 1;
        for (int i = 0; i < checkPosTransforms.Length; i++)
        {
            if(checkPosTransforms[i] != checkPoint.transform)
            {
                checkPositions.Add(checkPosTransforms[i]);
            }
        }
    }

    //�Q�b�^�[
    public int currentCheckPos
    {
        get
        {
            return m_currentCheckPos;
        }
    }
    public int maxCheckPoint
    {
        get
        {
            return m_maxCheckPoint;
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckPointDistance();
        if (gameObject.tag == "Player1")
        {
            //�A�N�Z��
            if (Input.GetKey(KeyCode.Keypad8))
            {
                carController2.forward = true;
            }
            //�u���[�L
            else if (Input.GetKey(KeyCode.Keypad5))
            {
                carController2.back = true;
            }
            //���ɐ؂�
            if (Input.GetKey(KeyCode.Keypad4))
            {
                carController2.left = true;
            }
            //�E�ɐ؂�
            else if (Input.GetKey(KeyCode.Keypad6))
            {
                carController2.right = true;
            }
            //�M�A���グ��
            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                carController2.gearshiftup = true;
            }
            //�M�A��������
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                carController2.gearshiftdown = true;
            }
            //
            RaceManager.instance.SpeedDisplay(carController2.KPH, 1);
            RaceManager.instance.GoalDistance(transform.position, chekPointarray, checkPositions,1);
            RaceManager.instance.DisplayGear(carController2.currentGearNum, 1);
        }
        else if(gameObject.tag == "Player2")
        {
            //�A�N�Z��
            if (Input.GetKey(KeyCode.W))
            {
                carController2.forward = true;
            }
            //�u���[�L
            else if (Input.GetKey(KeyCode.S))
            {
                carController2.back = true;
            }
            //���ɐ؂�
            if (Input.GetKey(KeyCode.A))
            {
                carController2.left = true;
            }
            //�E�ɐ؂�
            else if (Input.GetKey(KeyCode.D))
            {
                carController2.right = true;
            }
            //�M�A���グ��
            if (Input.GetKeyDown(KeyCode.E))
            {
                carController2.gearshiftup = true;
            }
            //�M�A��������
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                carController2.gearshiftdown = true;
            }
            RaceManager.instance.SpeedDisplay(carController2.KPH, 2);
            RaceManager.instance.GoalDistance(transform.position, chekPointarray, checkPositions,2);
            RaceManager.instance.DisplayGear(carController2.currentGearNum, 2);
        }
    }
    //�`�F�b�N�|�C���g�Ƃ̋����Ɠ���������
    void CheckPointDistance()
    {
        if (chekPointarray[m_maxCheckPoint-1] == false)
        {
            if (Vector3.Distance(transform.position, checkPositions[m_currentCheckPos].position) < 18f)
            {
                chekPointarray[m_currentCheckPos] = true;
                m_currentCheckPos++;

            }
        }
    }

}
