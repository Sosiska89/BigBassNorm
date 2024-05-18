using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private RectTransform _inventory;
    [SerializeField] private Image _fadeInventory;
    [SerializeField] private GameObject _inventoryGO;

    [SerializeField] private Color _finishColor;

    [SerializeField] private TextMeshProUGUI[] _selectText;
    [SerializeField] private TextMeshProUGUI[] _selectedText;

    [SerializeField] private Fisherman _fisherman;

    [SerializeField] private GameObject _noCoinsGO;
    [SerializeField] private RectTransform _noCoinsRT;
    [SerializeField] private Image _fadeNoCoins;
    [SerializeField] private Color _fadeColor;

    [SerializeField] private GameObject _pauseGO;
    [SerializeField] private RectTransform _pauseRT;
    [SerializeField] private Image _fadePause;

    public bool IsOpenUI = false;

    public void OnClickMenu() 
    {
        Time.timeScale = 1;
        CompRoot.Instanse.FadeToScene("Menu");
    }

    public void OnClickPause() 
    {
        IsOpenUI = true;
        Time.timeScale = 0;
        _pauseGO.SetActive(true);
        _pauseRT.DOAnchorPosY(0, 0.5f).SetUpdate(true);
        _fadePause.DOColor(_fadeColor, 0.5f).SetUpdate(true);
    }

    public void OnClickResume() 
    {

        Time.timeScale = 1;
        _pauseRT.DOAnchorPosY(-1500, 0.5f).SetUpdate(true);
        _fadePause.DOColor(Color.clear, 0.5f).OnComplete(() => 
        {
            _pauseGO.SetActive(false);
            _pauseRT.anchoredPosition = new Vector2(_pauseRT.anchoredPosition.x, 1500);
            IsOpenUI = false;
        }).SetUpdate(true);
    }

    private void InitializeInventory() 
    {

        for (int i = 0; i < _selectText.Length; i++)
        {
            if (_fisherman.IndexBait == i)
            {
                _selectText[i].gameObject.SetActive(false);
                _selectedText[i].gameObject.SetActive(true);
            }
            else 
            {
                _selectedText[i].gameObject.SetActive(false);
                _selectText[i].gameObject.SetActive(true);
            }
        }
    }

    public void OnClickBait(int indexBait) 
    {
        _fisherman.SetBait(indexBait);

        for (int i = 0; i < _selectText.Length; i++)
        {
            if (_fisherman.IndexBait == i)
            {
                _selectText[i].gameObject.SetActive(false);
                _selectedText[i].gameObject.SetActive(true);
            }
            else
            {
                _selectedText[i].gameObject.SetActive(false);
                _selectText[i].gameObject.SetActive(true);
            }
        }
    }

    public void OnShowNoCoinsPopUp() 
    {
        IsOpenUI = true;
        _noCoinsGO.SetActive(true);
        _noCoinsRT.DOAnchorPosY(0, 0.5f);
        _fadeNoCoins.DOColor(_fadeColor, 0.5f);
    }

    public void OnHideNoCoinsPopUp()
    {

        _noCoinsRT.DOAnchorPosY(-1500, 0.5f);
        _fadeNoCoins.DOColor(Color.clear, 0.5f).OnComplete(() => 
        {
            _noCoinsGO.SetActive(false);
            _noCoinsRT.anchoredPosition = new Vector2(_noCoinsRT.anchoredPosition.x, 1500);
            IsOpenUI = false;
        });
    }

    public void OnClickInventory() 
    {
        IsOpenUI = true;
        InitializeInventory();
        _inventoryGO.SetActive(true);
        _fadeInventory.color = Color.clear;
        _fadeInventory.DOColor(_finishColor, 0.5f);
        _inventory.DOAnchorPosY(0, 0.5f);
    }

    public void OnClickCloseInventory() 
    {
        _fadeInventory.DOColor(Color.clear, 0.5f);
        _inventory.DOAnchorPosY(-1650, 0.5f).OnComplete(delegate 
        {
            _inventory.anchoredPosition = new Vector2(_inventory.anchoredPosition.x, 1650);
            _inventoryGO.SetActive(false);

            IsOpenUI = false;
        });
    }
}
