using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LodingScreenController : MonoBehaviour
{
    //非同期
    private AsyncOperation async;
    [SerializeField]
    private Slider slider;
    bool trigger = false;
    //タイムカウント
    float count = 0;
    //進捗
    float progress;
    void Update()
    {
        count += Time.deltaTime;
        if (count > 1f && !trigger)
        {
            //コルーチン実行
            StartCoroutine(LoadSceneCoroutine());
            StartCoroutine(LoadAnim());
            trigger = true;
        }
        slider.value += Time.deltaTime;
    }
    //ロードバー
    IEnumerator LoadSceneCoroutine()
    {
        // Unityが安定するまで10秒待つ
        int lefttime = 10;
        async = SceneManager.LoadSceneAsync("GameScene");
        async.allowSceneActivation = false;
        slider.value = 0f;
        // シーンのロードが終わるまで待機
        while (true)
        {
            yield return null;
            progress = async.progress;
            yield return new WaitForSeconds(1.0f);
            --lefttime;
            if (async.progress >= 0.9f)
            {
                slider.value = 1f;
                async.allowSceneActivation = true;
                yield return new WaitForSeconds(1f);
                break;
            }
        }
    }
    //サークルローディング
    IEnumerator LoadAnim()
    {
        yield return null;
    }
}
