using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ResultEffect : MonoBehaviour
{
    //サウンド管理クラス
    SoundManager soundMgr;
    private float fadeIn = -1f;
    private float fadeOut = -0.02f;
    //1位のフェードイン
    private float firstPlaceFadeIn = -1f;
    //２位のフェードイン
    private float secondPlaceFadeIn = -1f;
    //アニメーション
    private float animCount = 0.0f;
    //実行中のアニメーション
    private int selectAnimation = 1;
    //アニメーションのタイミング
    private float animationTrigger = 0.3f;
    //自己ベストの記録
    [SerializeField, PersistentAmongPlayMode]
    private float[] m_personalBest = new float[2];
    private bool finishFedeOut = false;
    //一度文字を大きくした後戻してくるように
    private bool comeBack = false;
    //一度だけ読む
    private bool m_calledOnce = false;
    //リザルト画面をひらく
    private bool panelOpen = false;
    //セレクト終了
    private bool selectEnd = false;
    //１位のフェードイン終了
    private bool firstPlaceEnd = false;
    //２位のフェードイン終了
    private bool personalBestEnd = false;

    private Animator[] animator = new Animator[3];
    private GameObject ResultCanvas;
    private TextMeshProUGUI resultTitle;
    private RectTransform personalBest;
    //リザルトのフォントマテリアル
    private Material material;
    //フィニッシュ用のマテリアル
    private Material finishTitle;

    //WinかLoseを表示
    private TextMeshProUGUI SelectAiPlayer;
    private TextMeshProUGUI SelectPvpPlayer1;
    private TextMeshProUGUI SelectPvpPlayer2;
    //黄色
    private Color winColor = new Color(191, 191,0,255) * 2.416924f;
    //青色
    private Color loseColor = new Color(0, 38, 191, 255) * 3.416924f;
    //白色
    private Color drawColor = new Color(255, 255, 255, 255) * 3.250774f;

    // Start is called before the first frame update
    void Start()
    {
        soundMgr = FindObjectOfType<SoundManager>();
        animator[0] = transform.Find("Panel/PersonalBest").GetComponent<Animator>();
        animator[1] = transform.Find("Panel/TotalTiime").GetComponent<Animator>();
        animator[2] = transform.Find("Panel/MaxSpeed").GetComponent<Animator>();
        for (int i = 0; i < animator.Length; i++)
        {
            animator[i].enabled = false;
        }

        resultTitle = transform.Find("Panel/ResultTitle").GetComponent<TextMeshProUGUI>();
        personalBest = transform.Find("Panel/ResultTitle").GetComponent<RectTransform>();
        material = resultTitle.fontMaterial;
        finishTitle = transform.Find("Finish").GetComponent<TextMeshProUGUI>().fontMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (RaceManager.instance.result)
        {
            if (RaceManager.instance.playType == 0)
            {
                //Aiをセレクト
                SelectAi();
            }
            else
            {
                //PvPをセレクト
                SelectPvp();
            }
            //リザルト画面をひらく
            if (panelOpen)
            {
                //リザルトウィンドウ
                PanelWindow();
            }
        }
        if (RaceManager.instance.pose)
        {
            this.transform.Find("PoseorGameEnd").gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                soundMgr.PlaySeByName("News-Alert03-1");
                this.transform.Find("PoseorGameEnd").gameObject.GetComponent<Animator>().SetBool("closeWindow", true);
                StartCoroutine(Delay(0.5f, () =>
                {
                    this.transform.Find("PoseorGameEnd").gameObject.GetComponent<Animator>().SetBool("closeWindow", false);
                    this.transform.Find("PoseorGameEnd").gameObject.SetActive(false);
                    RaceManager.instance.ClosePose();
                    soundMgr.ReStartBgm();
                }));
            }
        }
    }
    //Aiをセレクト
    private void SelectAi()
    {
        if (!selectEnd)
        {
            if (!m_calledOnce)
            {
                this.transform.Find("Finish").gameObject.SetActive(true);
                GameObject SelectAi = this.transform.Find("SelectAI").gameObject;
                SelectAiPlayer = SelectAi.transform.Find("Player1").GetComponent<TextMeshProUGUI>();
                SelectAi.SetActive(true);
                m_calledOnce = true;
            }
            if (!finishFedeOut)
            {
                FinishFade();
            }
            else if (finishFedeOut)
            {
                //AIセレクト１位
                if (RaceManager.instance.resultRanking == 1)
                {
                    SelectAiPlayer.color = winColor;
                    RankFade(SelectAiPlayer, null);
                    SelectAiPlayer.text = "Win";
                }
                //AIセレクト２位
                else if (RaceManager.instance.resultRanking == 2)
                {
                    SelectAiPlayer.color = loseColor;
                    RankFade(SelectAiPlayer, null);
                    SelectAiPlayer.text = "Lose";
                }
                //AIセレクト同率
                else
                {
                    SelectAiPlayer.color = drawColor;
                    SelectAiPlayer.fontMaterial.SetFloat("_FaceDilate", 0);
                    SelectAiPlayer.text = "Draw";
                    StartCoroutine(Delay(1.0f, () =>
                    {
                        selectEnd = true;
                        GameObject panel = this.transform.Find("Panel").gameObject;
                        panel.SetActive(true);
                        if (!panelOpen)
                        {
                            if (!RaceManager.instance.timeUp)
                            {
                                panel.transform.Find("TotalTiime").GetComponent<TextMeshProUGUI>().text += RaceManager.instance.player1TotalTimetxt.text; //　TotalTiime
                                panel.transform.Find("MaxSpeed").GetComponent<TextMeshProUGUI>().text += RaceManager.instance.maxSpeed.ToString("F0");//MaxSpeed
                            }
                            else
                            {
                                panel.transform.Find("TotalTiime").GetComponent<TextMeshProUGUI>().text += "NoRecord"; //　TotalTiime
                                panel.transform.Find("MaxSpeed").GetComponent<TextMeshProUGUI>().text += "NoRecord";//MaxSpeed
                            }
                        }
                        BestTime(panel);
                        panelOpen = true;
                    }));
                }
            }
        }
        else
        {

            StartCoroutine(Delay(1.0f, () =>
            {
                this.transform.Find("Panel").gameObject.GetComponent<Animator>().SetBool("closeResult", true);
                StartCoroutine(Delay(1.5f, () =>
                {
                    this.transform.Find("PoseorGameEnd").gameObject.SetActive(true);
                }));
            }));
        }
    }
    //PvPをセレクト
    private void SelectPvp()
    {
        if (!selectEnd)
        {
            if (!m_calledOnce)
            {
                this.transform.Find("Finish").gameObject.SetActive(true);
                GameObject SelectPvp = this.transform.Find("SelectPVP").gameObject;
                SelectPvpPlayer1 = SelectPvp.transform.Find("Player1").GetComponent<TextMeshProUGUI>();
                SelectPvpPlayer2 = SelectPvp.transform.Find("Player2").GetComponent<TextMeshProUGUI>();
                SelectPvp.SetActive(true);
                m_calledOnce = true;
            }
            if (!finishFedeOut)
            {
                FinishFade();
            }
            else if (finishFedeOut)
            {
                //1位プレイヤー1
                //2位プレイヤー2
                if (RaceManager.instance.resultRanking == 1)
                {
                    SelectPvpPlayer1.color = winColor;
                    SelectPvpPlayer2.color = loseColor;
                    RankFade(SelectPvpPlayer1, SelectPvpPlayer2);
                    SelectPvpPlayer1.text = "Win";
                    SelectPvpPlayer2.text = "Lose";
                }
                //1位プレイヤー2
                //2位プレイヤー1
                else if (RaceManager.instance.resultRanking == 2)
                {
                    SelectPvpPlayer1.color = loseColor;
                    SelectPvpPlayer2.color = winColor;
                    RankFade(SelectPvpPlayer2, SelectPvpPlayer1);
                    SelectPvpPlayer1.text = "Lose";
                    SelectPvpPlayer2.text = "Win";
                }
                //同率
                else
                {
                    SelectPvpPlayer1.color = drawColor;
                    SelectPvpPlayer2.color = drawColor;
                    SelectPvpPlayer1.fontMaterial.SetFloat("_FaceDilate", 0);
                    SelectPvpPlayer2.fontMaterial.SetFloat("_FaceDilate", 0);
                    SelectPvpPlayer1.text = "Draw";
                    SelectPvpPlayer2.text = "Draw";
                    StartCoroutine(Delay(1.0f, () =>
                    {
                        selectEnd = true;
                    }));
                }
            }
        }
        else
        {
            this.transform.Find("PoseorGameEnd").gameObject.SetActive(true);
        }
    }
    //1位から2位の順番に表示する
    //TextMeshProUGUI　1位のText, TextMeshProUGUI 2位のText
    private void RankFade(TextMeshProUGUI firstPlace, TextMeshProUGUI SecondPlace)
    {
        //AIをセレクトしている場合
        if (SecondPlace == null)
        {
            if (!firstPlaceEnd)
            {
                if (!comeBack)
                {
                    firstPlaceFadeIn += 0.05f;
                    if (firstPlaceFadeIn >= 1f)
                    {
                        comeBack = true;
                    }
                }
                else
                {
                    firstPlaceFadeIn += -0.03f;
                    if (firstPlaceFadeIn <= 0f)
                    {
                        firstPlaceEnd = true;
                    }
                }
                firstPlace.fontMaterial.SetFloat("_FaceDilate", firstPlaceFadeIn);
            }
            else
            {
                StartCoroutine(Delay(1.0f, () =>
                {
                    selectEnd = true;
                    GameObject panel = this.transform.Find("Panel").gameObject;
                    panel.SetActive(true);
                    if (!panelOpen)
                    {
                        if (!RaceManager.instance.timeUp)
                        {
                            panel.transform.Find("TotalTiime").GetComponent<TextMeshProUGUI>().text += RaceManager.instance.player1TotalTimetxt.text; //　TotalTiime
                            panel.transform.Find("MaxSpeed").GetComponent<TextMeshProUGUI>().text += RaceManager.instance.maxSpeed.ToString("F0");//MaxSpeed
                        }
                        else
                        {
                            panel.transform.Find("TotalTiime").GetComponent<TextMeshProUGUI>().text += "NoRecord"; //　TotalTiime
                            panel.transform.Find("MaxSpeed").GetComponent<TextMeshProUGUI>().text += "NoRecord";//MaxSpeed
                        }
                    }
                    //自己ベスト更新か調べる
                    BestTime(panel);
                    panelOpen = true;
                }));
            }
        }
        //PvPをセレクトしている場合
        else
        { 
            if (!firstPlaceEnd)
            {
                if (!comeBack)
                {
                    firstPlaceFadeIn += 0.05f;
                    if (firstPlaceFadeIn >= 1f)
                    {
                        comeBack = true;
                    }
                }
                else
                {
                    firstPlaceFadeIn += -0.03f;
                    if (firstPlaceFadeIn <= 0f)
                    {
                        firstPlaceEnd = true;
                        comeBack = false;
                    }
                }
                firstPlace.fontMaterial.SetFloat("_FaceDilate", firstPlaceFadeIn);
            }
            else
            {
                if (!comeBack)
                {
                    secondPlaceFadeIn += 0.05f;
                    if (secondPlaceFadeIn >= 1f)
                    {
                        comeBack = true;
                    }
                }
                else
                {
                    secondPlaceFadeIn += -0.03f;
                    if (secondPlaceFadeIn <= 0f)
                    {
                        selectEnd = true;
                        StartCoroutine(Delay(1.0f, () =>
                        {
                            selectEnd = true;
                        }));
                    }
                }
                SecondPlace.fontMaterial.SetFloat("_FaceDilate", secondPlaceFadeIn);
            }
        }
    }
    //Finishのフェードインフェードアウト
    private void FinishFade()
    {
        if (!comeBack)
        {
            fadeOut += 0.05f;
            if (fadeOut >= 1f)
            {
                comeBack = true;
            }
        }
        else
        {
            fadeOut += -0.03f;
            if (fadeOut <= -1f)
            {
                finishFedeOut = true;
                comeBack = false;
            }
        }
        finishTitle.SetFloat("_FaceDilate", fadeOut);
    }
    //リザルトウィンドウ
    private void PanelWindow()
    {
        if (fadeIn <= 0)
        {
            fadeIn += 0.1f;
        }
        material.SetFloat("_FaceDilate", fadeIn);
        animator[0].enabled = true;
        if (selectAnimation != 3)
        {
            if (animCount > animationTrigger)
            {
                animator[selectAnimation].enabled = true;
                animationTrigger += 0.3f;
                selectAnimation++;
            }
            animCount += Time.deltaTime;
        }
    }
    //自己ベスト更新か調べる
    //GameObject　リザルトのオブジェクト
    private void BestTime(GameObject panel)
    {
        if (!RaceManager.instance.timeUp)
        {
            if (m_personalBest[0] > RaceManager.instance.minute)
            {
                m_personalBest[0] = RaceManager.instance.minute;
                m_personalBest[1] = RaceManager.instance.seconds;
            }
            else if (m_personalBest[0] == RaceManager.instance.minute && m_personalBest[1] > RaceManager.instance.seconds)
            {
                m_personalBest[1] = RaceManager.instance.seconds;
            }
        }
        if (!personalBestEnd)
        {
            if (m_personalBest[0] >= 10)
            {
                if (m_personalBest[1] >= 10)
                {
                    panel.transform.Find("PersonalBest").GetComponent<TextMeshProUGUI>().text += m_personalBest[0] + "." + m_personalBest[1].ToString("F3");// PersonalBest
                }
                else
                {
                    panel.transform.Find("PersonalBest").GetComponent<TextMeshProUGUI>().text += m_personalBest[0] + ".0" + m_personalBest[1].ToString("F3");// PersonalBest
                }
            }
            else
            {
                if (m_personalBest[1] >= 10)
                {
                    panel.transform.Find("PersonalBest").GetComponent<TextMeshProUGUI>().text += m_personalBest[0].ToString("0") + "." + m_personalBest[1].ToString("F3");// PersonalBest
                }
                else
                {
                    panel.transform.Find("PersonalBest").GetComponent<TextMeshProUGUI>().text += m_personalBest[0].ToString("0") + ".0" + m_personalBest[1].ToString("F3");// PersonalBest
                }
            }
            personalBestEnd = true;
        }
    }
    //遅延
    IEnumerator Delay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();

    }
}
