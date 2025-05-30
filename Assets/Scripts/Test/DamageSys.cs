using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageSys : MonoBehaviour
{
    public static DamageSys Instance { get; private set; }
    [Header("Object Dealer of Damage")]
    [SerializeField] private GameObject _damageDealer = default;
    public bool _isDead = default;

    public GameObject _playerObj = default;
    public Camera _camera;
    public bool _isPlayer = false;
    //public ProjectSaga.SFXController sfxController;

    [Header("Life System")]
    [SerializeField]
    public int _life = 50;

    private void Awake()
    {
        Instance = this;
        if (Instance != this)
        {
            Destroy(gameObject);
        }

        _camera = Camera.main;
    }

    public void RemovingLife(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            _life--;
            if (_life <= 0 && !_isPlayer)
            {
                this.gameObject.SetActive(false);
            }
            if(_life <= 0 && _isPlayer)
            {
                _camera.gameObject.transform.parent = null;
                _isDead = true;
                _playerObj.SetActive(false);

            }
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (_damageDealer.CompareTag("DamageDealer"))
        {
            RemovingLife(1);
            Debug.Log("Enemy has been hit");
        }
    }
}

