using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;
    //サウンド管理クラス
    SoundManager soundMgr;
    //AI用カメラ
    public GameObject OneCamera;
    //PVP用プレイヤー１カメラ
    public GameObject PvpCamera1;
    //PVP用プレイヤー２カメラ
    public GameObject PvpCamera2;
    //サブキャンバス
    public GameObject SubCanvas;
    //プレイヤー２のオブジェクト
    public GameObject Player2Obj;
    //プレイヤー２のコライダー
    public GameObject Player2Collider;
    //AIのオブジェクト
    public GameObject AiObj;
    //AIのコライダー
    public GameObject AiCollider;
    //CarController２スクリプトのプレイヤー１アタッチ
    public CarController2 m_CarConPlayer1;
    //CarController２スクリプトのプレイヤー２アタッチ
    public CarController2 m_CarConPlayer2;
    //Playerのプレイヤー１アタッチ
    public Player Player1;
    //Playerのプレイヤー２アタッチ
    public Player Player2;
    //AIスクリプト
    public AIController Ai;
    //ブラーするマテリアル
    public Material blurmat;

    //パラメーターの針
    private GameObject needle1;
    private GameObject needle2;
    //ゴールまでの距離オブジェクト
    private GameObject checkPointObj;
    //ブラーするオブジェクト
    private GameObject[] Blurs;

    //仮値
    private int m_playType = 0;
    //最終的な順位
    private int rankFirstPlace;

    //暗闇の値
    private float alfa = 1;
    //ブラーの初期値
    private float blurSmoothness = 0.6f;
    //フェードの速度
    private float fadeSpeed = 0.01f;

    //リザルト
    private bool m_result = false;
    //ポーズ
    private bool m_pose = false;
    //ゲームと停止
    private bool m_stopGame = false;
    //フェードの終了
    private bool fadeEnd = false;
    //画面のブラーが始まる
    private bool smoothIn = false;
    //画面のブラーを止める
    private bool smoothStop = false;
    //タイムアップ
    private bool m_timeUp = false;

    //暗闇画像
    private Image fadeInImage;

    //時速テキスト
    private TextMeshProUGUI player1Speedtxt;
    private TextMeshProUGUI player2Speedtxt;
    //タイムリミットテキスト
    private TextMeshProUGUI player1TimeLimittxt;
    private TextMeshProUGUI player2TimeLimittxt;
    //経過時間テキスト
    private TextMeshProUGUI m_player1TotalTimetxt;
    private TextMeshProUGUI player2TotalTimetxt;

    //ゴールまでの距離テキスト
    private TextMeshProUGUI player1GoalDistancetxt;
    private TextMeshProUGUI player2GoalDistancetxt;
    //現在のギアテキスト
    private TextMeshProUGUI player1CurrentGeartxt;
    private TextMeshProUGUI player2CurrentGeartxt;
    //現在の順位
    private TextMeshProUGUI player1Ranktxt;
    private TextMeshProUGUI player2Ranktxt;

    //AIの時のリザルト用最高速度
    private float m_maxSpeed = 0;

    //時間制限用
    private float countDown = 500.0f;
    //秒
    private float m_seconds = 0.0f;
    //分
    private int m_minute = 0;
    //車のスピード
    public float vehicleSpeed;
    //チェックポイントの最大数
    private int maxCheckPoint = 0;

    //スピードメーターの針の始まり
    private float startPos = 215.71f;
    //スピードメーターの針の終わり
    private float endPos = -38.5f;
    //針のベストな場所
    private float desiredPos;
    //シェーダーのプロパティー保存
    private int SmoothnessID;

    // Start is called before the first frame update
    private void Awake()
    {
        instance = this; //instanceに自分自身を設定する
        m_playType = SelectSceneController.playType;
    }
    void Start()
    {
        SmoothnessID = Shader.PropertyToID("_Smoothness");
        soundMgr = FindObjectOfType<SoundManager>();
        
        fadeInImage = transform.Find("FadeIn").GetComponent<Image>();
        fadeInImage.color = new Color(0, 0, 0, alfa);
        
        //AIをセレクト
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
        //PvPをセレクト
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

    //ゲッター
    //セッター
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
        //ポーズ
        else if (!m_stopGame)
        {
            if (!smoothIn)
            {
                //ブラーをかけない
                FadeInSmoothness();
            }
            else
            {
                //ブラーをかける
                FadeOutSmoothness();
            }
        }
        //ゲーム
        else if (m_stopGame)
        {
            //ゲームオーバー時間
            TimeLimit();
            //経過時間
            TotalTime();
            //順位
            Ranking();
            //車のメーター反映
            updateNeedle();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                m_pose = true;
                //ポーズやゲーム終了に出すウィンドウ
                SetWindow();
                soundMgr.PlaySeByName("News-Alert04-1");
                soundMgr.StopBgm();
            }
            //AIをセレクト
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
            //PvPをセレクト
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

    //車の速度を表示
    // float 車の速度　int プレイヤー１・２どちらか
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


    //ゲームオーバー時間
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
            //ポーズやゲーム終了に出すウィンドウ
            SetWindow();
            soundMgr.ChangeBgm();
            soundMgr.PlayBgmByName("Winning_Road_2");
        }
    }

    //経過時間
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

    //車のメーター反映
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

    //順位
    public void Ranking()
    {
        if (m_playType == 0)
        {
            if (Player1.currentCheckPos == Ai.currentCheckPos && Ai.currentCheckPos == Player1.currentCheckPos)
            {
                player1Ranktxt.text = "同率";
            }
            else if (Player1.currentCheckPos >= Ai.currentCheckPos)
            {
                player1Ranktxt.text = 1.ToString() + "位";
            }
            else
            {
                player1Ranktxt.text = 2.ToString() + "位";
            }
        }
        else if (m_playType == 1)
        {
            if (Player1.currentCheckPos == Player2.currentCheckPos && Player2.currentCheckPos == Player1.currentCheckPos)
            {
                player1Ranktxt.text = "同率";
                player2Ranktxt.text = "同率";
            }
            else if (Player1.currentCheckPos >= Player2.currentCheckPos)
            {
                player1Ranktxt.text = 1.ToString() + "位";
                player2Ranktxt.text = 2.ToString() + "位";
            }
            else
            {
                player1Ranktxt.text = 2.ToString() + "位";
                player2Ranktxt.text = 1.ToString() + "位";
            }
        }
    }

    //ポーズやゲーム終了に出すウィンドウ
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
    /// ゴールまでの距離
    /// </summary>
    /// <param name="carPos">車の座標</param>
    /// <param name="arr">チェックポイントがどこまできているか</param>
    /// <param name="checkPointPos">チェックポイントから距離を出す</param>
    /// <param name="player">int プレイヤー１・２どちらか</param>
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

    //現在のギア
    //int　現在のギア数　 int プレイヤー１・２どちらか
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
    //ポーズ画面を閉じる
    public void ClosePose()
    {
        SubCanvas.GetComponent<Canvas>().enabled = false;
        this.GetComponent<Canvas>().enabled = true;
        m_pose = false;
        smoothIn = false;
        //m_stopGame = true;
    }
    //ブラーをかけない
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
    //ブラーをかける
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
    //ゴール
    private void goal()
    {
        m_result = true;
        //ポーズやゲーム終了に出すウィンドウ
        SetWindow();
        soundMgr.ChangeBgm();
        soundMgr.PlayBgmByName("Winning_Road_2");
    }
}
