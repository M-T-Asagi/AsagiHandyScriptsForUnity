using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsagiHandyScripts
{
    public class TimeScaledProcessionMonoBehaviour : MonoBehaviour
    {
        [SerializeField]
        [Range(0.0f, 10.0f)]
        protected float timeScale = 1.0f;

        [SerializeField]
        [Range(0.0f, 10.0f)]
        protected float Resolution = 1.0f;

        private float lastProcessedTime = 0;

        // Use this for initialization
        void Start()
        {
            lastProcessedTime = CalculatedTimeWithResolution();
        }

        // Update is called once per frame
        private void Update()
        {
            float nowTime = CalculatedTimeWithResolution();
            if (nowTime != lastProcessedTime)
            {
                UpdateProcess(nowTime);
                lastProcessedTime = nowTime;
            }
        }

        private float CalculatedTimeWithResolution()
        {
            float nowTime = Time.time;
            if (Time.timeScale == 0)
                nowTime = lastProcessedTime;
            else
                nowTime *= timeScale;

            if (Resolution != 0)
                nowTime = Mathf.Floor(nowTime / Resolution) * Resolution;

            return nowTime;
        }

        virtual protected void UpdateProcess(float time) { }
    }
}