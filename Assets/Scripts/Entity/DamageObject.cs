using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Character;
using HUD;
using Utils;

public class DamageObject : MonoBehaviour
{
    [SerializeField] private float damageAmount = 20f; // Cantidad de daño
    [SerializeField] private float armorPiercing = 0f; // Penetración de armadura opcional
    public float camShakeStr = 0.3f;
    public int camShakeFrames = 10;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            DoSomething();
    }

    private void DoSomething()
    {
        if (CamShaker.Instance != null)
            CamShaker.Instance.ShakeIt(camShakeStr, camShakeFrames);
        EDebug.Log("¡El jugador activó el trigger y la cámara está temblando!");
    }
}
