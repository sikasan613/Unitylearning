using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController2 : MonoBehaviour
{
    //�}�j���A�����I�[�g�}�ɂł���
    internal enum gearBox
    {
        automatic,
        manual
    }
    [SerializeField]
    private gearBox gearChange;

    //�T�E���h�Ǘ��N���X
    SoundManager soundMgr;
    //���[�X�Ǘ��N���X
    RaceManager RaceMgr;
    //�Ԃɂ��郊�W�b�h�{�f�B�[
    public Rigidbody sphere;
    //�����o����
    private bool m_playPauseSmoke = false;
    //�Ԃ̑��x
    [HideInInspector]
    public float KPH;
    //�Ԃ̍ō����x
    [HideInInspector]
    public float maxRPM;
    //�Ԃ̃��[�e�[�V�����̍ő�l�ƍŏ��l
    private float max_rotatecar = 30f;
    //�Ԃ̑��x�̍ő�l�ƍŏ��l
    private float acceleration = 30f;
    //�Ԃ̋Ȃ����ő�l�ƍŏ��l
    private float steering = 5f;
    //���ɗ͂�������
    private float gravity = 10f;
    //�X�t�B�A���C�L���X�g�̔��a
    private float m_radius = 0.2f;
    //���C�L���X�g�̍ő勗��
    private float maxUnderDistance = 0.45f;
    private float maxStraightDistance = 8.0f;
    //�Ԃɂ��Ă�X�t�B�A�̍��W
    private Transform sphereTransform;
    //�������鑬�x
    private float speed;
    //���݂̑��x
    private float currentSpeed;
    //���������]
    private float rotate;
    //���݂̉�]
    private float m_currentRotate;
    //�h���t�g�p�̉�]
    private float m_rotatecar;
    //�M�A��
    private float[] gears = { 1f, 2.5f, 2.8f, 3.0f, 3.5f, 3.7f };

    private float[] gearChangeSpeed = { 40f, 80f, 120f, 140f, 160f,200f};
    //���݂̃M�A��
    private int m_currentGearNum = 0;
    Vector3 prevSpherePos;

    //�I�u�W�F�N�g�����邩�`�F�b�N
    private bool isPlayer1;
    private bool isPlayer2;
    private bool isAi;
    //�o�b�N���C�g�̃I�u�W�F�N�g
    GameObject brakeLight;
    //�o�b�N���C�g�̔��s�Ɏg�p
    Material breakeLightIntensity;
    //�����Ƀ��C�L���X�g���o��
    RaycastHit underHit;
    RaycastHit straightHit;
    //�A�N�Z��
    bool m_forward = false;
    //�o�b�N
    bool m_back = false;
    //���ɋȂ���
    bool m_left = false;
    //�E�ɋȂ���
    bool m_right = false;
    //�M�A��������
    bool m_gearshiftup = false;
    //�M�A��������
    bool m_gearshiftdown = false;
    //��ɔ��ł邩
    bool airGround = false;
    //�n�ʂɂ��Ă邩
    bool m_isHitGround = false;
    //�O�ɂԂ����ĂȂ���
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

    //�Q�b�^�[
    //�Z�b�^�[
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
        //�Ԃ̃��C���[�����O
        int layerMask = -1 - 1 << gameObject.layer;

        //�ΖʂɐG��Ă��邩
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

        //���ɕ����Ă��邩
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
        //���W�b�h�{�f�B�[�ɗ͂������Ă���
        sphere.AddForce(transform.forward * currentSpeed * Time.deltaTime * 48, ForceMode.Acceleration);   
        sphere.AddForce(currentSpeed / 100 *  Vector3.down * gravity * Time.deltaTime, ForceMode.Acceleration);
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, transform.eulerAngles.y + m_currentRotate, 0), Time.deltaTime * 5f);
    }

    //�f�o�b�N�pRayCast
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
    //�ԑ���
    private void Operation()
    {
        //�A�N�Z��
        if (m_forward)
        {
            breakeLightIntensity.SetFloat("_EmissiveExposureWeight", 10.0f);
            speed = acceleration;
        }
        //�o�b�N
        else if (m_back)
        {
            breakeLightIntensity.SetFloat("_EmissiveExposureWeight", 0.0f);
            speed = Mathf.Lerp(speed, 0, 1.0f * Time.deltaTime);
        }
        //������Ă��Ȃ��ꍇ�������Ă���
        else
        {
            breakeLightIntensity.SetFloat("_EmissiveExposureWeight", 10.0f);
            speed = Mathf.Lerp(speed, 0, 0.2f * Time.deltaTime);
        }
        //�E�ɋȂ���
        if (m_right)
        {
            int dir = 1;
            m_rotatecar += 0.5f;
            float amount = Mathf.Abs(dir);
            Steer(dir, amount);
        }
        //���ɋȂ���
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
            //�h���t�g����
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
            //�h���t�g������~
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
        //�͂��ɂ₩�ɉ�����
        ApplyForce();
        //�M�A�`�F���W
        GearShift();
    }

    //�͂��ɂ₩�ɉ�����
    private void ApplyForce()
    {
        m_rotatecar = Mathf.Clamp(m_rotatecar, -max_rotatecar, max_rotatecar);
        currentSpeed = Mathf.SmoothStep(currentSpeed, speed * gears[m_currentGearNum], Time.deltaTime* 12);
        m_currentRotate = Mathf.Lerp(m_currentRotate, rotate, Time.deltaTime* 4f);
        rotate = 0f;
        this.transform.position = sphereTransform.position - new Vector3(0, 0.6f, 0);
        KPH = sphere.velocity.magnitude* 3.6f;
    }

    //�M�A�`�F���W
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
    //�E�ɐ؂邩���ɐ؂邩�Ƃǂ̂��炢�؂邩���v�Z
    private void Steer(int direction, float amount)
    {
        rotate = (steering * direction) * amount;
    }
}
