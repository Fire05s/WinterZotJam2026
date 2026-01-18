using System.Linq;
using UnityEngine;

[System.Serializable]
public class AudioAsset
{
    public AudioType Type;
    public AudioClip[] SoundAssets;

    private int _currentSoundStep = 0;

    public AudioClip GetCurrentAudio()
    {
        _currentSoundStep++;
        if (_currentSoundStep >= SoundAssets.Count()) _currentSoundStep = 1;

        return SoundAssets[_currentSoundStep - 1];
    }
}

public enum AudioType
{
    PlayerAttack,
    PlayerEquip,
    PlayerThrow,
    ItemImpact,
    Chain,
    TorchOn,
    TorchOff,
    NPCDeath,
    NPCWalk,
    ToppleBookShelf,
    WoodBreak,
    NPCAlert,
    NPCCurious,
    DoorOpen
}

