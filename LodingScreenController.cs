using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LodingScreenController : MonoBehaviour
{
    //�񓯊�
    private AsyncOperation async;
    [SerializeField]
    private Slider slider;
    bool trigger = false;
    //�^�C���J�E���g
    float count = 0;
    //�i��
    float progress;
    void Update()
    {
        count += Time.deltaTime;
        if (count > 1f && !trigger)
        {
            //�R���[�`�����s
            StartCoroutine(LoadSceneCoroutine());
            StartCoroutine(LoadAnim());
            trigger = true;
        }
        slider.value += Time.deltaTime;
    }
    //���[�h�o�[
    IEnumerator LoadSceneCoroutine()
    {
        // Unity�����肷��܂�10�b�҂�
        int lefttime = 10;
        async = SceneManager.LoadSceneAsync("GameScene");
        async.allowSceneActivation = false;
        slider.value = 0f;
        // �V�[���̃��[�h���I���܂őҋ@
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
    //�T�[�N�����[�f�B���O
    IEnumerator LoadAnim()
    {
        yield return null;
    }
}
