using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Renderer), typeof(Rigidbody))]
public class Cube : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    [SerializeField] private Rigidbody _rigidbody;

    private bool _hasTouched = false;
    private int _minTimeToDie = 2;
    private int _maxTimeToDie = 5;

    public event Action<Cube> Died;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        _hasTouched = false;
        _renderer.material.color = Color.white;

        if (_rigidbody != null)
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_hasTouched == false)
        {
			if (collision.gameObject.TryGetComponent<Platform>(out var platform))
			{
            	_hasTouched = true;
            	_renderer.material.color = Random.ColorHSV();
            	StartCoroutine(ReturnToPoolAfterDelay(Random.Range(_minTimeToDie, _maxTimeToDie)));
        	}
		}
    }

    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        Died?.Invoke(this);
    }
}
