using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MemoryCell : MonoBehaviour
{
    [SerializeField] private Image[] _symbols;
    [SerializeField] private Image _btnImage;
    [SerializeField] private Image _tapImage;
    [SerializeField] private GameObject _emptyText;
    [SerializeField] private MemoryGame _memoryGame;

    public int IndexSymbol { get; private set; } = 0;
    private bool _isAnim = false;
    public bool IsEmpty = false;
    private bool _isOpen = false;

    public void OnClickCell() 
    {
        if (!_isAnim && !_isOpen && _memoryGame.IsGameStart) 
        {
            OpenCell();
        }
    }

    public void OpenCell() 
    {
        _isAnim = true;
        _btnImage.transform.DOScaleX(0, 0.3f).OnComplete(() =>
        {
            if (IsEmpty)
            {
                _emptyText.SetActive(true);
            }
            else 
            {
                _symbols[IndexSymbol].gameObject.SetActive(true);
            }

            _tapImage.gameObject.SetActive(false);
            _btnImage.transform.DOScaleX(1, 0.3f).OnComplete(() => {
                _isAnim = false; 
                _isOpen = true;
                _memoryGame.OnOpenCell(this);
            });
        });
    }

    public void HideCell() 
    {
        _isAnim = true;
        _btnImage.transform.DOScaleX(0, 0.3f).OnComplete(() => 
        {
            if (IsEmpty)
            {
                _emptyText.SetActive(false);
            }
            else
            {
                foreach (var symbol in _symbols)
                {
                    symbol.gameObject.SetActive(false);
                }
            }
            _tapImage.gameObject.SetActive(true);
            _btnImage.transform.DOScaleX(1, 0.3f).OnComplete(() => { _isAnim = false; _isOpen = false; });
        });
    }

    public void SetCell(int indexSymbol) 
    {
        IndexSymbol = indexSymbol;
    }

    public void SetEmpty(bool isEmpty) 
    {
        IsEmpty = isEmpty;
    }
}
