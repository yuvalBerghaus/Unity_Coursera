using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_GameModel : MonoBehaviour
{
    #region globalVars
    /*    private Dictionary<string, Sprite> unitySprites;*/
    private Dictionary<string, Sprite> unityExplosionSprites;
    private Dictionary<string, Sprite> unityGeneralSprites;
    private Dictionary<string, AudioClip> unitySFX;
    private Dictionary<string, RuntimeAnimatorController> unityExplosionAnimator;
    #endregion
    #region Singleton

    private static SC_GameModel instance;
    public static SC_GameModel Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_GameModel").GetComponent<SC_GameModel>();

            return instance;
        }
    }
    #endregion
    #region Logic
    private void Init()
    {
        unityExplosionSprites = new Dictionary<string, Sprite>();
        unityGeneralSprites = new Dictionary<string, Sprite>();
        unitySFX = new Dictionary<string, AudioClip>();
        unityExplosionAnimator = new Dictionary<string, RuntimeAnimatorController>();
        List<string> _spriteExplosionToLoad = new List<string>();
        List<string> _spriteGeneral = new List<string>();
        List<string> _animatorExplosionToLoad = new List<string> ();
        List<string> _AudioSource = new List<string>();
        _AudioSource.Add("Sound_Explosion");
        _animatorExplosionToLoad.Add("Sprite_Explosion/Sprite_waterSplash_1");
        _animatorExplosionToLoad.Add("Sprite_Explosion/Explosion_29");
        _spriteExplosionToLoad.Add("Sprite_Explosion/Sprite_Explosion");
        _spriteGeneral.Add("Sprite_winner");
        _spriteGeneral.Add("Sprite_loser");
        _spriteGeneral.Add("Sprite_me");
        _spriteGeneral.Add("Sprite_enemy");
        _spriteGeneral.Add("Sprite_reset");
        _spriteExplosionToLoad.Add("Sprite_Explosion/Sprite_waterSplash");
        foreach (string key in _spriteExplosionToLoad)
        {
            Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/"+key);
            unityExplosionSprites.Add(key, sprites[0]);
        }
        foreach (string key in _spriteGeneral)
        {
            Sprite sprite = Resources.Load<Sprite>("Sprites/" + key);
            unityGeneralSprites.Add(key, sprite);
        }
        foreach (string key in _animatorExplosionToLoad)
        {
            RuntimeAnimatorController _animator = Resources.Load<RuntimeAnimatorController>("Sprites/" + key) as RuntimeAnimatorController;
            unityExplosionAnimator.Add(key, _animator);
        }
        foreach (string key in _AudioSource)
        {
            AudioClip _audio = Resources.Load<AudioClip>("Sound_FX/" + key);
            unitySFX.Add(key, _audio);
        }
    }
    public Sprite GetSprite(string _SpriteName)
    {
        Debug.Log("GetSprite");
        if (unityExplosionSprites.ContainsKey(_SpriteName))
            return unityExplosionSprites[_SpriteName];
        if(unityGeneralSprites.ContainsKey(_SpriteName))
            return unityGeneralSprites[_SpriteName];
        return null;
    }
    public RuntimeAnimatorController GetAnimator(string _AnimatorName)
    {
        if (unityExplosionAnimator.ContainsKey(_AnimatorName))
            return unityExplosionAnimator[_AnimatorName] as RuntimeAnimatorController;
        return null;
    }
    public AudioClip GetSFX(string _SFX_name)
    {
        if (unitySFX.ContainsKey(_SFX_name))
            return unitySFX[_SFX_name];
        return null;
    }
    #endregion
    #region MonoBehavior
    void Awake()
    {
        Init();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
}
