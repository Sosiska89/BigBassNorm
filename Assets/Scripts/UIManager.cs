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

    [SerializeField] private Color _fadeColor;

    [SerializeField] private GameObject _pauseGO;
    [SerializeField] private RectTransform _pauseRT;
    [SerializeField] private Image _fadePause;

    [HideInInspector] public bool IsOpenUI = false;
    [SerializeField] public TextMeshProUGUI _bestScoreText;
    [SerializeField] public TextMeshProUGUI _scoreText;

    public void OnClickMenu() 
    {
        Time.timeScale = 1;
        CompRoot.Instanse.FadeToScene("Menu");
    }

    private void Update()
    {
        _bestScoreText.text = CompRoot.Instanse.BestScoreFish.ToString();
        _scoreText.text = _fisherman.Fishs.ToString();
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

    [SerializeField] private GameObject _gameOverGO;
    [SerializeField] private RectTransform _gameOverRT;
    [SerializeField] private Image _gameOverFade;

    public void OnShowGameOverPopUp() 
    {
        Time.timeScale = 0;
        IsOpenUI = true;
        _gameOverGO.SetActive(true);
        _gameOverRT.DOAnchorPosY(0, 0.5f).SetUpdate(true);
        _gameOverFade.DOColor(_fadeColor, 0.5f).SetUpdate(true);
    }

    public void OnClickRestart() 
    {
        Time.timeScale = 1;
        CompRoot.Instanse.FadeToScene("MainGamePlay");
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
