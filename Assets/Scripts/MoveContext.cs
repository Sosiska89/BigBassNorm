using UnityEngine;

public class MoveContext : MonoBehaviour
{
    [SerializeField] private bool _isHotizontal;
    [SerializeField] private float _minX, _maxX;
    [SerializeField] private float _horizontalSpeed;
   
    [SerializeField] private bool _isVertical;
    [SerializeField] private float _minY, _maxY;
    [SerializeField] private float _verticalSpeed;

    [SerializeField] private float _difference;
    [SerializeField] private GameObject[] _moveObjects;

    private void Update()
    {
        if (_isHotizontal) 
        {
            foreach (var moveObj in _moveObjects)
            {
                moveObj.transform.Translate(_horizontalSpeed * Time.deltaTime, 0, 0);

                if (moveObj.transform.position.x <= _minX) 
                {
                    GameObject lastMoveObj = GetLastX(_moveObjects);
                    moveObj.transform.position = new Vector3( lastMoveObj.transform.position.x + _difference, moveObj.transform.position.y, moveObj.transform.position.z);
                }
            }  
        }

        if (_isVertical) 
        {
            foreach (var moveObj in _moveObjects)
            {
                moveObj.transform.Translate(0, _verticalSpeed * Time.deltaTime, 0);

                if (moveObj.transform.position.y >= _maxY)
                {
                    GameObject lastMoveObj = GetLastY(_moveObjects);
                    moveObj.transform.position = new Vector3(moveObj.transform.position.x, lastMoveObj.transform.position.y - _difference, moveObj.transform.position.z);
                }
            }
        }
    }

    private GameObject GetLastX(GameObject[] moveObjects)
    {
        GameObject lastX = moveObjects[0];

        foreach (var moveObj in moveObjects) 
        {
            if (moveObj.transform.position.x >= lastX.transform.position.x) 
            {
                lastX = moveObj;   
            }
        }

        return lastX;
    }

    private GameObject GetLastY(GameObject[] moveObjects)
    {
        GameObject lastY = moveObjects[0];

        foreach (var moveObj in moveObjects)
        {
            if (moveObj.transform.position.y <= lastY.transform.position.y)
            {
                lastY = moveObj;
            }
        }

        return lastY;
    }
}
