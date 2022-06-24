using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour
{
    //�T�E���h�Ǘ��N���X
    SoundManager soundMgr;

    private GameObject Button;
    GameObject[] SelectCanvas;
    private float coolTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        SelectCanvas = GameObject.FindGameObjectsWithTag("Canvas");
        soundMgr = FindObjectOfType<SoundManager>();
        soundMgr.PlayBgmByName("Ride_out");
        Button = EventSystem.current.currentSelectedGameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (coolTime > 0.5)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (Button.name == "MasterVolmeter")
                {
                    GameObject selectVol = transform.parent.gameObject;
                    selectVol = selectVol.transform.Find("Button4").gameObject;
                    EventSystem.current.SetSelectedGameObject(selectVol);
                }
                else if(Button.name == "BgmVolmeter")
                {
                    GameObject selectVol = transform.parent.gameObject;
                    selectVol = selectVol.transform.Find("Button5").gameObject;
                    EventSystem.current.SetSelectedGameObject(selectVol);
                }
                else if(Button.name == "SeVolmeter")
                {
                    GameObject selectVol = transform.parent.gameObject;
                    selectVol = selectVol.transform.Find("Button6").gameObject;
                    EventSystem.current.SetSelectedGameObject(selectVol);
                }
            }
        }
        Button = EventSystem.current.currentSelectedGameObject;
        coolTime += Time.deltaTime;
    }

    //�Q�[���J�n
    public void StartGame()
    {
        SceneManager.LoadScene("SelectScene");
        soundMgr.PlaySeByName("decide14");
    }
    //�I�v�V������ʈڍs
    public void Option()
    {
        SelectCanvas[0].transform.Find("ButtonSummary").gameObject.SetActive(false);
        SelectCanvas[0].GetComponent<Canvas>().enabled = false;
        SelectCanvas[1].GetComponent<Canvas>().enabled = true;
        SelectCanvas[1].transform.Find("Manu").gameObject.SetActive(true);
        GameObject selectVol = SelectCanvas[1].transform.Find("Manu/Select/Button4").gameObject;
        GameObject selectVolmeter = SelectCanvas[1].transform.Find("Manu/Select").gameObject;
        EventSystem.current.SetSelectedGameObject(selectVol);
        selectVolmeter.transform.Find("Button4/Mastertxt/MasterVolmeter").GetComponent<Slider>().value = soundMgr.Volume;
        selectVolmeter.transform.Find("Button5/Bgmtxt/BgmVolmeter").GetComponent<Slider>().value = soundMgr.BgmVolume;
        selectVolmeter.transform.Find("Button6/Setxt/SeVolmeter").GetComponent<Slider>().value = soundMgr.SeVolume;
        soundMgr.PlaySeByName("decide14");
    }

    //�Q�[���I��
    public void ExitGame()
    {
        #if UNITY_EDITOR
		    UnityEditor.EditorApplication.isPlaying = false;
            soundMgr.PlaySeByName("decide13");
        #else
            Application.Quit();
            soundMgr.PlaySeByName("decide13");
        #endif
    }
    //�I�v�V������ʂ�ۑ����ĕ���
    public void OptionExit()
    {
        SelectCanvas[1].transform.Find("Manu").gameObject.SetActive(false);
        SelectCanvas[1].GetComponent<Canvas>().enabled = false;
        SelectCanvas[0].GetComponent<Canvas>().enabled = true;
        SelectCanvas[0].transform.Find("ButtonSummary").gameObject.SetActive(true);
        GameObject selectVol = SelectCanvas[0].transform.Find("ButtonSummary/Button1").gameObject;
        EventSystem.current.SetSelectedGameObject(selectVol);
        GameObject selectVolmeter = SelectCanvas[1].transform.Find("Manu/Select").gameObject;
        soundMgr.Volume = selectVolmeter.transform.Find("Button4/Mastertxt/MasterVolmeter").GetComponent<Slider>().value;
        soundMgr.BgmVolume = selectVolmeter.transform.Find("Button5/Bgmtxt/BgmVolmeter").GetComponent<Slider>().value;
        soundMgr.SeVolume = selectVolmeter.transform.Find("Button6/Setxt/SeVolmeter").GetComponent<Slider>().value;
        soundMgr.PlaySeByName("decide13");
    }
    //�I�v�V������ʂ�ۑ����Ȃ��ŕ���
    public void OptionCancelExit()
    {
        SelectCanvas[1].transform.Find("Manu").gameObject.SetActive(false);
        SelectCanvas[1].GetComponent<Canvas>().enabled = false;
        SelectCanvas[0].GetComponent<Canvas>().enabled = true;
        SelectCanvas[0].transform.Find("ButtonSummary").gameObject.SetActive(true);
        GameObject selectVol = SelectCanvas[0].transform.Find("ButtonSummary/Button1").gameObject;
        EventSystem.current.SetSelectedGameObject(selectVol);
        soundMgr.PlaySeByName("decide13");
    }
    //�S�̂̉���
    public void MasterVol()
    { 
        GameObject selectVol = this.transform.Find("Mastertxt/MasterVolmeter").gameObject;
        EventSystem.current.SetSelectedGameObject(selectVol);
        coolTime = 0;
    }
    //BGM�̉���
    public void BgmVol()
    {
        GameObject selectVol = this.transform.Find("Bgmtxt/BgmVolmeter").gameObject;
        EventSystem.current.SetSelectedGameObject(selectVol);
    }
    //SE�̉���
    public void SeVol()
    {
        GameObject selectVol = this.transform.Find("Setxt/SeVolmeter").gameObject;
        EventSystem.current.SetSelectedGameObject(selectVol);
    }
}
