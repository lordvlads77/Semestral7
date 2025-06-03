using System.Collections;
using System.Collections.Generic;

namespace Credits
{
    using UnityEngine;
    using SaveSystem;
    using Input;
    using Utils;

    public sealed class CreditsSceneScript : MonoBehaviour
    {
        enum CreditsState
        {
            None = 0,
            loadCredits = 1,
            MoveCredits,
            MoveIndex,
        }


        public TextAsset[] creditsAssets;

        public CreditsTextBox leader;

        public CreditsTextBox follower;

        public int index = 0;

        public Transform startPoint;

        public Transform endPoint;

        [SerializeField] private CreditsState creditsState = CreditsState.None;

        public float lastDistanceFromEndPoint = float.MaxValue;

        private float distanceBetweenStartAndEndPoint = float.MaxValue;

        private float currentSpeed = 1.0f;
        public float desiredSpeed = 1.0f;

        public Actions inputReceiver;

        void Start()
        {
            creditsAssets = Resources.LoadAll<TextAsset>("Credits");

            EDebug.Assert(creditsAssets.Length > 0, $"We need {typeof(TextAsset)} in the folder Assets/Resources/Credits", this);
            EDebug.Assert(startPoint != null, $"need a {typeof(Transform)} for script to work", this);
            EDebug.Assert(endPoint != null, $"need a {typeof(Transform)} for script to work", this);

            inputReceiver = GameManager.Instance.gameObject.GetComponent<Actions>();
            creditsState = CreditsState.loadCredits;
        }

        void Update()
        {
            currentSpeed = desiredSpeed;
            leader.speed = currentSpeed;
            follower.speed = currentSpeed;

            if (inputReceiver.Jump)
            {
                LoadingManager.Instance.LoadSceneByName("Nuevo Menu");
            }


            if (creditsState == CreditsState.None) { return; }

            switch (creditsState)
            {
                case CreditsState.loadCredits:

                    leader.textMeshPro.text = creditsAssets[index].text;
                    leader.transform.position = startPoint.position;
                    Vector3 delta = (endPoint.position - startPoint.position);

                    if (index + 1 < creditsAssets.Length)
                    {
                        follower.textMeshPro.text = creditsAssets[index + 1].text;
                        follower.transform.position = startPoint.position + (Vector3.down * (delta.magnitude * 1.01f));
                        follower.move = true;
                    }
                    else
                    {
                        follower.textMeshPro.text = ""; 
                        follower.move = false;
                    }

                    distanceBetweenStartAndEndPoint = delta.magnitude;

                    leader.setDirection(delta.normalized);
                    leader.transform.position = startPoint.position;

                    follower.setDirection(delta.normalized);

                    leader.move = true;
                    creditsState = CreditsState.MoveCredits;
                    break;
                case CreditsState.MoveCredits:
                    Vector3 textBoxPos = leader.transform.position;
                    float distance = Vector3.Distance(textBoxPos, endPoint.position);

                    if (distance < leader.max_distance)
                    {
                        creditsState = CreditsState.MoveIndex;
                    }
                    // por si se pasa del end point
                    else if (distance > lastDistanceFromEndPoint)
                    {
                        creditsState = CreditsState.MoveIndex;
                    }
                    else if (distance < distanceBetweenStartAndEndPoint * 0.5f)

                        lastDistanceFromEndPoint = distance;

                    break;
                case CreditsState.MoveIndex:
                    int nextIndex = index + 1;
                    if (nextIndex >= creditsAssets.Length)
                    {
                        creditsState = CreditsState.None;
                        break;
                    }
                    index = nextIndex;
                    creditsState = CreditsState.loadCredits;
                    lastDistanceFromEndPoint = float.MaxValue;
                    break;
                default:
                    EDebug.LogError($"un-handled case |{creditsState}|", this);
                    break;
            }

        }

    }

}
