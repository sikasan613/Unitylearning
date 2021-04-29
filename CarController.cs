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

        m_forward = false;
        m_back = false;
        m_left = false;
        m_right = false;
    }

    //�Ԃ̂��ׂĂ̑���������ł��
    public void operation()
    {
        //�A�N�Z��
        if (m_forward)
        {
            Debug.Log(Time.deltaTime);
            accelerator();
        }
        //�u���[�L
        else if (m_back)
        {
            brake();
        }
        //����
        else
        {
            Decelerate();
        }
        m_force = Mathf.Clamp(m_force, -m_maxForce , m_maxForce);
        rigidBody.AddForce(transform.forward * m_force * Time.deltaTime, ForceMode.Acceleration);
    }


    //�Ԃ̍��W
    public void coordinate()
    {

    }

    //�A�N�Z��
    private void accelerator()
    {
        m_force += m_accelSpec * Time.deltaTime;
    }
    //�u���[�L
    private void brake()
    {
        m_force -= m_accelSpec * Time.deltaTime;
    }
    //����
    private void Decelerate()
    {
        m_force = Mathf.Lerp(m_force, 0, 0.2f * Time.deltaTime);
    }
}
