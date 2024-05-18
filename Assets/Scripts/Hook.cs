using System;
using UnityEngine;

public class Hook : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _baits;

    public event Action<Fish> OnCatchFish;
    public event Action<Symbol> OnCatchSymbol;   
    private int _indexBait = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        foreach (var bait in _baits)
        {
            bait.gameObject.SetActive(false);
        }

        if (collision.gameObject.CompareTag("Fish")) 
        {
            OnCatchFish?.Invoke(collision.gameObject.GetComponent<Fish>());
        }

        if (collision.gameObject.CompareTag("Symbol"))
        {
            OnCatchSymbol?.Invoke(collision.gameObject.GetComponent<Symbol>());
        }
    }

    public void OnStartHook() 
    {
        foreach (var bait in _baits) 
        {
            bait.gameObject.SetActive(false);
        }

        _baits[_indexBait].gameObject.SetActive(true);
    }

    public void SetBait(int indexBait) 
    {
        _indexBait = indexBait;

        foreach (var bait in _baits)
        {
            bait.gameObject.SetActive(false);
        }

        _baits[_indexBait].gameObject.SetActive(true);
    }
}
