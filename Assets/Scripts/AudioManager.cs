using System.Collections.Generic;
using UnityEngine;

public class AudioManager: MonoBehaviour
{
    [SerializeField] AudioSource _audioSource;
    [SerializeField] private AudioAsset[] _audioAssets;

    [HideInInspector] public static AudioManager Instance {get; private set;}

    private Dictionary<AudioType, AudioAsset> _audioDictionary;
    private Dictionary<AudioType, float> _audioTimerDictionary;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        _audioDictionary = new Dictionary<AudioType, AudioAsset>();
        _audioTimerDictionary = new Dictionary<AudioType, float>();

        _audioTimerDictionary.Add(AudioType.NPCWalk, 0f);

        foreach (AudioAsset asset in _audioAssets)
        {
            _audioDictionary.Add(asset.Type, asset);
        }
    }

    private bool CanPlaySound(AudioType audioType)
    {
        switch (audioType)
        {
            case AudioType.NPCWalk:
                if (_audioTimerDictionary.ContainsKey(audioType))
                {
                    float lastTimePlayed = _audioTimerDictionary[audioType];
                    float playerMoveTimerMax = 0.5f;
                    if (lastTimePlayed + playerMoveTimerMax < Time.time)
                    {
                        _audioTimerDictionary[audioType] = Time.time;
                        return true;
                    }
                    return false;
                }
                return true;
        }
        return true;
    }

    public void PlayerAudio(AudioType audioType)
    {
        if (!CanPlaySound(audioType)) return;
        _audioSource.PlayOneShot(_audioDictionary[audioType].GetCurrentAudio());
    }
}
