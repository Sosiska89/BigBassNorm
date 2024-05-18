using UnityEngine;

public class RTMainGame : MonoBehaviour
{
    [SerializeField] private MainGame _mainGame;

    private void OnRectTransformDimensionsChange()
    {
        _mainGame.ChangeOrintir();
    }
}
