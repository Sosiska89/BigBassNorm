using DG.Tweening;
using UnityEngine;
using UnityEngine.U2D;

public class Symbol : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    [SerializeField] private SpriteRenderer _circle;
    private bool _isInit = false;

    private Vector2 _startPosition;
    private Vector2 _endPosition;

    private float timer = 0;
    private float timePath = 10;

    private void Start()
    {
        _circle.transform.DOLocalRotate(new Vector3(0.0f, 0.0f, 360), 5.0f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear).SetRelative();
    }

    public void InitializeSymbol(Transform spawnPosition)
    {
        _startPosition = spawnPosition.position;
        _endPosition = spawnPosition.position;

        if (_startPosition.x > 0)
        {
            _endPosition.x -= 10;
        }
        else 
        {
            _endPosition.x += 10;
            gameObject.transform.localScale = new Vector3(-gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        }

        _isInit = true;
    }

    private void Update()
    {
        if (_isInit)
        {
            timer += Time.deltaTime;
            transform.position = WaveLerp(_startPosition, _endPosition, timer/timePath);                

            if (Mathf.Abs(transform.position.x) >= 4)
            {
                Destroy(gameObject);
            }
        }
    }

    private Vector2 WaveLerp(Vector2 a, Vector2 b, float time, float waveScale = 0.25f, float freq = 30f)
    {
        Vector2 result = Vector2.Lerp(a, b, time);

        Vector2 dir = (b - a).normalized;
        Vector2 leftNormal = result + new Vector2(-dir.y, dir.x) * waveScale;

        result = Vector2.LerpUnclamped(result, leftNormal, Mathf.Sin(time * freq));

        return result;
    }

    public void OffSymbolBehaviour()
    {
        _collider.enabled = false;
        _isInit = false;
    }
}
