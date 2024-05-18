using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Fisherman : MonoBehaviour
{
    [SerializeField] private GameObject _boat;
    [SerializeField] private float _speedBoat;
    [SerializeField] private float _clapmX;
    [SerializeField] private LineRenderer _rope;
    [SerializeField] private GameObject _hookGO;
    [SerializeField] private Transform _startPos;
    [SerializeField] private Hook _hook;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private BaitManager _baitManager;
    [SerializeField] private UIManager _uiManager;

    private bool _isHook = false;
    private Vector2 _clickPosHook = Vector2.zero;
    private float _speedHook = 4f;
    private bool _isTargetPath = true;
    private Fish _fish;
    private Symbol _symbol;

    public int IndexBait { get; set; } = 0;

    public void SetBait(int indexBait) 
    {
        IndexBait = indexBait;
        _hook.SetBait(indexBait);
    }

    private void Start()
    {
        _hook.OnCatchFish += OnCatchFish;
        _hook.OnCatchSymbol += OnCatchSymbol;
        _hook.OnStartHook();
        _hook.SetBait(IndexBait);
    }

    private void OnCatchSymbol(Symbol symbol)
    {
        if (_symbol == null && _fish == null)
        {
            _symbol = symbol;
            _isTargetPath = false;
            _symbol.OffSymbolBehaviour();
            _symbol.transform.parent = _hook.transform;
        }
    }

    private void OnCatchFish(Fish fish)
    {
        if (_fish == null && _symbol == null) 
        {
            _fish = fish;
            _isTargetPath = false;
            fish.OffFishBehaviour();
            _fish.transform.parent = _hook.transform;
        }
    }

    [SerializeField] private TextMeshProUGUI _coinsAnimText;
    private Tween _posYTween;
    private Tween _colorTween;

    private void CoinsAnimation(bool isMinus, int coins) 
    {
        if (_posYTween != null)
        {
            _posYTween.Kill();
            _posYTween = null;
        }

        if (_colorTween != null) 
        {
            _colorTween.Kill();
            _colorTween = null;
        }

        _coinsAnimText.rectTransform.anchoredPosition = new Vector2(_coinsAnimText.rectTransform.anchoredPosition.x, -60f);
        _coinsAnimText.gameObject.SetActive(true);

        if (isMinus)
        {
            _coinsAnimText.color = Color.red;
            _coinsAnimText.text = "-" + coins.ToString();
        }
        else 
        {
            _coinsAnimText.color = Color.green;
            _coinsAnimText.text = "+" + coins.ToString();
        }

        _posYTween = _coinsAnimText.rectTransform.DOAnchorPosY(-170f, 2f);
        _colorTween = _coinsAnimText.DOColor(Color.clear, 2f);
    }

    private void Update()
    {
        _coinsText.text = CompRoot.Instanse.Coins.ToString();

        if (Input.GetMouseButtonDown(0) && !_isHook && !_uiManager.IsOpenUI)
        {
            Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (clickPos.y <= 1.7f)
            {
                if (CompRoot.Instanse.Coins - _baitManager.GetPrice(IndexBait) >= 0)
                {
                    CoinsAnimation(true, _baitManager.GetPrice(IndexBait));
                    CompRoot.Instanse.Coins -= _baitManager.GetPrice(IndexBait);
                    _isHook = true;
                    _clickPosHook = clickPos;
                }
                else 
                {
                    _uiManager.OnShowNoCoinsPopUp();
                }
            }
        }
       
        if (_isHook)
        {
            if (_isTargetPath)
            {
                _hookGO.transform.position = Vector2.MoveTowards(_hookGO.transform.position, _clickPosHook, _speedHook * Time.deltaTime);

                if (Vector2.Distance(_hookGO.transform.position, _clickPosHook) <= 0.1f)
                {
                    _isTargetPath = false;
                }
            }
            else
            {
                _hookGO.transform.position = Vector2.MoveTowards(_hookGO.transform.position, _startPos.position, _speedHook * Time.deltaTime);

                if (Vector2.Distance(_hookGO.transform.position, _startPos.position) <= 0.1f)
                {
                    _hook.OnStartHook();
                    _isTargetPath = true;
                    _isHook = false;

                    if (_fish != null)
                    {
                        CompRoot.Instanse.Coins += _baitManager.GetReward(IndexBait, _fish.gameObject);
                        CoinsAnimation(false, _baitManager.GetReward(IndexBait, _fish.gameObject));
                        Destroy(_fish.gameObject);
                        _fish = null;
                    }

                    if (_symbol != null)
                    {
                        CompRoot.Instanse.Coins += _baitManager.GetReward(IndexBait, _symbol.gameObject);
                        CoinsAnimation(false, _baitManager.GetReward(IndexBait, _symbol.gameObject));
                        Destroy(_symbol.gameObject);
                        _symbol = null;
                    }
                }
            }
        }
        else
        {
            _hookGO.transform.position = _startPos.position;
        }

        UpdateRopePositions();
        MoveBoat();
    }

    private void MoveBoat() 
    {
        if (_boat.transform.localScale.x == 1)
        {
            _boat.transform.Translate(-_speedBoat * Time.deltaTime, 0, 0);

            if (_boat.transform.position.x <= -_clapmX) 
            {
                _boat.transform.localScale = new Vector3(-_boat.transform.localScale.x, _boat.transform.localScale.y, _boat.transform.localScale.z);
            }
        }
        else
        {
            _boat.transform.Translate(_speedBoat * Time.deltaTime, 0, 0);

            if (_boat.transform.position.x >= _clapmX)
            {
                _boat.transform.localScale = new Vector3(-_boat.transform.localScale.x, _boat.transform.localScale.y, _boat.transform.localScale.z);
            }
        }               
    }

    private void UpdateRopePositions() 
    {        
        _rope.SetPosition(0, new Vector3(_startPos.position.x, _startPos.position.y, -1));
        _rope.SetPosition(1, new Vector3(_hookGO.transform.position.x, _hookGO.transform.position.y, -1));   
    }
}
