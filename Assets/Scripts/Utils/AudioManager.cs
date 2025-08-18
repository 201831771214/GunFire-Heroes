using UnityEngine;
using System.Collections.Generic;

namespace GunFireHeroes.Utils
{
    /// <summary>
    /// 音频管理器，负责背景音乐和音效的播放
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [Header("音频源")]
        public AudioSource bgmSource;
        public AudioSource sfxSource;
        
        [Header("音频配置")]
        public AudioClip[] backgroundMusics;
        public AudioClip[] soundEffects;
        
        [Header("音量设置")]
        [Range(0f, 1f)]
        public float masterVolume = 1f;
        [Range(0f, 1f)]
        public float bgmVolume = 0.7f;
        [Range(0f, 1f)]
        public float sfxVolume = 1f;
        
        private Dictionary<string, AudioClip> bgmDict;
        private Dictionary<string, AudioClip> sfxDict;
        private List<AudioSource> sfxSources;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudio();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            LoadAudioSettings();
        }
        
        private void InitializeAudio()
        {
            // 初始化音频字典
            bgmDict = new Dictionary<string, AudioClip>();
            sfxDict = new Dictionary<string, AudioClip>();
            sfxSources = new List<AudioSource>();
            
            // 添加背景音乐
            foreach (var bgm in backgroundMusics)
            {
                if (bgm != null)
                    bgmDict[bgm.name] = bgm;
            }
            
            // 添加音效
            foreach (var sfx in soundEffects)
            {
                if (sfx != null)
                    sfxDict[sfx.name] = sfx;
            }
            
            // 创建额外的音效源（用于同时播放多个音效）
            for (int i = 0; i < 5; i++)
            {
                GameObject sfxObj = new GameObject($"SFX_Source_{i}");
                sfxObj.transform.SetParent(transform);
                AudioSource source = sfxObj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                sfxSources.Add(source);
            }
            
            // 设置默认音频源
            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true;
                bgmSource.playOnAwake = false;
            }
            
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }
        }
        
        /// <summary>
        /// 播放背景音乐
        /// </summary>
        public void PlayBGM(string bgmName, bool loop = true, float fadeTime = 1f)
        {
            if (!bgmDict.ContainsKey(bgmName))
            {
                Debug.LogWarning($"背景音乐不存在: {bgmName}");
                return;
            }
            
            AudioClip clip = bgmDict[bgmName];
            
            if (bgmSource.clip == clip && bgmSource.isPlaying)
                return;
                
            if (fadeTime > 0f && bgmSource.isPlaying)
            {
                StartCoroutine(FadeBGM(clip, loop, fadeTime));
            }
            else
            {
                bgmSource.clip = clip;
                bgmSource.loop = loop;
                bgmSource.volume = bgmVolume * masterVolume;
                bgmSource.Play();
            }
        }
        
        /// <summary>
        /// 停止背景音乐
        /// </summary>
        public void StopBGM(float fadeTime = 1f)
        {
            if (fadeTime > 0f)
            {
                StartCoroutine(FadeOutBGM(fadeTime));
            }
            else
            {
                bgmSource.Stop();
            }
        }
        
        /// <summary>
        /// 播放音效
        /// </summary>
        public void PlaySFX(string sfxName, float volume = 1f, float pitch = 1f)
        {
            if (!sfxDict.ContainsKey(sfxName))
            {
                Debug.LogWarning($"音效不存在: {sfxName}");
                return;
            }
            
            AudioClip clip = sfxDict[sfxName];
            AudioSource source = GetAvailableSFXSource();
            
            if (source != null)
            {
                source.clip = clip;
                source.volume = sfxVolume * masterVolume * volume;
                source.pitch = pitch;
                source.Play();
            }
        }
        
        /// <summary>
        /// 在指定位置播放3D音效
        /// </summary>
        public void PlaySFX3D(string sfxName, Vector3 position, float volume = 1f, float pitch = 1f)
        {
            if (!sfxDict.ContainsKey(sfxName))
            {
                Debug.LogWarning($"音效不存在: {sfxName}");
                return;
            }
            
            AudioClip clip = sfxDict[sfxName];
            AudioSource.PlayClipAtPoint(clip, position, sfxVolume * masterVolume * volume);
        }
        
        /// <summary>
        /// 获取可用的音效源
        /// </summary>
        private AudioSource GetAvailableSFXSource()
        {
            // 优先使用主音效源
            if (!sfxSource.isPlaying)
                return sfxSource;
                
            // 查找空闲的额外音效源
            foreach (var source in sfxSources)
            {
                if (!source.isPlaying)
                    return source;
            }
            
            // 如果都在播放，使用第一个额外音效源
            return sfxSources.Count > 0 ? sfxSources[0] : sfxSource;
        }
        
        /// <summary>
        /// 设置主音量
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();
            SaveAudioSettings();
        }
        
        /// <summary>
        /// 设置背景音乐音量
        /// </summary>
        public void SetBGMVolume(float volume)
        {
            bgmVolume = Mathf.Clamp01(volume);
            bgmSource.volume = bgmVolume * masterVolume;
            SaveAudioSettings();
        }
        
        /// <summary>
        /// 设置音效音量
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            SaveAudioSettings();
        }
        
        /// <summary>
        /// 更新所有音量
        /// </summary>
        private void UpdateAllVolumes()
        {
            bgmSource.volume = bgmVolume * masterVolume;
            // 音效音量会在播放时应用
        }
        
        /// <summary>
        /// 静音/取消静音
        /// </summary>
        public void SetMute(bool mute)
        {
            bgmSource.mute = mute;
            sfxSource.mute = mute;
            
            foreach (var source in sfxSources)
            {
                source.mute = mute;
            }
        }
        
        /// <summary>
        /// 暂停/恢复背景音乐
        /// </summary>
        public void PauseBGM()
        {
            bgmSource.Pause();
        }
        
        public void ResumeBGM()
        {
            bgmSource.UnPause();
        }
        
        /// <summary>
        /// 背景音乐淡入淡出
        /// </summary>
        private System.Collections.IEnumerator FadeBGM(AudioClip newClip, bool loop, float fadeTime)
        {
            float startVolume = bgmSource.volume;
            
            // 淡出
            for (float t = 0; t < fadeTime / 2; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0, t / (fadeTime / 2));
                yield return null;
            }
            
            // 切换音乐
            bgmSource.clip = newClip;
            bgmSource.loop = loop;
            bgmSource.Play();
            
            // 淡入
            for (float t = 0; t < fadeTime / 2; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(0, bgmVolume * masterVolume, t / (fadeTime / 2));
                yield return null;
            }
            
            bgmSource.volume = bgmVolume * masterVolume;
        }
        
        /// <summary>
        /// 背景音乐淡出
        /// </summary>
        private System.Collections.IEnumerator FadeOutBGM(float fadeTime)
        {
            float startVolume = bgmSource.volume;
            
            for (float t = 0; t < fadeTime; t += Time.deltaTime)
            {
                bgmSource.volume = Mathf.Lerp(startVolume, 0, t / fadeTime);
                yield return null;
            }
            
            bgmSource.Stop();
            bgmSource.volume = startVolume;
        }
        
        /// <summary>
        /// 保存音频设置
        /// </summary>
        private void SaveAudioSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.Save();
        }
        
        /// <summary>
        /// 加载音频设置
        /// </summary>
        private void LoadAudioSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            
            UpdateAllVolumes();
        }
        
        /// <summary>
        /// 添加音频剪辑
        /// </summary>
        public void AddBGM(string name, AudioClip clip)
        {
            if (clip != null)
                bgmDict[name] = clip;
        }
        
        public void AddSFX(string name, AudioClip clip)
        {
            if (clip != null)
                sfxDict[name] = clip;
        }
    }
}