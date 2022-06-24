using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHighlight : MonoBehaviour
{
    //�L�[�{�[�h����̎��̏����l
    [SerializeField]
    private GameObject firstSelect;

    //�L�[�{�[�h����̎��̃}�X�^�[����
    [SerializeField]
    private GameObject MasterButton;

    //�L�[�{�[�h����̎���BGM����
    [SerializeField]
    private GameObject BgmButton;

    //�L�[�{�[�h����̎���SE����
    [SerializeField]
    private GameObject SeButton;

    //�Z���N�g���̃{�^���I�u�W�F�N�g
    private GameObject Button;
    //��O�̃Z���N�g�{�^��
    private GameObject prevButton;
    //�{�^���������炤
    private GameObject ButtonBuf;
    //�{�^���ɃG�t�F�N�g������z��
    private GameObject[] ButtonEffect = new GameObject[3];
    //��񂾂��ʂ�����
    private int i;
    //�A���h�~
    private float coolTime = 1f;
    //���ʃo�[�Ɉړ����������ĂȂ���
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
                                * �I�𒆂̃{�^���̎q�v�f���擾(1�Ԗڂ̗v�f�͕����Ȃ̂Ŕ�΂�)
                                */
                            foreach (Transform child in Button.transform)
                            {
                                ButtonEffect[j] = child.gameObject;
                                if (j > 0) ButtonEffect[j].SetActive(true);
                                j++;
                            }
                            /*
                            * �e�L�X�g�̃G�t�F�N�g��ON�ɂ���
                            */
                            ButtonEffect[0].GetComponent<TextHighlight>().enabled = true;
                            j = 0;
                            /*
                                * ��O�ɂɑI�����Ă����{�^���̎q�v�f���擾(1�Ԗڂ̗v�f�͕����Ȃ̂Ŕ�΂�)
                                */
                            foreach (Transform child in ButtonBuf.transform)
                            {
                                ButtonEffect[j] = child.gameObject;
                                if (j > 0) ButtonEffect[j].SetActive(false);
                                j++;
                            }
                            /*
                            * �e�L�X�g�̃G�t�F�N�g��OFF�ɂ���
                            */
                            ButtonEffect[0].GetComponent<TextHighlight>().enabled = false;
                            /*
                            * �e�L�X�g�̃G�t�F�N�g�𕁒ʂ̏��(0)�ɖ߂�
                            */
                            TextMeshProUGUI tmPro = ButtonEffect[0].GetComponent<TextMeshProUGUI>();
                            Material material = tmPro.fontMaterial;
                            material.SetFloat("_OutlineWidth", 0);
                        }
                        /*
                            * ���ݑI������Ă���{�^����ButtonBuf�ɑ��(���݂̃{�^������O�ɑI�����Ă����{�^���ɐݒ肷��)
                            */
                        ButtonBuf = Button;
                    }
                }
            }
        }
        coolTime += Time.deltaTime;
    }
}
