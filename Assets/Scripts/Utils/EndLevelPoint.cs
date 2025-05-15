using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class EndLevelPoint : MonoBehaviour
    {

        public string SceneName = "Scenes/Nuevo Menu";
        private Rigidbody rigidbody;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            EDebug.Log($"<color=red>{nameof(OnTriggerEnter)}</color>", this);
            if (other.CompareTag("Player"))
            {
                LoadingManager.Instance.LoadSceneByName(SceneName);
            }
            else
            {
                EDebug.Log($"<color=red> Tag =|{other.tag}|</color>", this);
            }

        }

        private void OnCollisionEnter(Collision collision)
        {
            EDebug.Log($"<color=red>{nameof(OnCollisionEnter)}</color>", this);
            if (collision.gameObject.CompareTag("Player"))
            {
                LoadingManager.Instance.LoadSceneByName(SceneName);
            }

        }

    }
}


