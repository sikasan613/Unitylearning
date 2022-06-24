using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class CameraScreenPointer : MonoBehaviour
{
    //サウンド管理クラス
    SoundManager soundMgr;
    //Aiオブジェクト
    public GameObject AiObj;
    //PVPオブジェクト
    public GameObject PvpObj;
    //Aiムービー
    private  VideoPlayer AiMove;
    //PVPムービー
    private VideoPlayer PvpMove;
    //選択中に音を連続させないため
    private bool selected = false;
    // Start is called before the first frame update
    void Start()
    {
        soundMgr = FindObjectOfType<SoundManager>();
        AiMove = AiObj.GetComponent<VideoPlayer>();
        PvpMove = PvpObj.GetComponent<VideoPlayer>();
        AiMove.Play();
        PvpMove.Play();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit_info = new RaycastHit();
        float max_distance = 10000f;
        bool hit = Physics.Raycast(ray, out hit_info, max_distance);
        if (hit)
        {
            if (hit_info.transform.name == AiObj.name)
            {
                if (!selected)
                {
                    AiMove.Play();
                    soundMgr.PlaySeByName("decide14");
                }
            }
            else if (hit_info.transform.name == PvpObj.name)
            {
                if (!selected)
                {
                    PvpMove.Play();
                    soundMgr.PlaySeByName("decide14");
                }
            }
            selected = true;
        }
        else
        {
            AiMove.Stop();
            PvpMove.Stop();
            selected = false;
        }
    }
}
