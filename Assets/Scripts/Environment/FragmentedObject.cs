using System.Collections.Generic;
using UnityEngine;

public class FragmentedObject : MonoBehaviour
{
    private List<Rigidbody> _rigidbodies = new List<Rigidbody>();
    private Vector3[] _initialPositions;
    private Quaternion[] _initialRotations;
    private Transform[] _pieces;
    private float _lifeTime = 3f;
    private GameObject _prefabKey;

    void Awake()
    {
        _pieces = GetComponentsInChildren<Transform>();
        _initialPositions = new Vector3[_pieces.Length];
        _initialRotations = new Quaternion[_pieces.Length];

        // Store initial positions and rotations of the pieces
        for(int i=0; i < _pieces.Length; i++)
        {
            _initialPositions[i] = _pieces[i].localPosition;
            _initialRotations[i] = _pieces[i].localRotation;
        }

        _rigidbodies.AddRange(GetComponentsInChildren<Rigidbody>());
    }

    public void SetPoolKey(GameObject prefabKey)
    {
        _prefabKey = prefabKey;
    }

    public void Activate(Vector3 position, Quaternion rotation, Vector3 hitPoint, Vector3 hitForce, float explosionForce, float explosionRadius, float upward, float torque)
    {
        transform.position = position;
        transform.rotation = rotation;
        gameObject.SetActive(true);

        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero; 

            rb.AddExplosionForce(explosionForce + hitForce.magnitude, hitPoint, explosionRadius, upward, ForceMode.Impulse);
            rb.AddTorque(Random.onUnitSphere * torque, ForceMode.Impulse);
        }

        CancelInvoke();
        Invoke(nameof(ReturnToPool), _lifeTime);
    }

    void ReturnToPool()
    {
        ResetPieces();
        FragmentPool.Instance.Return(_prefabKey, this);
    }

    void ResetPieces()
    {
        for (int i=0; i<_pieces.Length; i++)
        {
            _pieces[i].localPosition = _initialPositions[i];
            _pieces[i].localRotation = _initialRotations[i];
        }

        foreach (Rigidbody rb in _rigidbodies)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
