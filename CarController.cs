using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField]
    float m_accelSpec = 5f;
    [SerializeField]
    float m_maxForce = 100f;
    float m_force = 0;
    [SerializeField]
    float m_corneringSpec = 5f;
    [SerializeField]
    float m_maxCornering = 50f;
    float m_steering = 0;
    float tilt = 0;

    Rigidbody rigidBody;
    bool m_forward = false;
    bool m_back = false;
    bool m_left = false;
    bool m_right = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }
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

    // Update is called once per frame
    void Update()
    {
        //�Ԃ̂��ׂĂ̑���������ł��
        operation();
        //����̏�����
        m_forward = false;
        m_back = false;
        m_left = false;
        m_right = false;
    }

    //�Ԃ̂��ׂĂ̑���������ł��
    public void operation()
    {
        //���ɐ؂�
        if (m_left)
        {
            TurnLeft();
        }
        //�E�ɐ؂�
        else if (m_right)
        {
            TurnRight();
        }
        //�n���h����߂�
        else
        {
            CenterHandle();
        }
        //�n���h����؂����
        m_steering = Mathf.Clamp(m_steering, -m_maxCornering, m_maxCornering);
        Vector3 rot = transform.rotation.eulerAngles;
        rot.y += m_steering;
        transform.rotation = Quaternion.Euler(rot);

        //�A�N�Z��
        if (m_forward)
        {
            Accelerator();
        }
        //�u���[�L
        else if (m_back)
        {
            Brake();
        }
        //����
        else
        {
            Decelerate();
        }
        //���x���
        m_force = Mathf.Clamp(m_force, -m_maxForce , m_maxForce);
        //�͂�������
        rigidBody.AddForce(transform.forward * m_force * Time.deltaTime, ForceMode.Acceleration);


        Vector3 targetVector = transform.forward;
        float magunitude = rigidBody.velocity.magnitude;
        targetVector.y = rigidBody.velocity.y / magunitude;
        rigidBody.velocity = Vector3.Lerp(rigidBody.velocity,
                                          targetVector * magunitude,
                                          0.5f * Time.deltaTime);
    }

    //�Ԃ̍��W
    public void Coordinate()
    {

    }
    //���ɐ؂�
    private void TurnLeft()
    {
        m_steering -= m_corneringSpec * Time.deltaTime;
    }
    //�E�ɐ؂�
    private void TurnRight()
    {
        m_steering += m_corneringSpec * Time.deltaTime;
    }
    //�n���h����߂�
    private void CenterHandle()
    {
        m_steering = Mathf.Lerp(m_steering,0,0.1f);
    }

    //�A�N�Z��
    private void Accelerator()
    {
        m_force += m_accelSpec * Time.deltaTime;
    }
    //�u���[�L
    private void Brake()
    {
        m_force -= m_accelSpec * Time.deltaTime;
    }
    //����
    private void Decelerate()
    {
        m_force = Mathf.Lerp(m_force, 0, 0.2f * Time.deltaTime);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Guardrail")
        {
            Debug.Log("�K�[�h���[��");
           m_force = m_force / 2;
        }
        if (collision.gameObject.tag == "Seams")
        {
            Debug.Log("�Ȃ��ړ�������!");
            Vector3 pos = gameObject.transform.position;
            pos.y += 0.08f;
        }
    }
}
