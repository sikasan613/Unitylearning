using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHighlight : MonoBehaviour
{
    //キーボード操作の時の初期値
    [SerializeField]
    private GameObject firstSelect;

    //キーボード操作の時のマスター音量
    [SerializeField]
    private GameObject MasterButton;

    //キーボード操作の時のBGM音量
    [SerializeField]
    private GameObject BgmButton;

    //キーボード操作の時のSE音量
    [SerializeField]
    private GameObject SeButton;

    //セレクト中のボタンオブジェクト
    private GameObject Button;
    //一個前のセレクトボタン
    private GameObject prevButton;
    //ボタン情報をもらう
    private GameObject ButtonBuf;
    //ボタンにエフェクトをつける配列
    private GameObject[] ButtonEffect = new GameObject[3];
    //一回だけ通すもの
    private int i;
    //連続防止
    private float coolTime = 1f;
    //音量バーに移動したかしてないか
    private bool selected;



    // Start is called before the first frame update
    void Start()
    {
        EventSystem.current.SetSelectedGameObject(firstSelect);
        Button = EventSystem.current.currentSelectedGameObject;
        prevButton = Button;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                EventSystem.current.SetSelectedGameObject(prevButton);
            }
        }
        if (coolTime > 0.5)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                coolTime = 0;
                for (int i = 4; i <= 6; i++)
                {
                    if (Button.name == "Button" + i)
                    {
                        selected = true;
                    }
                }
                if (Button.name == "MasterVolmeter")
                {
                    selected = false;
                }
                else if (Button.name == "BgmVolmeter")
                {
                    selected = false;
                }
                else if (Button.name == "SeVolmeter")
                {
                    selected = false;
                }
                Button = EventSystem.current.currentSelectedGameObject;
                prevButton = Button;
            }
            else if (!selected)
            {
                if (EventSystem.current.currentSelectedGameObject == null) return;
                Button = EventSystem.current.currentSelectedGameObject;
                prevButton = Button;
                if (Button != null)
                {
                    if(Button.name == "MasterVolmeter")
                    {
                        EventSystem.current.SetSelectedGameObject(MasterButton);
                    }
                    else if (Button.name == "BgmVolmeter")
                    {
                        EventSystem.current.SetSelectedGameObject(BgmButton);
                    }
                    else if (Button.name == "SeVolmeter")
                    {
                        EventSystem.current.SetSelectedGameObject(SeButton);
                    }
                    if (Button.name != "MasterVolmeter" && Button.name != "BgmVolmeter" && Button.name != "SeVolmeter")
                    {
                        if (i == 0)
                        {
                            ButtonBuf = Button;
                            i++;
                        }
                        if (Button != ButtonBuf)
                        {
                            int j = 0;
                            /*
                                * 選択中のボタンの子要素を取得(1番目の要素は文字なので飛ばす)
                                */
                            foreach (Transform child in Button.transform)
                            {
                                ButtonEffect[j] = child.gameObject;
                                if (j > 0) ButtonEffect[j].SetActive(true);
                                j++;
                            }
                            /*
                            * テキストのエフェクトをONにする
                            */
                            ButtonEffect[0].GetComponent<TextHighlight>().enabled = true;
                            j = 0;
                            /*
                                * 一つ前にに選択していたボタンの子要素を取得(1番目の要素は文字なので飛ばす)
                                */
                            foreach (Transform child in ButtonBuf.transform)
                            {
                                ButtonEffect[j] = child.gameObject;
                                if (j > 0) ButtonEffect[j].SetActive(false);
                                j++;
                            }
                            /*
                            * テキストのエフェクトをOFFにする
                            */
                            ButtonEffect[0].GetComponent<TextHighlight>().enabled = false;
                            /*
                            * テキストのエフェクトを普通の状態(0)に戻す
                            */
                            TextMeshProUGUI tmPro = ButtonEffect[0].GetComponent<TextMeshProUGUI>();
                            Material material = tmPro.fontMaterial;
                            material.SetFloat("_OutlineWidth", 0);
                        }
                        /*
                            * 現在選択されているボタンをButtonBufに代入(現在のボタンを一つ前に選択していたボタンに設定する)
                            */
                        ButtonBuf = Button;
                    }
                }
            }
        }
        coolTime += Time.deltaTime;
    }
}
