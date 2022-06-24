using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectSceneController : MonoBehaviour
{
    //サウンド管理クラス
    SoundManager soundMgr;
    public static int m_playType;

    // Start is called before the first frame update
    void Start()
    {
        soundMgr = FindObjectOfType<SoundManager>();
        m_playType = 0;
    }

    public static int playType
    {
        get
        {
            return m_playType;
        }
    }
    public void SelectAi()
    {
        m_playType = 0;
        SceneManager.LoadScene("LoadingScene");
        soundMgr.PlaySeByName("decide13");
    }
    public void SelectPvP()
    {
        m_playType = 1;
        SceneManager.LoadScene("LoadingScene");
        soundMgr.PlaySeByName("decide13");
    }
    public void SelectBack()
    {
        SceneManager.LoadScene("TitleScene");
        soundMgr.PlaySeByName("decide13");
    }
}
