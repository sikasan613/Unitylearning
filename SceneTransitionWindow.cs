using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionWindow : MonoBehaviour
{
    //サウンド管理クラス
    SoundManager soundMgr;
    
    public void ReStart()
    {
        SceneManager.LoadScene("LoadingScene");
    }
    public void Select()
    {
        soundMgr = FindObjectOfType<SoundManager>();
        soundMgr.ChangeBgm();
        soundMgr.PlayBgmByName("Ride_out");
        SceneManager.LoadScene("SelectScene");
    }
    public void Exit()
    {
        #if UNITY_EDITOR
		            UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
}
