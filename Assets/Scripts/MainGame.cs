using UnityEngine;

public class MainGame : MonoBehaviour
{
    public static string Url;
    [SerializeField] private RectTransform _idealRt;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.AutoRotation;
    }
    private UniWebView _uWw;
    private void Start()
    {
        _uWw = gameObject.AddComponent<UniWebView>();
        _uWw.OnShouldClose += OnShouldClose;
        _uWw.ReferenceRectTransform = _idealRt;
        _uWw.SetAllowBackForwardNavigationGestures(true);
        _uWw.Load(MainGame.Url);
        _uWw.Show(true);
    }

    private bool OnShouldClose(UniWebView webView)
    {
        return false;
    }

    public void ChangeOrintir()
    {
        _uWw.UpdateFrame();
    }
}
