using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    CarController2 carConPlayer1;
    CarController2 carConPlayer2;
    CarController2 carConAi;
    GameObject player1;
    GameObject player2;
    GameObject Ai;
    [SerializeField, PersistentAmongPlayMode]
    float m_volume = 1;
    [SerializeField, PersistentAmongPlayMode]
    float m_bgmVolume = 1;
    [SerializeField, PersistentAmongPlayMode]
    float m_seVolume = 1;

    AudioClip[] bgm;
    AudioClip[] se;
    AudioClip[] engine;
    //音を保存して呼び出せるようにする
    Dictionary<string, int> bgmIndex = new Dictionary<string, int>();
    Dictionary<string, int> seIndex = new Dictionary<string, int>();
    Dictionary<string, int> engineIndex = new Dictionary<string, int>();

    private bool m_startedSound;
    private float lowPitchMin = 1f;
    private float lowPitchMax = 6f;
    private float pitchMultiplier = 1f;
    private float highPitchMultiplier = 0.25f;
    private bool useDoppler = true;
    //一度だけ通す
    private bool justonce = false;
    private float dopplerLevel = 1;
    private int m_type;
    private float[] time = new float[3];

    AudioSource bgmAudioSource;
    AudioSource seAudioSource;
    //プレイヤー１エンジン音
    AudioSource m_highAccelPlayer1;
    //プレイヤー２エンジン音
    AudioSource m_highAccelPlayer2;
    //AIエンジン音
    AudioSource m_highAccelAi;
    //プレイヤー１ドリフト音
    AudioSource m_driftPlayer1;
    //プレイヤー２ドリフト音
    AudioSource m_driftPlayer2;
    //プレイヤー２ドリフト音
    AudioSource m_driftAi;

    //マスターボリューム
    public float Volume
    {
        set
        {
            m_volume = Mathf.Clamp01(value);
            bgmAudioSource.volume = m_bgmVolume * m_volume;
            seAudioSource.volume = m_seVolume * m_volume;
        }
        get
        {
            return m_volume;
        }
    }
    //BGMボリューム
    public float BgmVolume
    {
        set
        {
            m_bgmVolume = Mathf.Clamp01(value);
            bgmAudioSource.volume = m_bgmVolume * m_volume;
        }
        get
        {
           return m_bgmVolume;
        }
    }
    //SEボリューム
    public float SeVolume
    {
        set
        {
            m_seVolume = Mathf.Clamp01(value);
            seAudioSource.volume = m_seVolume * m_volume;
        }
        get
        {
            return m_seVolume;
        }
    }
    public bool startedSound
    {
        get
        {
            return m_startedSound;
        }
    }


    override protected void Awake()
    {
        base.Awake();
        bgmAudioSource = gameObject.AddComponent<AudioSource>();
        seAudioSource = gameObject.AddComponent<AudioSource>();
        for (int i = 0; i < time.Length; i++)
        {
            time[i] = 3.80873f;
        }
        bgm = Resources.LoadAll<AudioClip>("Audio/BGM");
        se = Resources.LoadAll<AudioClip>("Audio/SE");
        engine = Resources.LoadAll<AudioClip>("Audio/engine");

        for (int i = 0; i < bgm.Length; i++)
        {
             bgmIndex.Add(bgm[i].name, i);
        }
        for (int i = 0; i < se.Length; i++)
        {
            seIndex.Add(se[i].name, i);
        }
        for (int i = 0; i < engine.Length; i++)
        {
            engineIndex.Add(engine[i].name, i);
        }
    }
    //音を流す
    public void StartSound()
    {
        //PvPだった場合
        //右からプレイヤー１の車の音
        //左からはプレイヤー２の車の音
        if (!justonce)
        {
            justonce = true;
            if (m_highAccelPlayer2 == null) return;
            m_highAccelPlayer1.panStereo = 1;
            m_highAccelPlayer1.spatialBlend = 0;
            m_driftPlayer1.panStereo = 1;
            m_driftPlayer1.spatialBlend = 0;

            m_highAccelPlayer2.panStereo = -1;
            m_highAccelPlayer2.spatialBlend = 0;
            m_driftPlayer2.panStereo = -1;
            m_driftPlayer2.spatialBlend = 0;
        }
        if (m_highAccelPlayer1 != null)
        {
            float pitch = UnLerp(lowPitchMin, lowPitchMax, carConPlayer1.KPH / carConPlayer1.maxRPM);
            pitch = Mathf.Min(lowPitchMax, pitch);
            m_highAccelPlayer1.pitch = pitch * pitchMultiplier * highPitchMultiplier;
            m_highAccelPlayer1.dopplerLevel = useDoppler ? dopplerLevel : 0;
            m_highAccelPlayer1.volume = m_seVolume * m_volume;
        }
        if(m_highAccelPlayer2 != null)
        {
            float pitch = UnLerp(lowPitchMin, lowPitchMax, carConPlayer2.KPH / carConPlayer2.maxRPM);
            pitch = Mathf.Min(lowPitchMax, pitch);
            m_highAccelPlayer2.pitch = pitch * pitchMultiplier * highPitchMultiplier;
            m_highAccelPlayer2.dopplerLevel = useDoppler ? dopplerLevel : 0;
            m_highAccelPlayer2.volume = m_seVolume * m_volume;
        }
        if (m_highAccelAi != null)
        {
            float pitch = UnLerp(lowPitchMin, lowPitchMax, carConAi.KPH / carConAi.maxRPM);
            pitch = Mathf.Min(lowPitchMax, pitch);
            m_highAccelAi.pitch = pitch * pitchMultiplier * highPitchMultiplier;
            m_highAccelAi.dopplerLevel = useDoppler ? dopplerLevel : 0;
            m_highAccelAi.volume = m_seVolume * m_volume;
        }
        
    }
    //ドリフトの音を流す
    public void StartDrift(int type)
    {
        if(type == 1)
        {
            //ドリフト音を鳴らす
            PlayEngineName("Skid",1);
        }
        else if(type == 2)
        {
            PlayEngineName("Skid", 2);
        }
        else if(type == 3)
        {
            PlayEngineName("Skid", 3);
        }
    }
    //ドリフトの音を止める
    public void StopDrift(int type)
    {
        if (type == 1)
        {
            m_driftPlayer1.Stop();
            time[0] = 3.80873f;
        }
        else if (type == 2)
        {
            m_driftPlayer2.Stop();
            time[1] = 3.80873f;
        }
        else if (type == 3)
        {
            m_driftAi.Stop();
            time[2] = 3.80873f;
        }
    }
    //エンジンを準備する
    public void InitializeEngineSE(int type)
    {
        player1 = GameObject.FindGameObjectWithTag("Player1");
        player2 = GameObject.FindGameObjectWithTag("Player2");
        Ai = GameObject.FindGameObjectWithTag("AI");
        m_type = type;
        if(type == 1)
        {
            m_highAccelPlayer1 = PlaySetEngineName("up");
            m_driftPlayer1 = PlaySetEngineName("Skid");
        }
        else if(type == 2)
        {
            m_highAccelPlayer2 = PlaySetEngineName("up");
            m_driftPlayer2 = PlaySetEngineName("Skid");
        }
        else if(type == 3)
        {
            m_highAccelAi = PlaySetEngineName("up");
            m_driftAi = PlaySetEngineName("Skid");
        }
        m_startedSound = true;
    }
    //エンジンの音をセットする
    private AudioSource SetUpEngineAudioSource(AudioClip clip)
    {
        if (m_type == 1)
        {
            AudioSource source = player1.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = 1;
            source.loop = true;
            source.time = Random.Range(0f, clip.length);
            source.Play();
            source.minDistance = 5;
            source.maxDistance = 50000;
            source.dopplerLevel = 0;
            carConPlayer1 = player1.GetComponent<CarController2>();

            return source;
        }
        else if (m_type == 2)
        {
            AudioSource source = player2.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = 1;
            source.loop = true;
            source.time = Random.Range(0f, clip.length);
            source.Play();
            source.minDistance = 5;
            source.maxDistance = 50000;
            source.dopplerLevel = 0;
            carConPlayer2 = player2.GetComponent<CarController2>();
            return source;
        }
        else if (m_type == 3)
        {
            AudioSource source = Ai.AddComponent<AudioSource>();
            source.clip = clip;
            source.spatialBlend = 1;
            source.loop = true;
            source.time = Random.Range(0f, clip.length);
            source.Play();
            source.minDistance = 5;
            source.maxDistance = 50000;
            source.dopplerLevel = 0;
            carConAi = Ai.GetComponent<CarController2>();
            return source;
        }
        return null;
    }
    //BGMの名前からunityで保存されてるのを引っ張ってくる
    public int GetBgmIndex(string name)
    {
        if (bgmIndex.ContainsKey(name))
        {
            return bgmIndex[name];
        }
        else
        {
            Debug.Log("指定された名前のSEファイルが存在しません。");
            return 0;
        }
    }
    //SEの名前からunityで保存されてるのを引っ張ってくる
    public int GetSeIndex(string name)
    {
        if(seIndex.ContainsKey(name))
        {
            return seIndex[name];
        }
        else
        {
            Debug.Log("指定された名前のSEファイルが存在しません。");
            return 0;
        }
    }
    //Engineの名前からunityで保存されてるのを引っ張ってくる
    public int GetEngineIndex(string name)
    {
        if (engineIndex.ContainsKey(name))
        {
            return engineIndex[name];
        }
        else
        {
            Debug.Log("指定された名前のEngineファイルが存在しません。");
            return 0;
        }
    }

    //BGM
    public void PlayBgm(int index)
    {
        if (bgmAudioSource.clip != null) return;

        index = Mathf.Clamp(index, 0, bgm.Length);
        bgmAudioSource.clip = bgm[index];
        bgmAudioSource.volume = m_bgmVolume * m_volume;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }
    //止めてたものを再生する
    public void ReStartBgm()
    {
        bgmAudioSource.UnPause();
    }
    //BGMを一時停止する
    public void StopBgm()
    {
        bgmAudioSource.Pause();
    }
    //BGM完全に止める
    public void ChangeBgm()
    {
        bgmAudioSource.Stop();
        bgmAudioSource.clip = null;
    }
    //BGMの名前を入れると再生される
    public void PlayBgmByName(string name)
    {
        PlayBgm(GetBgmIndex(name));
    }

    //システム音
    public void PlaySe(int index)
    {
        index = Mathf.Clamp(index, 0, se.Length);
        seAudioSource.PlayOneShot(se[index], m_seVolume * m_volume);
    }
    //SEを止める
    public void StopSe()
    {
        seAudioSource.Stop();
        seAudioSource.clip = null;
    }
    //SEの名前を入れると再生される
    public void PlaySeByName(string name)
    {
        PlaySe(GetSeIndex(name));
    }

    //車に関する音
    public AudioSource PlayEngine(int index)
    {
        AudioSource source = SetUpEngineAudioSource(engine[index]);
        return source;
    }
    //ドリフト音をセットする
   public AudioSource SetDriftSound(int index)
    {
        if (m_type == 1)
        {
            AudioSource source = player1.AddComponent<AudioSource>();
            return source;
        }
        else if(m_type == 2)
        {
            AudioSource source = player2.AddComponent<AudioSource>();
            return source;
        }
        else if(m_type == 3)
        {
            AudioSource source = Ai.AddComponent<AudioSource>();
            source.spatialBlend = 1;
            source.minDistance = 5;
            source.maxDistance = 50000;
            return source;
        }
        return null;
    }
    //ドリフト

    public void DriftSound(int index, int type)
    {
        index = Mathf.Clamp(index, 0, engine.Length);
        if(type == 1)
        {
            if (time[0] >= engine[index].length)
            {
                m_driftPlayer1.PlayOneShot(engine[index], m_seVolume * m_volume);
                m_driftPlayer1.time = Random.Range(0f, engine[index].length);
                time[0] = 0;
            }
            time[0] += Time.deltaTime;
        }
        else if(type == 2)
        {
            if (time[1] >= engine[index].length)
            {
                m_driftPlayer2.PlayOneShot(engine[index], m_seVolume * m_volume);
                m_driftPlayer2.time = Random.Range(0f, engine[index].length);
                time[1] = 0;
            }
            time[1] += Time.deltaTime;
        }
        else if(type == 3)
        {
            if (time[2] >= engine[index].length)
            {
                m_driftAi.PlayOneShot(engine[index], m_seVolume * m_volume);
                m_driftAi.time = Random.Range(0f, engine[index].length);
                time[2] = 0;
            }
            time[2] += Time.deltaTime;
        }

    }
    //エンジン音かドリフト音かを判断
    public AudioSource PlaySetEngineName(string name)
    {
        if (name == "up")
        {
            AudioSource source = PlayEngine(GetEngineIndex(name));
            return source;
        }
        else
        {
            AudioSource source = SetDriftSound(GetEngineIndex(name));
            return source;
        }
    }
    //ドリフト音を鳴らす
    public void PlayEngineName(string name, int type)
    {
        DriftSound(GetEngineIndex(name),type);
    }
    private static float UnLerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }
}
