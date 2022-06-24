using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;
    //�T�E���h�Ǘ��N���X
    SoundManager soundMgr;
    //AI�p�J����
    public GameObject OneCamera;
    //PVP�p�v���C���[�P�J����
    public GameObject PvpCamera1;
    //PVP�p�v���C���[�Q�J����
    public GameObject PvpCamera2;
    //�T�u�L�����o�X
    public GameObject SubCanvas;
    //�v���C���[�Q�̃I�u�W�F�N�g
    public GameObject Player2Obj;
    //�v���C���[�Q�̃R���C�_�[
    public GameObject Player2Collider;
    //AI�̃I�u�W�F�N�g
    public GameObject AiObj;
    //AI�̃R���C�_�[
    public GameObject AiCollider;
    //CarController�Q�X�N���v�g�̃v���C���[�P�A�^�b�`
    public CarController2 m_CarConPlayer1;
    //CarController�Q�X�N���v�g�̃v���C���[�Q�A�^�b�`
    public CarController2 m_CarConPlayer2;
    //Player�̃v���C���[�P�A�^�b�`
    public Player Player1;
    //Player�̃v���C���[�Q�A�^�b�`
    public Player Player2;
    //AI�X�N���v�g
    public AIController Ai;
    //�u���[����}�e���A��
    public Material blurmat;

    //�p�����[�^�[�̐j
    private GameObject needle1;
    private GameObject needle2;
    //�S�[���܂ł̋����I�u�W�F�N�g
    private GameObject checkPointObj;
    //�u���[����I�u�W�F�N�g
    private GameObject[] Blurs;

    //���l
    private int m_playType = 0;
    //�ŏI�I�ȏ���
    private int rankFirstPlace;

    //�Èł̒l
    private float alfa = 1;
    //�u���[�̏����l
    private float blurSmoothness = 0.6f;
    //�t�F�[�h�̑��x
    private float fadeSpeed = 0.01f;

    //���U���g
    private bool m_result = false;
    //�|�[�Y
    private bool m_pose = false;
    //�Q�[���ƒ�~
    private bool m_stopGame = false;
    //�t�F�[�h�̏I��
    private bool fadeEnd = false;
    //��ʂ̃u���[���n�܂�
    private bool smoothIn = false;
    //��ʂ̃u���[���~�߂�
    private bool smoothStop = false;
    //�^�C���A�b�v
    private bool m_timeUp = false;

    //�Èŉ摜
    private Image fadeInImage;

    //�����e�L�X�g
    private TextMeshProUGUI player1Speedtxt;
    private TextMeshProUGUI player2Speedtxt;
    //�^�C�����~�b�g�e�L�X�g
    private TextMeshProUGUI player1TimeLimittxt;
    private TextMeshProUGUI player2TimeLimittxt;
    //�o�ߎ��ԃe�L�X�g
    private TextMeshProUGUI m_player1TotalTimetxt;
    private TextMeshProUGUI player2TotalTimetxt;

    //�S�[���܂ł̋����e�L�X�g
    private TextMeshProUGUI player1GoalDistancetxt;
    private TextMeshProUGUI player2GoalDistancetxt;
    //���݂̃M�A�e�L�X�g
    private TextMeshProUGUI player1CurrentGeartxt;
    private TextMeshProUGUI player2CurrentGeartxt;
    //���݂̏���
    private TextMeshProUGUI player1Ranktxt;
    private TextMeshProUGUI player2Ranktxt;

    //AI�̎��̃��U���g�p�ō����x
    private float m_maxSpeed = 0;

    //���Ԑ����p
    private float countDown = 500.0f;
    //�b
    private float m_seconds = 0.0f;
    //��
    private int m_minute = 0;
    //�Ԃ̃X�s�[�h
    public float vehicleSpeed;
    //�`�F�b�N�|�C���g�̍ő吔
    private int maxCheckPoint = 0;

    //�X�s�[�h���[�^�[�̐j�̎n�܂�
    private float startPos = 215.71f;
    //�X�s�[�h���[�^�[�̐j�̏I���
    private float endPos = -38.5f;
    //�j�̃x�X�g�ȏꏊ
    private float desiredPos;
    //�V�F�[�_�[�̃v���p�e�B�[�ۑ�
    private int SmoothnessID;

    // Start is called before the first frame update
    private void Awake()
    {
        instance = this; //instance�Ɏ������g��ݒ肷��
        m_playType = SelectSceneController.playType;
    }
    void Start()
    {
        SmoothnessID = Shader.PropertyToID("_Smoothness");
        soundMgr = FindObjectOfType<SoundManager>();
        
        fadeInImage = transform.Find("FadeIn").GetComponent<Image>();
        fadeInImage.color = new Color(0, 0, 0, alfa);
        
        //AI���Z���N�g
        if (m_playType == 0)
        {
            PvpCamera1.SetActive(false);
            PvpCamera2.SetActive(false);
            Player2Obj.SetActive(false);
            Player2Collider.SetActive(false);
            transform.Find("SelectPVP").gameObject.SetActive(false);
            GameObject SelectAi = transform.Find("SelectAI").gameObject;
            player1Speedtxt = SelectAi.transform.Find("Speed").GetComponent<TextMeshProUGUI>();
            player1TimeLimittxt = SelectAi.transform.Find("Timelimit").GetComponent<TextMeshProUGUI>();
            m_player1TotalTimetxt = SelectAi.transform.Find("Totaltime").GetComponent<TextMeshProUGUI>();
            player1GoalDistancetxt = SelectAi.transform.Find("Goaldistance").GetComponent<TextMeshProUGUI>();
            player1CurrentGeartxt = SelectAi.transform.Find("Currentgear").GetComponent<TextMeshProUGUI>();
            player1Ranktxt = SelectAi.transform.Find("Rank").GetComponent<TextMeshProUGUI>();
            needle1 = SelectAi.transform.Find("speedmeter/needele").gameObject;
        }
        //PvP���Z���N�g
        else
        {
            OneCamera.SetActive(false);
            AiObj.SetActive(false);
            AiCollider.SetActive(false);
            transform.Find("SelectAI").gameObject.SetActive(false);
            GameObject Player1 = transform.Find("SelectPVP/Player1").gameObject;
            player1Speedtxt = Player1.transform.Find("Speed").GetComponent<TextMeshProUGUI>();
            player1TimeLimittxt = Player1.transform.Find("Timelimit").GetComponent<TextMeshProUGUI>();
            m_player1TotalTimetxt = Player1.transform.Find("Totaltime").GetComponent<TextMeshProUGUI>();
            player1GoalDistancetxt = Player1.transform.Find("Goaldistance").GetComponent<TextMeshProUGUI>();
            player1CurrentGeartxt = Player1.transform.Find("Currentgear").GetComponent<TextMeshProUGUI>();
            player1Ranktxt = Player1.transform.Find("Rank").GetComponent<TextMeshProUGUI>();
            needle1 = Player1.transform.Find("speedmeter/needele").gameObject;


            GameObject Player2 = transform.Find("SelectPVP/Player2").gameObject;
            player2Speedtxt = Player2.transform.Find("Speed").GetComponent<TextMeshProUGUI>();
            player2TimeLimittxt = Player2.transform.Find("Timelimit").GetComponent<TextMeshProUGUI>();
            player2TotalTimetxt = Player2.transform.Find("Totaltime").GetComponent<TextMeshProUGUI>();
            player2GoalDistancetxt = Player2.transform.Find("Goaldistance").GetComponent<TextMeshProUGUI>();
            player2CurrentGeartxt = Player2.transform.Find("Currentgear").GetComponent<TextMeshProUGUI>();
            player2Ranktxt = Player2.transform.Find("Rank").GetComponent<TextMeshProUGUI>();
            needle2 = Player2.transform.Find("speedmeter/needele").gameObject;
        }
        countDown = 500.0f;
        m_seconds = 0.0f;
        m_minute = 0;
        soundMgr.ChangeBgm();
        soundMgr.PlayBgmByName("Sagittarius_2");
        Blurs = GameObject.FindGameObjectsWithTag("Blur");
        blurmat.SetFloat(SmoothnessID, blurSmoothness);
    }

    //�Q�b�^�[
    //�Z�b�^�[
    public int resultRanking
    {
        get
        {
            return rankFirstPlace;
        }
    }
    public int playType
    {
        get
        {
            return m_playType;
        }
    }
    
    public float maxSpeed
    {
        get
        {
            return m_maxSpeed;
        }
    }
    public float minute
    {
        get
        {
            return m_minute;
        }
    }
    public float seconds
    {
        get
        {
            return m_seconds;
        }
    }

    public bool result
    {
        get
        {
            return m_result;
        }
    }
    public bool pose
    {
        get
        {
            return m_pose;
        }
    }
    public bool stopGame
    {
        get
        {
            return m_stopGame;
        }
    }
    public bool timeUp
    {
        get
        {
            return m_timeUp;
        }
    }

    public TextMeshProUGUI player1TotalTimetxt
    {
        get
        {
            return m_player1TotalTimetxt;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_stopGame && !fadeEnd)
        {
            fadeInImage.color = new Color(0, 0, 0, alfa);
            alfa -= fadeSpeed;
            if (alfa < 0)
            {
                maxCheckPoint = Player1.maxCheckPoint;
                fadeEnd = true;
            }
        }
        //�|�[�Y
        else if (!m_stopGame)
        {
            if (!smoothIn)
            {
                //�u���[�������Ȃ�
                FadeInSmoothness();
            }
            else
            {
                //�u���[��������
                FadeOutSmoothness();
            }
        }
        //�Q�[��
        else if (m_stopGame)
        {
            //�Q�[���I�[�o�[����
            TimeLimit();
            //�o�ߎ���
            TotalTime();
            //����
            Ranking();
            //�Ԃ̃��[�^�[���f
            updateNeedle();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                m_pose = true;
                //�|�[�Y��Q�[���I���ɏo���E�B���h�E
                SetWindow();
                soundMgr.PlaySeByName("News-Alert04-1");
                soundMgr.StopBgm();
            }
            //AI���Z���N�g
            if (m_playType == 0)
            {
                if (Player1.currentCheckPos == maxCheckPoint)
                {
                    goal();
                }
                else if (Ai.currentCheckPos == maxCheckPoint -1)
                {
                    goal();
                }
            }
            //PvP���Z���N�g
            else if (m_playType == 1)
            {
                if (Player1.currentCheckPos == maxCheckPoint)
                {
                    goal();
                }
                else if (Player2.currentCheckPos == maxCheckPoint)
                {
                    goal();
                }
            }
        }
    }

    //�Ԃ̑��x��\��
    // float �Ԃ̑��x�@int �v���C���[�P�E�Q�ǂ��炩
    public void SpeedDisplay(float speed, int player)
    {
        if (player == 1)
        {
            player1Speedtxt.text = speed.ToString("F0");
            if (speed >= m_maxSpeed)
            {
                m_maxSpeed = speed;
            }
        }
        else if (player == 2)
        {
            player2Speedtxt.text = speed.ToString("F0");
        }
    }


    //�Q�[���I�[�o�[����
    private void TimeLimit()
    {
        if (countDown >= 0)
        {
            countDown -= Time.deltaTime;
            player1TimeLimittxt.text = countDown.ToString("F0");
            if (player2TimeLimittxt == null) return;
            player2TimeLimittxt.text = countDown.ToString("F0");
        }
        else
        {
            m_timeUp = true;
            m_result = true;
            //�|�[�Y��Q�[���I���ɏo���E�B���h�E
            SetWindow();
            soundMgr.ChangeBgm();
            soundMgr.PlayBgmByName("Winning_Road_2");
        }
    }

    //�o�ߎ���
    private void TotalTime()
    {
        m_seconds += Time.deltaTime;
        if (m_minute == 0)
        {
            if (m_seconds <= 10)
            {
                m_player1TotalTimetxt.text = "0.0" + m_seconds.ToString("F3");
                if (player2TimeLimittxt == null) return;
                player2TotalTimetxt.text = "0.0" + m_seconds.ToString("F3");
            }
            else if (m_seconds <= 60)
            {
                m_player1TotalTimetxt.text = "0." + m_seconds.ToString("F3");
                if (player2TimeLimittxt == null) return;
                player2TotalTimetxt.text = "0.0" + m_seconds.ToString("F3");
            }
            else
            {
                m_minute++;
                m_seconds = 0;
            }
        }
        else
        {
            if (m_seconds <= 10)
            {
                m_player1TotalTimetxt.text = m_minute.ToString("0") + ".0" + m_seconds.ToString("F3");
                if (player2TimeLimittxt == null) return;
                player2TotalTimetxt.text = m_minute.ToString("0") + ".0" + m_seconds.ToString("F3");
            }
            else if (m_seconds <= 60)
            {
                m_player1TotalTimetxt.text = m_minute.ToString("0") + "." + m_seconds.ToString("F3");
                if (player2TimeLimittxt == null) return;
                player2TotalTimetxt.text = m_minute.ToString("0") + ".0" + m_seconds.ToString("F3");
            }
            else
            {
                m_minute++;
                m_seconds = 0;
            }
        }
    }

    //�Ԃ̃��[�^�[���f
    public void updateNeedle()
    {
        vehicleSpeed = m_CarConPlayer1.KPH;
        desiredPos = startPos - endPos;
        float temp = vehicleSpeed / 180;
        needle1.transform.eulerAngles = new Vector3(0, 0, (startPos - temp * desiredPos));
        if (m_playType == 0) return;
        vehicleSpeed = m_CarConPlayer2.KPH;
        temp = vehicleSpeed / 180;
        needle2.transform.eulerAngles = new Vector3(0, 0, (startPos - temp * desiredPos));
    }

    //����
    public void Ranking()
    {
        if (m_playType == 0)
        {
            if (Player1.currentCheckPos == Ai.currentCheckPos && Ai.currentCheckPos == Player1.currentCheckPos)
            {
                player1Ranktxt.text = "����";
            }
            else if (Player1.currentCheckPos >= Ai.currentCheckPos)
            {
                player1Ranktxt.text = 1.ToString() + "��";
            }
            else
            {
                player1Ranktxt.text = 2.ToString() + "��";
            }
        }
        else if (m_playType == 1)
        {
            if (Player1.currentCheckPos == Player2.currentCheckPos && Player2.currentCheckPos == Player1.currentCheckPos)
            {
                player1Ranktxt.text = "����";
                player2Ranktxt.text = "����";
            }
            else if (Player1.currentCheckPos >= Player2.currentCheckPos)
            {
                player1Ranktxt.text = 1.ToString() + "��";
                player2Ranktxt.text = 2.ToString() + "��";
            }
            else
            {
                player1Ranktxt.text = 2.ToString() + "��";
                player2Ranktxt.text = 1.ToString() + "��";
            }
        }
    }

    //�|�[�Y��Q�[���I���ɏo���E�B���h�E
    public void SetWindow()
    {
        SubCanvas.GetComponent<Canvas>().enabled = true;
        this.GetComponent<Canvas>().enabled = false;
        if (m_result)
        {
            if (m_timeUp)
            {
                rankFirstPlace = 3;
            }
            else
            {
                if (m_playType == 0)
                {
                    //Draw
                    if (Player1.currentCheckPos == Ai.currentCheckPos && Ai.currentCheckPos == Player1.currentCheckPos)
                    {
                        rankFirstPlace = 3;
                    }
                    else if (Player1.currentCheckPos >= Ai.currentCheckPos)
                    {
                        rankFirstPlace = 1;
                    }
                    else
                    {
                        rankFirstPlace = 2;
                    }
                    Debug.Log("rankFirstPlace" + rankFirstPlace);
                }
                else
                {
                    if (Player1.currentCheckPos == Player2.currentCheckPos && Player2.currentCheckPos == Player1.currentCheckPos)
                    {
                        rankFirstPlace = 3;
                    }
                    else if (Player1.currentCheckPos >= Player2.currentCheckPos)
                    {
                        rankFirstPlace = 1;
                    }
                    else
                    {
                        rankFirstPlace = 2;
                    }
                }
            }
        }
        m_stopGame = false;
        for (int i = 0; i < Blurs.Length; i++)
        {
            Blurs[i].SetActive(true);
        }
    }

    /// <summary>
    /// �S�[���܂ł̋���
    /// </summary>
    /// <param name="carPos">�Ԃ̍��W</param>
    /// <param name="arr">�`�F�b�N�|�C���g���ǂ��܂ł��Ă��邩</param>
    /// <param name="checkPointPos">�`�F�b�N�|�C���g���狗�����o��</param>
    /// <param name="player">int �v���C���[�P�E�Q�ǂ��炩</param>
    public void GoalDistance(Vector3 carPos, bool[] arr, List<Transform> checkPointPos, int player)
    {
        float _distance = 0;
        bool fromCar = false;
        for (int i = 0; i < checkPointPos.Count; i++)
        {
            if (arr[i] == false)
            {
                if (!fromCar)
                {
                    _distance += Vector3.Distance(carPos, checkPointPos[i].position);
                    if (i != checkPointPos.Count - 1)
                    {
                        _distance += Vector3.Distance(checkPointPos[i].position, checkPointPos[i + 1].position);
                    }
                    fromCar = true;
                }
                else if (i != checkPointPos.Count - 1)
                {
                    _distance += Vector3.Distance(checkPointPos[i].position, checkPointPos[i + 1].position);
                }
            }
        }
        if (player == 1)
        {
            player1GoalDistancetxt.text = "dis" + _distance.ToString("F3");
        }
        if (player == 2)
        {
            player2GoalDistancetxt.text = "dis" + _distance.ToString("F3");
        }
    }

    //���݂̃M�A
    //int�@���݂̃M�A���@ int �v���C���[�P�E�Q�ǂ��炩
    public void DisplayGear(int gear, int player)
    {
        gear++;
        if (player == 1)
        {
            player1CurrentGeartxt.text = gear.ToString();
        }
        else if (player == 2)
        {
            player2CurrentGeartxt.text = gear.ToString();
        }
    }
    //�|�[�Y��ʂ����
    public void ClosePose()
    {
        SubCanvas.GetComponent<Canvas>().enabled = false;
        this.GetComponent<Canvas>().enabled = true;
        m_pose = false;
        smoothIn = false;
        //m_stopGame = true;
    }
    //�u���[�������Ȃ�
    public void FadeInSmoothness()
    {
        if (blurSmoothness <= 1)
        {
            blurmat.SetFloat("_Smoothness", blurSmoothness);
            blurSmoothness += fadeSpeed;
        }
        else
        {
            for (int i = 0; i < Blurs.Length; i++)
            {
                Blurs[i].SetActive(false);
            }
            smoothIn = true;
            smoothStop = false;
            m_stopGame = true;
        }
    }
    //�u���[��������
    public void FadeOutSmoothness()
    {
        if (!smoothStop)
        {
            if (blurSmoothness >= 0.6f)
            {
                blurmat.SetFloat("_Smoothness", blurSmoothness);
                blurSmoothness -= fadeSpeed;
            }
            else
            {
                smoothStop = true;
            }
        }
    }
    //�S�[��
    private void goal()
    {
        m_result = true;
        //�|�[�Y��Q�[���I���ɏo���E�B���h�E
        SetWindow();
        soundMgr.ChangeBgm();
        soundMgr.PlayBgmByName("Winning_Road_2");
    }
}
