using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaitManager : MonoBehaviour
{
    [SerializeField] private Sprite[] _skins;
    [SerializeField] private int[] _coins;
    [SerializeField] private int[] _priceBait;

    public int GetReward(int indexBait, GameObject skin) 
    {
        Sprite spSkin = skin.GetComponent<SpriteRenderer>().sprite;
        int indexSkin = 0;

        for (int i = 0; i < _skins.Length; i++)
        {
            if (_skins[i] == spSkin) 
            {
                indexSkin = i;
                break;
            }
        }

        switch (indexBait)
        {
            case 0:
                {
                    return _coins[indexSkin];
                }
                break;
            case 1:
                {
                    return _coins[indexSkin] * 2;
                }
                break;
            case 2:
                {
                    return _coins[indexSkin] * 4;
                }
                break;
            default:
                {
                    return _coins[indexSkin];
                }
                break;
        }
    }

    public int GetPrice(int indexBait) 
    {
        return _priceBait[indexBait];
    }
}
