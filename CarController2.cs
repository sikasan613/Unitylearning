using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController2 : MonoBehaviour
{
    //マニュアルかオートマにできる
    internal enum gearBox
    {
        automatic,
        manual
    }
    [SerializeField]
    private gearBox gearChange;

    //サウンド管理クラス
    SoundManager soundMgr;
    //レース管理クラス
    RaceManager RaceMgr;
    //車につけるリジッドボディー
    public Rigidbody sphere;
    //煙を出すか
    private bool m_playPauseSmoke = false;
    //車の速度
    [HideInInspector]
    public float KPH;
    //車の最高速度
    [HideInInspector]
    public float maxRPM;
    //車のローテーションの最大値と最小値
    private float max_rotatecar = 30f;
    //車の速度の最大値と最小値
    private float acceleration = 30f;
    //車の曲げれる最大値と最小値
    private float steering = 5f;
    //下に力を加える
    private float gravity = 10f;
    //スフィアレイキャストの半径
    private float m_radius = 0.2f;
    //レイキャストの最大距離
    private float maxUnderDistance = 0.45f;
    private float maxStraightDistance = 8.0f;
    //車についてるスフィアの座標
    private Transform sphereTransform;
    //代入される速度
    private float speed;
    //現在の速度
    private float currentSpeed;
    //代入される回転
    private float rotate;
    //現在の回転
    private float m_currentRotate;
    //ドリフト用の回転
    private float m_rotatecar;
    //ギア数
    private float[] gears = { 1f, 2.5f, 2.8f, 3.0f, 3.5f, 3.7f };

    private float[] gearChangeSpeed = { 40f, 80f, 120f, 140f, 160f,200f};
    //現在のギア数
    private int m_currentGearNum = 0;
    Vector3 prevSpherePos;

    //オブジェクトがあるかチェック
    private bool isPlayer1;
    private bool isPlayer2;
    private bool isAi;
    //バックライトのオブジェクト
    GameObject brakeLight;
    //バックライトの発行に使用
    Material breakeLightIntensity;
    //足元にレイキャストを出す
    RaycastHit underHit;
    RaycastHit straightHit;
    //アクセル
    bool m_forward = false;
    //バック
    bool m_back = false;
    //左に曲げる
    bool m_left = false;
    //右に曲げる
    bool m_right = false;
    //ギアをあげる
    bool m_gearshiftup = false;
    //ギアをあげる
    bool m_gearshiftdown = false;
    //空に飛んでるか
    bool airGround = false;
    //地面についてるか
    bool m_isHitGround = false;
    //前にぶつかってないか
    bool isHitStraight = false;
    // Start is called before the first frame update
    void Start()
    {
        soundMgr = FindObjectOfType<SoundManager>();
        brakeLight = transform.Find("lamborghini_centenario_v1.0/REFLECTOR_REAR_PLASTIC").gameObject;
        breakeLightIntensity = brakeLight.GetComponent<Renderer>().material;
        breakeLightIntensity.SetFloat("_EmissiveIntensity", 30000000);
        breakeLightIntensity.SetFloat("_EmissiveExposureWeight", 10.0f);
        sphereTransform = sphere.transform;
        prevSpherePos = sphereTransform.position;
        m_playPauseSmoke = false;
        maxRPM = 300;

        if (this.gameObject.CompareTag("Player1"))
        {
            isPlayer1 = this.gameObject.CompareTag("Player1");
            soundMgr.InitializeEngineSE(1);
        }
        else if(this.gameObject.CompareTag("Player2"))
        {
            isPlayer2 = this.gameObject.CompareTag("Player2");
            soundMgr.InitializeEngineSE(2);
        }
        else if(this.gameObject.CompareTag("AI"))
        {
            isAi = this.gameObject.CompareTag("Player2");
            soundMgr.InitializeEngineSE(3);
        }
    }

    //ゲッター
    //セッター
    public bool forward
    {
        set
        {
            m_forward = value;
        }
    }
    public bool back
    {
        set
        {
            m_back = value;
        }
    }
    public bool left
    {
        set
        {
            m_left = value;
        }
    }
    public bool right
    {
        set
        {
            m_right = value;
        }
    }
    public bool gearshiftup
    {
        set
        {
            m_gearshiftup = value;
        }
    }
    public bool gearshiftdown
    {
        set
        {
            m_gearshiftdown = value;
        }
    }


    public bool playPauseSmoke
    {
        get
        {
            return m_playPauseSmoke;
        }
    }

    public bool isHitGround
    {
        get
        {
            return m_isHitGround;
        }
    }
    public float currentRotate
    {
        get
        {
            return m_currentRotate;
        }
    }

    public int currentGearNum
    {
        get
        {
            return m_currentGearNum;
        }
    }


    // Update is called once per frame
    void Update()
    {
        Operation();
        if (soundMgr.startedSound)
        {
            soundMgr.StartSound();
        }
        m_forward = false;
        m_back = false;
        m_left = false;
        m_right = false;
        m_gearshiftup = false;
        m_gearshiftdown = false;
    }
    void FixedUpdate()
    {
        if (!RaceManager.instance.stopGame) return;
        //車のレイヤーを除外
        int layerMask = -1 - 1 << gameObject.layer;

        //斜面に触れているか
        isHitStraight = Physics.SphereCast(origin: transform.position, m_radius, direction: transform.TransformDirection(new Vector3(0, -0.08f, 1).normalized), out straightHit, maxStraightDistance, layerMask);

        if (!isHitStraight)
        {
            if (sphere.velocity.y >= 3.0f)
            {
                sphere.velocity = new Vector3(sphere.velocity.x, -4.0f, sphere.velocity.z);
            }
            else if (sphere.velocity.y >= 1.0f)
            {
                sphere.velocity = new Vector3(sphere.velocity.x, -1.0f, sphere.velocity.z);
            }
        }

        //宙に浮いているか
        m_isHitGround = Physics.SphereCast(origin: transform.position, m_radius, direction: -transform.up, out underHit, maxUnderDistance, layerMask);
        if (m_isHitGround)
        {
            prevSpherePos = sphereTransform.position;
            if (airGround)
            {
                maxUnderDistance = 0.45f;
                airGround = false;
            }
        }
        else
        {
            sphere.velocity = new Vector3(sphere.velocity.x, -3.0f, sphere.velocity.z);
            if (sphere.velocity.y >= -3.0f)
            {
                sphere.velocity = new Vector3(sphere.velocity.x, -5.0f, sphere.velocity.z); ;
            }
            maxUnderDistance++;
            airGround = true;
        }
        //リジッドボディーに力を加えている
        sphere.AddForce(transform.forward * currentSpeed * Time.deltaTime * 48, ForceMode.Acceleration);   
        sphere.AddForce(currentSpeed / 100 *  Vector3.down * gravity * Time.deltaTime, ForceMode.Acceleration);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + m_currentRotate, 0), Time.deltaTime * 5f);
    }

    //デバック用RayCast
    void OnDrawGizmosSelected()
    {
        isHitStraight = Physics.SphereCast(origin: transform.position, m_radius, direction: transform.TransformDirection(new Vector3(0, -0.08f, 1).normalized), out straightHit, maxStraightDistance);
        
        if (isHitStraight)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(from: transform.position, direction: transform.TransformDirection(new Vector3(0, -0.08f, 1).normalized) * straightHit.distance);
            Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(new Vector3(0, -0.08f, 1).normalized) * (straightHit.distance), m_radius);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(from: transform.position, direction: transform.TransformDirection(new Vector3(0, -0.08f, 1).normalized) * maxStraightDistance);
            Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(new Vector3(0, -0.08f, 1).normalized) * (maxStraightDistance), m_radius);
        }



        m_isHitGround = Physics.SphereCast(origin: transform.position, m_radius, direction: -transform.up, out underHit, maxUnderDistance);

        if (m_isHitGround)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(from: transform.position, direction: -transform.up * underHit.distance);
            Gizmos.DrawWireSphere(transform.position - transform.up * (underHit.distance), m_radius);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(from: transform.position, direction: -transform.up * maxUnderDistance);
            Gizmos.DrawWireSphere(transform.position - transform.up * (maxUnderDistance), m_radius);
        }
    }
    //車操作
    private void Operation()
    {
        //アクセル
        if (m_forward)
        {
            breakeLightIntensity.SetFloat("_EmissiveExposureWeight", 10.0f);
            speed = acceleration;
        }
        //バック
        else if (m_back)
        {
            breakeLightIntensity.SetFloat("_EmissiveExposureWeight", 0.0f);
            speed = Mathf.Lerp(speed, 0, 1.0f * Time.deltaTime);
        }
        //押されていない場合減速していく
        else
        {
            breakeLightIntensity.SetFloat("_EmissiveExposureWeight", 10.0f);
            speed = Mathf.Lerp(speed, 0, 0.2f * Time.deltaTime);
        }
        //右に曲げる
        if (m_right)
        {
            int dir = 1;
            m_rotatecar += 0.5f;
            float amount = Mathf.Abs(dir);
            Steer(dir, amount);
        }
        //左に曲げる
        else if (m_left)
        {
            int dir = -1;
            m_rotatecar -= 0.5f;
            float amount = Mathf.Abs(dir);
            Steer(dir, amount);
        }
        if (m_rotatecar != 0)
        {
            m_rotatecar = Mathf.Lerp(m_rotatecar, 0, 1.0f * Time.deltaTime);
        }
        if (m_rotatecar > 5 || m_rotatecar < -5)
        {
            m_playPauseSmoke = true;
            //ドリフト音声
            if (isPlayer1)
            {
                soundMgr.StartDrift(1);
            }
            else if (isPlayer2)
            {
                soundMgr.StartDrift(2);
            }
            else if (isAi)
            {
                soundMgr.StartDrift(3);
            }
        }
        else
        {
            m_playPauseSmoke = false;
            //ドリフト音声停止
            if (isPlayer1)
            {
                soundMgr.StopDrift(1);
            }
            else if (isPlayer2)
            {
                soundMgr.StopDrift(2);
            }
            else if (isAi)
            {
                soundMgr.StopDrift(3);
            }
        }
        //力を緩やかに加える
        ApplyForce();
        //ギアチェンジ
        GearShift();
    }

    //力を緩やかに加える
    private void ApplyForce()
    {
        m_rotatecar = Mathf.Clamp(m_rotatecar, -max_rotatecar, max_rotatecar);
        currentSpeed = Mathf.SmoothStep(currentSpeed, speed * gears[m_currentGearNum], Time.deltaTime* 12);
        m_currentRotate = Mathf.Lerp(m_currentRotate, rotate, Time.deltaTime* 4f);
        rotate = 0f;
        this.transform.position = sphereTransform.position - new Vector3(0, 0.6f, 0);
        KPH = sphere.velocity.magnitude* 3.6f;
    }

    //ギアチェンジ
    private void GearShift()
    {
        
        if (KPH >= gearChangeSpeed[m_currentGearNum])
        {
            if (gearChange == gearBox.automatic)
            {
                if (currentSpeed + 2.0f >= acceleration * gears[m_currentGearNum] && m_currentGearNum != 5)
                {
                    m_currentGearNum++;
                }
                else if (KPH + 5.0f < acceleration * gears[m_currentGearNum] && m_currentGearNum != 0)
                {
                    m_currentGearNum--;
                }
            }
            else
            {
                if (m_gearshiftup && m_currentGearNum != 5)
                {
                    m_currentGearNum++;
                }
                else if (m_gearshiftdown && m_currentGearNum != 0)
                {
                    m_currentGearNum--;
                }
            }
        }
        else if (m_currentGearNum != 0)
        {
            if (KPH <= gearChangeSpeed[m_currentGearNum - 1])
            {
                m_currentGearNum--;
            }
        }
    }
    //右に切るか左に切るかとどのぐらい切るかを計算
    private void Steer(int direction, float amount)
    {
        rotate = (steering * direction) * amount;
    }
}
