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
    //����ۑ����ČĂяo����悤�ɂ���
    Dictionary<string, int> bgmIndex = new Dictionary<string, int>();
    Dictionary<string, int> seIndex = new Dictionary<string, int>();
    Dictionary<string, int> engineIndex = new Dictionary<string, int>();

    private bool m_startedSound;
    private float lowPitchMin = 1f;
    private float lowPitchMax = 6f;
    private float pitchMultiplier = 1f;
    private float highPitchMultiplier = 0.25f;
    private bool useDoppler = true;
    //��x�����ʂ�
    private bool justonce = false;
    private float dopplerLevel = 1;
    private int m_type;
    private float[] time = new float[3];

    AudioSource bgmAudioSource;
    AudioSource seAudioSource;
    //�v���C���[�P�G���W����
    AudioSource m_highAccelPlayer1;
    //�v���C���[�Q�G���W����
    AudioSource m_highAccelPlayer2;
    //AI�G���W����
    AudioSource m_highAccelAi;
    //�v���C���[�P�h���t�g��
    AudioSource m_driftPlayer1;
    //�v���C���[�Q�h���t�g��
    AudioSource m_driftPlayer2;
    //�v���C���[�Q�h���t�g��
    AudioSource m_driftAi;

    //�}�X�^�[�{�����[��
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
    //BGM�{�����[��
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
    //SE�{�����[��
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
    //���𗬂�
    public void StartSound()
    {
        //PvP�������ꍇ
        //�E����v���C���[�P�̎Ԃ̉�
        //������̓v���C���[�Q�̎Ԃ̉�
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
    //�h���t�g�̉��𗬂�
    public void StartDrift(int type)
    {
        if(type == 1)
        {
            //�h���t�g����炷
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
    //�h���t�g�̉����~�߂�
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
    //�G���W������������
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
    //�G���W���̉����Z�b�g����
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
    //BGM�̖��O����unity�ŕۑ�����Ă�̂����������Ă���
    public int GetBgmIndex(string name)
    {
        if (bgmIndex.ContainsKey(name))
        {
            return bgmIndex[name];
        }
        else
        {
            Debug.Log("�w�肳�ꂽ���O��SE�t�@�C�������݂��܂���B");
            return 0;
        }
    }
    //SE�̖��O����unity�ŕۑ�����Ă�̂����������Ă���
    public int GetSeIndex(string name)
    {
        if(seIndex.ContainsKey(name))
        {
            return seIndex[name];
        }
        else
        {
            Debug.Log("�w�肳�ꂽ���O��SE�t�@�C�������݂��܂���B");
            return 0;
        }
    }
    //Engine�̖��O����unity�ŕۑ�����Ă�̂����������Ă���
    public int GetEngineIndex(string name)
    {
        if (engineIndex.ContainsKey(name))
        {
            return engineIndex[name];
        }
        else
        {
            Debug.Log("�w�肳�ꂽ���O��Engine�t�@�C�������݂��܂���B");
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
    //�~�߂Ă����̂��Đ�����
    public void ReStartBgm()
    {
        bgmAudioSource.UnPause();
    }
    //BGM���ꎞ��~����
    public void StopBgm()
    {
        bgmAudioSource.Pause();
    }
    //BGM���S�Ɏ~�߂�
    public void ChangeBgm()
    {
        bgmAudioSource.Stop();
        bgmAudioSource.clip = null;
    }
    //BGM�̖��O������ƍĐ������
    public void PlayBgmByName(string name)
    {
        PlayBgm(GetBgmIndex(name));
    }

    //�V�X�e����
    public void PlaySe(int index)
    {
        index = Mathf.Clamp(index, 0, se.Length);
        seAudioSource.PlayOneShot(se[index], m_seVolume * m_volume);
    }
    //SE���~�߂�
    public void StopSe()
    {
        seAudioSource.Stop();
        seAudioSource.clip = null;
    }
    //SE�̖��O������ƍĐ������
    public void PlaySeByName(string name)
    {
        PlaySe(GetSeIndex(name));
    }

    //�ԂɊւ��鉹
    public AudioSource PlayEngine(int index)
    {
        AudioSource source = SetUpEngineAudioSource(engine[index]);
        return source;
    }
    //�h���t�g�����Z�b�g����
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
    //�h���t�g

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
    //�G���W�������h���t�g�����𔻒f
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
    //�h���t�g����炷
    public void PlayEngineName(string name, int type)
    {
        DriftSound(GetEngineIndex(name),type);
    }
    private static float UnLerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }
}
