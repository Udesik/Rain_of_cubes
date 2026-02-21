using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Cube _cubePrefab;
    [SerializeField] private int _defaultCapacity = 20;
    [SerializeField] private int _maxSize = 100;

    [SerializeField] private float _spawnDelay = 0.5f;
    [SerializeField] private float _spawnRangeX = 10f;
    [SerializeField] private int _spawnHigh = 22;

    private ObjectPool<Cube> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Cube>(
            createFunc: () => Instantiate(_cubePrefab),
            actionOnGet: (cube) => OnGetFromPool(cube),
            actionOnRelease: (cube) => OnReleaseFromPool(cube),
            actionOnDestroy: (cube) => Destroy(cube.gameObject),
            collectionCheck: false,
            defaultCapacity: _defaultCapacity,
            maxSize: _maxSize
        );
    }

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        var wait = new WaitForSeconds(_spawnDelay);

        while (true)
        {
            _pool.Get();
            yield return wait;
        }
    }

    private void OnGetFromPool(Cube cube)
    {
        float x = Random.Range(-_spawnRangeX, _spawnRangeX);
        float z = Random.Range(-_spawnRangeX, _spawnRangeX);
        cube.transform.position = new Vector3(x, _spawnHigh, z);

        cube.Died += ReleaseCube;
        cube.gameObject.SetActive(true);
    }

    private void OnReleaseFromPool(Cube cube)
    {
        cube.Died -= ReleaseCube;
        cube.gameObject.SetActive(false);
    }

    private void ReleaseCube(Cube cube)
    {
        _pool.Release(cube);
    }
}
