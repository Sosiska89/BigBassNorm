using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private Collider2D _collider;
    private bool _isInit = false;
    private float _speed;

    public void StartLeft(float speed) 
    {
        _speed = speed;
        gameObject.transform.localScale = new Vector3(-gameObject.transform.localScale.x, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
        _isInit = true;
    }

    public void StartRight(float speed) 
    {
        _speed = -speed;
        _isInit = true;
    }

    private void Update()
    {
        if (_isInit) 
        {
            transform.Translate(_speed * Time.deltaTime, 0,0);

            if (Mathf.Abs(transform.position.x) >= 4) 
            {
                Destroy(gameObject);   
            }
        }
    }

    public void OffFishBehaviour() 
    {
        _collider.enabled = false;
        _isInit = false;
    }
}
