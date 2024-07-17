using System;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;
using Random = UnityEngine.Random;

public class AudioSystem : MonoBehaviour
{
    public static AudioSystem AudioSys = null;

    public enum Bgm { No, Dungeon, Battle }

    public enum Sfx { No, Walk, DiceRoll, Atk, Dmg, Def, Dge }

    [SerializeField]
    private AudioSource _bgmPlayer;

    [SerializeField]
    private AudioSource[] _sfxPlayer;

    [SerializeField]
    private int _sfxCursor = 0;

    [Serializable]
    class SoundInfo
    {
        public AudioClip[] Audio;
        public bool IsPlaying;
    }

    [SerializeField]
    private SerializableDictionaryBase<Bgm, SoundInfo> _bgmDic;
    [SerializeField]
    private SerializableDictionaryBase<Sfx, SoundInfo> _sfxDic;

    private void Awake()
    {
        if (AudioSys)
        {
            DestroyImmediate(gameObject);
            return;
        }
        else
        {
            AudioSys = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Play_Bgm(Bgm bgm)
    {
        _bgmPlayer.clip = _bgmDic[bgm].Audio[Random.Range(0, _bgmDic[bgm].Audio.Length)];
        _bgmPlayer.Play();
    }

    public void Play_Sfx(Sfx sfx)
    {
        _sfxPlayer[_sfxCursor].clip = _sfxDic[sfx].Audio[Random.Range(0, _sfxDic[sfx].Audio.Length)];
        _sfxPlayer[_sfxCursor].Play();

        _sfxCursor = (_sfxCursor + 1) % _sfxPlayer.Length;
    }
}
