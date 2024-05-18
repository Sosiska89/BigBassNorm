using System.Collections;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private Fish[] _fishs;
    [SerializeField] private Symbol[] _symbols;
    [SerializeField] private float _minSpeed, _maxSpeed;

    private void Start()
    {
        StartCoroutine(SpawnFishRoutine());
    }

    private IEnumerator SpawnFishRoutine() 
    {
        while (true)
        {
            Transform _spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

            int randFishSymb = Random.Range(0, 10);

            if (randFishSymb > 0)
            {
                Fish fish = Instantiate(_fishs[Random.Range(0, _fishs.Length)], _spawnPoint.position, Quaternion.identity);

                if (_spawnPoint.position.x > 0)
                {
                    fish.StartRight(Random.Range(_minSpeed, _maxSpeed));
                }
                else
                {
                    fish.StartLeft(Random.Range(_minSpeed, _maxSpeed));
                }
            }
            else 
            {
                Symbol symbol = Instantiate(_symbols[Random.Range(0, _symbols.Length)], _spawnPoint.position, Quaternion.identity);
                symbol.InitializeSymbol(_spawnPoint);
            }

            yield return new WaitForSeconds(1f);
        }
    }
}