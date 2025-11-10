using UnityEngine;

/// <summary>
/// 音效管理器，统一管理游戏中的所有音效
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("音效文件")]
    [Tooltip("赋能音效 - 理解特性时播放")]
    public AudioClip empowerSound;
    
    [Tooltip("第一关摩擦木头&第三关打磨冰块音效")]
    public AudioClip rubWoodGrindIceSound;
    
    [Tooltip("鸟巢掉在草地上的音效")]
    public AudioClip birdNestDropSound;
    
    [Tooltip("第一关冻结&第三关冰面破碎的音效")]
    public AudioClip freezeIceBreakSound;
    
    [Tooltip("第三关-植物复苏音效")]
    public AudioClip plantRevivalSound;
    
    [Tooltip("石碑点亮部件音效")]
    public AudioClip tabletChargeSound;
    
    [Tooltip("第一关-柴火燃烧音效")]
    public AudioClip fireBurnSound;
    
    [Header("背景音乐")]
    [Tooltip("森林背景乐 - Level1和Level2")]
    public AudioClip forestBGM;
    
    [Tooltip("雪山背景乐 - Level3")]
    public AudioClip snowMountainBGM;

    [Header("音频源设置")]
    public AudioSource sfxSource;        // 音效音频源
    public AudioSource bgmSource;        // 背景音乐音频源

    void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 如果没有手动指定音频源，自动创建
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }
            
            if (bgmSource == null)
            {
                GameObject bgmObj = new GameObject("BGMSource");
                bgmObj.transform.SetParent(transform);
                bgmSource = bgmObj.AddComponent<AudioSource>();
                bgmSource.playOnAwake = false;
                bgmSource.loop = true;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public void PlayBGM(AudioClip clip, float volume = 0.5f)
    {
        if (bgmSource != null)
        {
            if (bgmSource.clip == clip && bgmSource.isPlaying)
            {
                return; // 已经在播放同一首音乐
            }
            
            bgmSource.clip = clip;
            bgmSource.volume = volume;
            bgmSource.Play();
        }
    }

    /// <summary>
    /// 停止背景音乐
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    // 便捷方法
    public void PlayEmpowerSound() => PlaySFX(empowerSound);
    public void PlayRubWoodGrindIceSound() => PlaySFX(rubWoodGrindIceSound);
    public void PlayBirdNestDropSound() => PlaySFX(birdNestDropSound);
    public void PlayFreezeIceBreakSound() => PlaySFX(freezeIceBreakSound);
    public void PlayPlantRevivalSound() => PlaySFX(plantRevivalSound);
    public void PlayTabletChargeSound() => PlaySFX(tabletChargeSound);
    public void PlayFireBurnSound() => PlaySFX(fireBurnSound);
    public void PlayForestBGM() => PlayBGM(forestBGM);
    public void PlaySnowMountainBGM() => PlayBGM(snowMountainBGM);
}

