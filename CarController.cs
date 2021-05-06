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
        //車のすべての操作をここでやる
        operation();
        //操作の初期化
        m_forward = false;
        m_back = false;
        m_left = false;
        m_right = false;
    }

    //車のすべての操作をここでやる
    public void operation()
    {
        //左に切る
        if (m_left)
        {
            TurnLeft();
        }
        //右に切る
        else if (m_right)
        {
            TurnRight();
        }
        //ハンドルを戻す
        else
        {
            CenterHandle();
        }
        //ハンドルを切れる上限
        m_steering = Mathf.Clamp(m_steering, -m_maxCornering, m_maxCornering);
        Vector3 rot = transform.rotation.eulerAngles;
        rot.y += m_steering;
        transform.rotation = Quaternion.Euler(rot);

        //アクセル
        if (m_forward)
        {
            Accelerator();
        }
        //ブレーキ
        else if (m_back)
        {
            Brake();
        }
        //減速
        else
        {
            Decelerate();
        }
        //速度上限
        m_force = Mathf.Clamp(m_force, -m_maxForce , m_maxForce);
        //力を加える
        rigidBody.AddForce(transform.forward * m_force * Time.deltaTime, ForceMode.Acceleration);


        Vector3 targetVector = transform.forward;
        float magunitude = rigidBody.velocity.magnitude;
        targetVector.y = rigidBody.velocity.y / magunitude;
        rigidBody.velocity = Vector3.Lerp(rigidBody.velocity,
                                          targetVector * magunitude,
                                          0.5f * Time.deltaTime);
    }

    //車の座標
    public void Coordinate()
    {

    }
    //左に切る
    private void TurnLeft()
    {
        m_steering -= m_corneringSpec * Time.deltaTime;
    }
    //右に切る
    private void TurnRight()
    {
        m_steering += m_corneringSpec * Time.deltaTime;
    }
    //ハンドルを戻す
    private void CenterHandle()
    {
        m_steering = Mathf.Lerp(m_steering,0,0.1f);
    }

    //アクセル
    private void Accelerator()
    {
        m_force += m_accelSpec * Time.deltaTime;
    }
    //ブレーキ
    private void Brake()
    {
        m_force -= m_accelSpec * Time.deltaTime;
    }
    //減速
    private void Decelerate()
    {
        m_force = Mathf.Lerp(m_force, 0, 0.2f * Time.deltaTime);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Guardrail")
        {
            Debug.Log("ガードレール");
           m_force = m_force / 2;
        }
        if (collision.gameObject.tag == "Seams")
        {
            Debug.Log("つなぎ目当たった!");
            Vector3 pos = gameObject.transform.position;
            pos.y += 0.08f;
        }
    }
}
