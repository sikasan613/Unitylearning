using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    //Carスクリプト
    CarController2 carController2;
    //チェックポイント座標
    public Transform checkPoint;
    //チェックポイントリスト
    private List<Transform> checkPositions = new List<Transform>();
    //ブレーキポイント
    public Transform brakepos;
    //ブレーキポイントリスト
    private List<Transform> brakPositions = new List<Transform>();
    //現在のチェックポイント数
    private int m_currentCheckPos = 0;
    //現在のブレーキポイント数
    private int currentBrakePos = 0;
    //カーブのためにスピード落とすか
    private bool curve = false;
    //チェックポイントの間
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
    //現在のチェックポイント数
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
        //曲げるか曲げないか判断する
        ApplySteer();
        //チェックポイントの距離を見て反映する
        CheckPointDistance();
        //ブレーキポイントの距離見て反映する
        BrakePointDistance();
    }
    //曲げるか曲げないか判断する
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
                //遅延のためにする
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
    //チェックポイントの距離を見て反映する
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
    //ブレーキポイントの距離見て反映する
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
    //遅延のためにする
    private void CurveSpeedDown()
    {
        curve = false;
    }
}
