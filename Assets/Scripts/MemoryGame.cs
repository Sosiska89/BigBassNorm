using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MemoryGame : MonoBehaviour
{
    [SerializeField] private MemoryCell[] _memoryCells;
    [SerializeField] private TextMeshProUGUI _timer;
    [SerializeField] private int _maxIndexSymbols;
    [SerializeField] private int _startTimer;
    [SerializeField] private RectTransform _startBtn;

    [HideInInspector] public bool IsGameStart = false;

    [SerializeField] private GameObject _GOPanel;
    [SerializeField] private RectTransform _goRt;
    [SerializeField] private Image _fadeGO;
    [SerializeField] private Color _fadeColor;
    [SerializeField] private TextMeshProUGUI _couplesText;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _wrongs;

    private Coroutine startRoutine = null;

    private int timerCounter = 0;

    private MemoryCell _memoryCell1;
    private MemoryCell _memoryCell2;

    private int _couples = 0;
    private int _hp = 4;

    [SerializeField] private Image[] _hearts;
    [SerializeField] private Sprite _fullHeart;
    [SerializeField] private Sprite _emptyHeart;
    [SerializeField] private GameObject _winTitle;
    [SerializeField] private GameObject _loseTitle;

    [SerializeField] private RectTransform _mainGameRT;
    [SerializeField] private GameObject _exitBtn;

    public void OnClickMenu() 
    {
        CompRoot.Instanse.FadeToScene("Menu");
    }

    public void OnClickRestart() 
    {
        CompRoot.Instanse.FadeToScene("MemoryGame");
    }


    public void OnClickExit() 
    {
        CompRoot.Instanse.FadeToScene("Menu");
    }

    private void SetHp(int hp) 
    {
        for (int i = 0; i < _hearts.Length; i++)
        {
            if (i+1 <= hp)
            {
                _hearts[i].sprite = _fullHeart;
            }
            else 
            {
                _hearts[i].sprite = _emptyHeart;
            }
        }
    }

    private void Update()
    {
        _couplesText.text = _couples.ToString();
        _wrongs.text = (4 - _hp).ToString();
    }

    public void OnOpenCell(MemoryCell memoryCell) 
    {
        if (IsGameStart) 
        {
            if (_memoryCell1 == null)
            {
                _memoryCell1 = memoryCell;
                return;
            }

            if (_memoryCell2 == null)
            {
                _memoryCell2 = memoryCell;

                if (_memoryCell1.IndexSymbol == _memoryCell2.IndexSymbol && (!_memoryCell1.IsEmpty && !_memoryCell2.IsEmpty))
                {
                    _couples++;

                    if (_couples == 4)
                    {
                        ShowGameOverPanel(true);
                    }

                    _memoryCell1 = null;
                    _memoryCell2 = null;
                }
                else
                {
                    _hp--;

                    SetHp(_hp);

                    if (_hp == 0)
                    {
                        ShowGameOverPanel(false);
                    }

                    _memoryCell1.HideCell();
                    _memoryCell2.HideCell();

                    _memoryCell1 = null;
                    _memoryCell2 = null;
                }
            }
        }
    }

    public void OnClickStart() 
    {
        if (startRoutine == null) 
        {
            _startBtn.DOAnchorPosY(-180, 0.3f);
            startRoutine = StartCoroutine(StartTimerRoutine());
        }
    }

    private IEnumerator StartTimerRoutine() 
    {
        IsGameStart = false;
        InitializeCellsPlane();

        foreach (var cell in _memoryCells) 
        {
            cell.OpenCell();
        }

        timerCounter = 20;
        _timer.text = timerCounter.ToString();

        for (int i = 0; i < _startTimer; i++)
        {
            timerCounter--;

            if (timerCounter <= 5) 
            {
                _timer.color = Color.red;
            }

            if (timerCounter < 10)
            {
                _timer.text = "00:0" + timerCounter.ToString();
            }
            else 
            {
                _timer.text = "00:" + timerCounter.ToString();
            }

            yield return new WaitForSeconds(1);
        }

        IsGameStart = true;

        timerCounter = 0;
        
        _timer.text = "00:00";
        _timer.color = Color.white;
        
        foreach (var cell in _memoryCells)
        {
            cell.HideCell();
        }

        int fullTime = 60 * 5;

        while (timerCounter < fullTime) 
        {
            timerCounter++;
            
            yield return new WaitForSeconds(1); 
            
            TimeSpan timeSpan = TimeSpan.FromSeconds(fullTime - timerCounter);
            _timer.text = timeSpan.ToString(@"mm\:ss");

            TimeSpan timeSpanPanel = TimeSpan.FromSeconds(timerCounter);
            _timeText.text = timeSpanPanel.ToString(@"mm\:ss");
        }

        if (timerCounter >= fullTime) 
        {
            ShowGameOverPanel(false);
        }
    }

    private void ShowGameOverPanel(bool isWin) 
    {
        if (isWin)
        {
            _winTitle.SetActive(true);
            _loseTitle.SetActive(false);

            if (CompRoot.Instanse.BestScoreMemory == -1)
            {
                CompRoot.Instanse.BestScoreMemory = timerCounter;
            }
            else 
            {
                if (timerCounter < CompRoot.Instanse.BestScoreMemory) 
                {
                    CompRoot.Instanse.BestScoreMemory = timerCounter;
                }
            }
        }
        else 
        {
            _winTitle.SetActive(false);
            _loseTitle.SetActive(true);
        }

        _exitBtn.SetActive(false);
        _GOPanel.gameObject.SetActive(true);
        _goRt.DOAnchorPosY(0, 0.3f);
        _fadeGO.DOColor(_fadeColor, 0.3f);
        StopCoroutine(startRoutine);
        startRoutine = null;

        _mainGameRT.DOAnchorPosY(-1750, 0.3f);
    }

    private void InitializeCellsPlane() 
    {
        List<MemoryCell> cells = _memoryCells.ToList();

        int emptyCellIndex = Random.Range(0, cells.Count);
        cells[emptyCellIndex].SetEmpty(true);
        cells.Remove(cells[emptyCellIndex]);
        
        HashSet<int> indexSymbols = new HashSet<int>();
        while (indexSymbols.Count < 4)
            indexSymbols.Add(Random.Range(0, _maxIndexSymbols));
        int[] indexSymbolsArray = indexSymbols.ToArray();

        for (int i = 0; i < indexSymbolsArray.Length; i++) 
        {
            for (int j = 0; j < 2; j++)
            {
                int indexCell = Random.Range(0, cells.Count);
                cells[indexCell].SetCell(indexSymbolsArray[i]);
                cells.Remove(cells[indexCell]);
            }             
        }
    }
}
