using Mirror;
using UnityEngine;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager instance;

    [Header("Audio Settings")]
    public AudioSource soundEffectSource; // ���ڲ�����Ч
    public AudioSource backgroundMusicSource; // ���ڲ��ű�������
    public AudioClip[] audioClips; // �洢��Ч�ļ�
    public AudioClip backgroundMusicClip; // ���������ļ�

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

    // ���������ò�����Ч
    [Command(requiresAuthority = false)]
    public void CmdPlaySound(int clipIndex, Vector3 position)
    {
        RpcPlaySound(clipIndex, position); // �����пͻ��˲�����Ч
    }

    // �����пͻ��˲�����Ч
    [ClientRpc]
    public void RpcPlaySound(int clipIndex, Vector3 position)
    {
        if (!isLocalPlayer) // ȷ��ֻ����һ��
        {
            //AudioSource.PlayClipAtPoint(audioClips[clipIndex], position);

            // ����һ���µ� GameObject ���ڲ�����Ч
            GameObject soundObject = new GameObject("SoundEffect");
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = audioClips[clipIndex];
            audioSource.spatialBlend = 0f; // ����Ϊ 2D ��Ч������λ��Ӱ��
            audioSource.volume = 1f; // ��������Ϊ���
            audioSource.Play();

            // ������Ч���ź�� GameObject
            Destroy(soundObject, audioSource.clip.length);
        }
    }

    // ���������ÿ�ʼ���ű�������
    [Command(requiresAuthority = false)]
    public void CmdPlayBackgroundMusic()
    {
        RpcPlayBackgroundMusic(); // �����пͻ��˲��ű�������
    }

    // �����пͻ��˲��ű�������
    [ClientRpc]
    public void RpcPlayBackgroundMusic()
    {
        if (!backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.clip = backgroundMusicClip;
            backgroundMusicSource.loop = true; // ��������ѭ������
            backgroundMusicSource.Play();
        }
    }

    // ����������ֹͣ��������
    [Command(requiresAuthority = false)]
    public void CmdStopBackgroundMusic()
    {
        RpcStopBackgroundMusic(); // �����пͻ���ֹͣ��������
    }

    // �����пͻ���ֹͣ��������
    [ClientRpc]
    public void RpcStopBackgroundMusic()
    {
        if (backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }
    }
}
