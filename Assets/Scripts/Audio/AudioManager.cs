using Mirror;
using UnityEngine;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager instance;

    [Header("Audio Settings")]
    public AudioSource soundEffectSource; // 用于播放音效
    public AudioSource backgroundMusicSource; // 用于播放背景音乐
    public AudioClip[] audioClips; // 存储音效文件
    public AudioClip backgroundMusicClip; // 背景音乐文件

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Duplicate AudioManager found. Disabling this component.");
            enabled = false;
            return;
        }
        instance = this;
    }

    // 服务器调用播放音效
    [Command(requiresAuthority = false)]
    public void CmdPlaySound(int clipIndex, Vector3 position)
    {
        RpcPlaySound(clipIndex, position); // 在所有客户端播放音效
    }

    // 在所有客户端播放音效
    [ClientRpc]
    public void RpcPlaySound(int clipIndex, Vector3 position)
    {
        if (!isLocalPlayer) // 确保只播放一次
        {
            //AudioSource.PlayClipAtPoint(audioClips[clipIndex], position);

            // 创建一个新的 GameObject 用于播放音效
            GameObject soundObject = new GameObject("SoundEffect");
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = audioClips[clipIndex];
            audioSource.spatialBlend = 0f; // 设置为 2D 音效，不受位置影响
            audioSource.volume = 1f; // 设置音量为最大
            audioSource.Play();

            // 销毁音效播放后的 GameObject
            Destroy(soundObject, audioSource.clip.length);
        }
    }

    // 服务器调用开始播放背景音乐
    [Command(requiresAuthority = false)]
    public void CmdPlayBackgroundMusic()
    {
        RpcPlayBackgroundMusic(); // 在所有客户端播放背景音乐
    }

    // 在所有客户端播放背景音乐
    [ClientRpc]
    public void RpcPlayBackgroundMusic()
    {
        if (!backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.clip = backgroundMusicClip;
            backgroundMusicSource.loop = true; // 背景音乐循环播放
            backgroundMusicSource.Play();
        }
    }

    // 服务器调用停止背景音乐
    [Command(requiresAuthority = false)]
    public void CmdStopBackgroundMusic()
    {
        RpcStopBackgroundMusic(); // 在所有客户端停止背景音乐
    }

    // 在所有客户端停止背景音乐
    [ClientRpc]
    public void RpcStopBackgroundMusic()
    {
        if (backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }
    }
}
