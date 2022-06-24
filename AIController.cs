using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    //Car�X�N���v�g
    CarController2 carController2;
    //�`�F�b�N�|�C���g���W
    public Transform checkPoint;
    //�`�F�b�N�|�C���g���X�g
    private List<Transform> checkPositions = new List<Transform>();
    //�u���[�L�|�C���g
    public Transform brakepos;
    //�u���[�L�|�C���g���X�g
    private List<Transform> brakPositions = new List<Transform>();
    //���݂̃`�F�b�N�|�C���g��
    private int m_currentCheckPos = 0;
    //���݂̃u���[�L�|�C���g��
    private int currentBrakePos = 0;
    //�J�[�u�̂��߂ɃX�s�[�h���Ƃ���
    private bool curve = false;
    //�`�F�b�N�|�C���g�̊�
    private bool checkPointBetween = false;
   
    // Start is called before the first frame update
    void Start()
    {
        carController2 = GetComponent<CarController2>();
        Transform[] pathTransforms = checkPoint.GetComponentsInChildren<Transform>();
        Transform[] brakPosTransforms = brakepos.GetComponentsInChildren<Transform>();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != checkPoint.transform)
            {
                checkPositions.Add(pathTransforms[i]);
            }
        }
        for (int i = 0; i < brakPosTransforms.Length; i++)
        {
            if(brakPosTransforms[i] != brakepos.transform)
            {
                brakPositions.Add(brakPosTransforms[i]);
            }
        }
    }
    //���݂̃`�F�b�N�|�C���g��
    public int currentCheckPos
    {
        get
        {
            return m_currentCheckPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //�Ȃ��邩�Ȃ��Ȃ������f����
        ApplySteer();
        //�`�F�b�N�|�C���g�̋��������Ĕ��f����
        CheckPointDistance();
        //�u���[�L�|�C���g�̋������Ĕ��f����
        BrakePointDistance();
    }
    //�Ȃ��邩�Ȃ��Ȃ������f����
    void ApplySteer()
    {
        Vector3 relativeVector = transform.InverseTransformPoint(checkPositions[m_currentCheckPos].position);
        float newSteer = relativeVector.x / relativeVector.magnitude;
        if (!curve) 
        {
            if (3.5f > carController2.currentRotate && carController2.currentRotate > -3.5f)
            {
                carController2.forward = true;
            }
            else
            {
                curve = true;
                //�x���̂��߂ɂ���
                Invoke("CurveSpeedDown", 0.5f);
            }
        }
        else
        {
            carController2.back = true;
        }
        if (!checkPointBetween)
        {
            if (newSteer < -0.03)
            {
                carController2.left = true;
            }
            else if (newSteer > 0.03)
            {
                carController2.right = true;
            }
        }
        else
        {
            if (m_currentCheckPos != checkPositions.Count - 1)
            {
                Vector3 betweenVector = Vector3.Lerp(checkPositions[m_currentCheckPos].position, checkPositions[m_currentCheckPos + 1].position, 0.5f);
                Vector3 betweenRelativeVector = transform.InverseTransformPoint(betweenVector);
                float betweeenSteer = betweenRelativeVector.x / betweenRelativeVector.magnitude;


                if (betweeenSteer < -0.04)
                {
                    carController2.left = true;
                }
                else if (betweeenSteer > 0.04)
                {
                    carController2.right = true;
                }
            }
        }
    }
    //�`�F�b�N�|�C���g�̋��������Ĕ��f����
    void CheckPointDistance()
    {
        if(Vector3.Distance(transform.position,checkPositions[m_currentCheckPos].position) < 8f)
        {
            if(m_currentCheckPos == checkPositions.Count)
            {
                m_currentCheckPos = 0;
            }
            else
            {
                m_currentCheckPos++;
                checkPointBetween = false;
            }
        }
        if (m_currentCheckPos != checkPositions.Count - 1)
        {
            Vector3 nextRelativeVector = transform.InverseTransformPoint(checkPositions[m_currentCheckPos + 1].position);
            float nextNewSteer = nextRelativeVector.x / nextRelativeVector.magnitude;
            if (nextNewSteer < -0.1)
            {
                checkPointBetween = true;
            }
            else if (nextNewSteer > 0.1)
            {
                checkPointBetween = true;
            }

        }
    }
    //�u���[�L�|�C���g�̋������Ĕ��f����
    void BrakePointDistance()
    {
        if (Vector3.Distance(transform.position, brakPositions[currentBrakePos].position) < 10f)
        {
            if (currentBrakePos == brakPositions.Count - 1)
            {
                currentBrakePos = 0;
            }
            else
            {
                curve = true;
                currentBrakePos++;
                Invoke("CurveSpeedDown", 1.0f);
            }
        }
    }
    //�x���̂��߂ɂ���
    private void CurveSpeedDown()
    {
        curve = false;
    }
}
