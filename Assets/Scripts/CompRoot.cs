using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CompRoot : MonoBehaviour
{
    [SerializeField] private Image _fadeImg;
    
    [HideInInspector] public bool IsSound;
    [HideInInspector] public int BestScoreFish;
    [HideInInspector] public int BestScoreMemory;

    [HideInInspector] public static CompRoot Instanse;
    private float _fadeTime = 0.5f;
    private Coroutine _fadeRoutine = null;

    [HideInInspector] public bool IsFirstOpenMenu = true;

    private void Awake()
    {
        if (Instanse == null)
        {
            Instanse = this;
            DontDestroyOnLoad(gameObject);
            IsSound = Convert.ToBoolean(PlayerPrefs.GetInt("IsSound", 1));
            BestScoreFish = PlayerPrefs.GetInt("BestScoreFish", 0);
            BestScoreMemory = PlayerPrefs.GetInt("BestScoreMemory", -1);
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    public void FadeToScene(string sceneName) 
    {
        if (_fadeRoutine == null) 
        {
            _fadeRoutine = StartCoroutine(FadeSceneRoutine(sceneName));
        }
    }

    private IEnumerator FadeSceneRoutine(string scName) 
    {
        _fadeImg.gameObject.SetActive(true);
        _fadeImg.color = Color.clear;

        float timer = 0;

        while (timer < _fadeTime)
        {
            _fadeImg.color = Color.Lerp(Color.clear, Color.black, timer/_fadeTime);
            timer += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(scName);

        timer = 0;
        while (timer < _fadeTime)
        {
            _fadeImg.color = Color.Lerp(Color.black, Color.clear, timer / _fadeTime); 
            timer += Time.deltaTime;
            yield return null;
        }

        _fadeImg.gameObject.SetActive(false);
        _fadeRoutine = null;
    }

    private void OnApplicationFocus(bool focus)
    {
        PlayerPrefs.SetInt("IsSound", Convert.ToInt32(IsSound));
        PlayerPrefs.SetInt("BestScoreFish", BestScoreFish);
        PlayerPrefs.SetInt("BestScoreMemory", BestScoreMemory);
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("IsSound", Convert.ToInt32(IsSound));
        PlayerPrefs.SetInt("BestScoreFish", BestScoreFish);
        PlayerPrefs.SetInt("BestScoreMemory", BestScoreMemory);
        PlayerPrefs.Save();
    }

    private void OnApplicationPause(bool pause)
    {
        PlayerPrefs.SetInt("IsSound", Convert.ToInt32(IsSound));
        PlayerPrefs.SetInt("BestScoreFish", BestScoreFish);
        PlayerPrefs.SetInt("BestScoreMemory", BestScoreMemory);
        PlayerPrefs.Save();
    }
}
