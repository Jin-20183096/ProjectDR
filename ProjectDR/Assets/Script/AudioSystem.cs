using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
