using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    using UnityEngine.UI;
    /// <summary>
    /// A slider but the values are represented by blocks
    /// </summary>
    public sealed class BlockySlider : MonoBehaviour
    {
        [SerializeField] Color filledColor = Color.white;
        [SerializeField] Color emptyColor = Color.black;

        [Tooltip("This is for the gameObject that contains all the Images that will be manipulated")]
        [SerializeField] Transform blockContainer;

        [Tooltip("This is what the slider will use to visually represent the values")]
        [SerializeField] Image[] blocks;

        [field: Header("Valores para controlar el BlockySlider")]
        [Tooltip("This is the percentage represented by the slider ")]
        [field: SerializeField, Range(0.0f, 1.0f)] public float percent { get; private set; } = 0.5f;

        [Tooltip("This is the value represented by the slider ")]
        public int currentTurnedOnBlockCount { get; private set; } = 0;
        public int desiredTurnedOnBlockCount { get; private set; } = 0;

        [field: Header("Evento para saber cuando cambia la variable percent")]
        public Action<float> OnBlockChangeAction;


        void Start()
        {
            Debug.Assert(blockContainer != null, $"Asignar el objecto que contiene las {nameof(UnityEngine.UI.Image)} que necesita este script", this);

            blocks = blockContainer.GetComponentsInChildren<Image>();

            Debug.Assert(blocks.Length > 1);
            currentTurnedOnBlockCount = -1337;// blocks.Length / 2;
            desiredTurnedOnBlockCount = blocks.Length / 2;
            UpdateBlocks();
        }

        private void FixedUpdate()
        {
            if (currentTurnedOnBlockCount != desiredTurnedOnBlockCount)
            {
                UpdateBlocks();
                OnBlockChangeAction?.Invoke(percent);
            }

        }

        private void turnOnBlocks()
        {
            int temp = currentTurnedOnBlockCount;
            for (int i = 0; i < blocks.Length; ++i)
            {
                if (temp > 0)
                {
                    blocks[i].color = filledColor;
                }
                else
                {
                    blocks[i].color = emptyColor;
                }

                temp -= 1;
            }


        }

        private void calculatePercent()
        {
            if (currentTurnedOnBlockCount == 0)
            {
                percent = 0.0f;
                return;
            }
            percent = blocks.Length / (float)currentTurnedOnBlockCount;
        }

        public void increaseBlocks()
        {
            desiredTurnedOnBlockCount += 1;
            if (desiredTurnedOnBlockCount > blocks.Length)
            {
                desiredTurnedOnBlockCount = blocks.Length;
            }
        }

        public void decreaseBlocks()
        {
            desiredTurnedOnBlockCount -= 1;
            if (desiredTurnedOnBlockCount < 0)
            {
                desiredTurnedOnBlockCount = 0;
            }
        }

        public void setBlocks(int _blocks)
        {
            desiredTurnedOnBlockCount = Math.Clamp(_blocks, 0, blocks.Length - 1);
        }

        private void UpdateBlocks()
        {
            currentTurnedOnBlockCount = desiredTurnedOnBlockCount;
            turnOnBlocks();
            calculatePercent();
        }

        public void Subscribe(Action<float> callback)
        {
            OnBlockChangeAction += callback;
        }

        public void UnSubscribe(Action<float> callback)
        {
            OnBlockChangeAction -= callback;
        }


    }

}

