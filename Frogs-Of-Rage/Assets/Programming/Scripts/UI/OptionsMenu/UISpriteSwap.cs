using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//for editor script (see bottom)
#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

//class for swapping out a sprite on a UI element, used in the Options menu.
//attatched to sliders that range from 0 to 10 or 1 to 10(Mouse sensitivity)
public class UISpriteSwap : MonoBehaviour
{
    //slider that controls when the sprite updates
    [SerializeField] private Slider _updateSlider;

    //array of sprite and value pairings used to control when the sprite changes
    [SerializeField] private SpriteIntegerPairing[] _swapPairings;
    public SpriteIntegerPairing[] SwapPairings
    {
        get { return _swapPairings; }
        set { _swapPairings = value; }
    }

    //Image component that has it's sprite updated, gotten through code
    private Image _spriteImage;

    //public function called from sliders that changes the sprites
    public void UpdateSprite()
    {
        if(_spriteImage == null)
        {
            _spriteImage = GetComponent<Image>();
        }

        if(_spriteImage != null && _swapPairings != null && _updateSlider != null)
        {
            for (int i = 0; i < _swapPairings.Length; i++)
            {
                if(_updateSlider.value <= _swapPairings[i].value)
                {
                    if(_swapPairings[i].sprite != null)
                    {
                        _spriteImage.sprite = _swapPairings[i].sprite;
                        return;
                    }
                }
            }

            //none was set, sets to highest index in swappairings
            if (_swapPairings[_swapPairings.Length - 1].sprite != null)
            {
                _spriteImage.sprite = _swapPairings[_swapPairings.Length - 1].sprite;
            }

        }
    }

    //updates sprites initialy
    private void OnEnable()
    {
        UpdateSprite();
    }
}

//struct used to organize what sprites should display when easier, and is editable in the inspector
[System.Serializable]
public struct SpriteIntegerPairing
{
    public Sprite sprite;
    public int value;
}


//custom editor that sorts the pairings by lowest to highest value automaticaly
#if UNITY_EDITOR
[CustomEditor(typeof(UISpriteSwap))]
public class UISpriteSwapEditor: Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UISpriteSwap spriteSwapTarget = (UISpriteSwap)target;

        if (spriteSwapTarget != null)
        {
            if(spriteSwapTarget.SwapPairings != null)
            {
                List<SpriteIntegerPairing> pairingList = spriteSwapTarget.SwapPairings.ToList();
                pairingList.Sort(SortPairings);

                spriteSwapTarget.SwapPairings = pairingList.ToArray();
            }
        }
    }

    private int SortPairings(SpriteIntegerPairing a, SpriteIntegerPairing b)
    {
        if(a.value > b.value)
        {
            return 1;
        }
        else if(a.value < b.value)
        {
            return - 1;
        }
        return 0;
    }
}
#endif

