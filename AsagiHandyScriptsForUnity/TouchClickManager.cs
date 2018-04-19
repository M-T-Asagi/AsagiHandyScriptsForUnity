using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AsagiHandyScripts
{
    public class TouchClickManager : MonoBehaviour
    {
        public class TapEventArgs : EventArgs
        {
            public float time;
            public Vector2 position;

            public TapEventArgs(float _time, Vector2 _pos)
            {
                time = _time;
                position = _pos;
            }
        }

        public event EventHandler<TapEventArgs> Tap;

        public class TapHoldEventArgs : EventArgs
        {
            public float time;
            public float holdTime;
            public Vector2 position;

            public TapHoldEventArgs(float _time, float _holdTime, Vector2 _pos)
            {
                time = _time;
                holdTime = _holdTime;
                position = _pos;
            }
        }

        public event EventHandler<TapHoldEventArgs> TapHold;

        public class TapHoldInEventArgs : EventArgs
        {
            public float time;
            public Vector2 position;

            public TapHoldInEventArgs(float _time, Vector2 _pos)
            {
                time = _time;
                position = _pos;
            }
        }

        public event EventHandler<TapHoldInEventArgs> TapHoldIn;

        public class TapHoldOutEventArgs : EventArgs
        {
            public float time;
            public Vector2 position;

            public TapHoldOutEventArgs(float _time, Vector2 _pos)
            {
                time = _time;
                position = _pos;
            }
        }

        public event EventHandler<TapHoldOutEventArgs> TapHoldOut;

        public class TapHoldCancelEventArgs : EventArgs
        {
            public float time;
            public Vector2 position;

            public TapHoldCancelEventArgs(float _time, Vector2 _pos)
            {
                time = _time;
                position = _pos;
            }
        }

        public event EventHandler<TapHoldCancelEventArgs> TapHoldCancel;

        public class TouchInEventArgs : EventArgs
        {
            public float time;
            public Vector2 position;

            public TouchInEventArgs(float _time, Vector2 _pos)
            {
                time = _time;
                position = _pos;
            }
        }

        public event EventHandler<TouchInEventArgs> TouchIn;

        public class TouchOutEventArgs : EventArgs
        {
            public float time;
            public Vector2 position;

            public TouchOutEventArgs(float _time, Vector2 _pos)
            {
                time = _time;
                position = _pos;
            }
        }

        public event EventHandler<TouchOutEventArgs> TouchOut;

        public class SwipeEventArgs : EventArgs
        {
            public float time;
            public Vector2 moves;

            public SwipeEventArgs(float _time, Vector2 _moves)
            {
                time = _time;
                moves = _moves;
            }
        }

        public event EventHandler<SwipeEventArgs> Swipe;

        public class SwipeStartEventArgs : EventArgs
        {
            public float time;
            public Vector2 position;

            public SwipeStartEventArgs(float _time, Vector2 _pos)
            {
                time = _time;
                position = _pos;
            }
        }

        public event EventHandler<SwipeStartEventArgs> SwipeStart;

        public class SwipeEndEventArgs : EventArgs
        {
            public float time;
            public Vector2 position;

            public SwipeEndEventArgs(float _time, Vector2 _pos)
            {
                time = _time;
                position = _pos;
            }
        }

        public event EventHandler<SwipeEndEventArgs> SwipeEnd;

        #region public Variables
        public bool Tapped { get; private set; }
        public bool Swiped { get; private set; }
        public bool Held { get; private set; }
        public bool Cancelled { get; private set; }
        #endregion

        #region serialized fields
        public float tapToSwipeMoves = 0.1f;
        public float timeHoldDetection = 1.0f;
        #endregion

        #region private variables
        float tapStartTime = 0;
        Vector2 tapStartPos = Vector2.zero;
        Vector2 tapNowPos = Vector2.zero;
        float lastTime;
        #endregion

        // Use this for initialization
        void Start()
        {
            lastTime = Time.time;
            Tapped = false;
            Swiped = false;
            Held = false;
            Cancelled = false;
        }

        // Update is called once per frame
        void Update()
        {
            bool mouseClick = Input.GetMouseButton(0);
            bool isTouch = Input.touchCount > 0;
            bool anyTap = (mouseClick || isTouch);

            if (!Cancelled && anyTap)
            {
                float timeNow = Time.time;
                tapNowPos = GetPointerPosition(mouseClick);

                float distanceTapPos = Vector2.Distance(tapNowPos, tapStartPos);

                if (!Tapped)
                {
                    tapStartPos = tapNowPos;
                    tapStartTime = Time.time;
                    Tapped = true;

                    if (TouchIn != null)
                        TouchIn(this, new TouchInEventArgs(timeNow, tapNowPos));

                    Debug.Log("TouchIn : TouchClickManager : " + timeNow);
                }
                else if (!Held && distanceTapPos < tapToSwipeMoves && timeNow - tapStartTime >= timeHoldDetection)
                {
                    Held = true;
                    if (TapHoldIn != null)
                        TapHoldIn(this, new TapHoldInEventArgs(timeNow, tapNowPos));

                    Debug.Log("TapHoldIn : TouchClickManager : " + timeNow);
                }
                else if (!Held && !Swiped && distanceTapPos >= tapToSwipeMoves)
                {
                    Swiped = true;
                    if (SwipeStart != null)
                        SwipeStart(this, new SwipeStartEventArgs(timeNow, tapStartPos));

                    Debug.Log("SwipeStart : TouchClickManager : " + timeNow);
                }
                else if (Held && !Swiped && distanceTapPos >= tapToSwipeMoves)
                {
                    Tapped = false;
                    Swiped = false;
                    Held = false;
                    Cancelled = true;

                    if (TapHoldCancel != null)
                        TapHoldCancel(this, new TapHoldCancelEventArgs(timeNow, tapNowPos));

                    Debug.Log("TapHoldCancelled : TouchClickManager : " + timeNow);
                }
            }
            else if (!anyTap && (Tapped || Swiped || Held) && !Cancelled)
            {
                float timeNow = Time.time;

                if (TouchOut != null)
                    TouchOut(this, new TouchOutEventArgs(timeNow, tapNowPos));

                Debug.Log("TouchOut : TouchClickManager : " + timeNow);

                if (Swiped)
                {
                    if (Swipe != null)
                        Swipe(this, new SwipeEventArgs(timeNow, tapNowPos - tapStartPos));

                    if (SwipeEnd != null)
                        SwipeEnd(this, new SwipeEndEventArgs(timeNow, tapNowPos));

                    Debug.Log("SwipeIsEnd : TouchClickManager : " + timeNow);
                }
                else if (Held)
                {
                    if (TapHold != null)
                        TapHold(this, new TapHoldEventArgs(timeNow, timeNow - tapStartTime, tapNowPos));

                    if (TapHoldOut != null)
                        TapHoldOut(this, new TapHoldOutEventArgs(timeNow, tapNowPos));

                    Debug.Log("TapHeld : TouchClickManager : " + timeNow);
                }
                else if (Tapped)
                {
                    if (Tap != null)
                        Tap(this, new TapEventArgs(timeNow, tapNowPos));

                    Debug.Log("Tapped : TouchClickManager : " + timeNow);
                }

                ResetFlag();
            }
            else if (Cancelled && !anyTap)
            {
                Cancelled = false;
            }
        }

        Vector2 GetPointerPosition(bool isMouse)
        {
            if (isMouse)
                return Input.mousePosition;
            else
                return Input.GetTouch(0).position;
        }

        void ResetFlag()
        {
            Tapped = false;
            Swiped = false;
            Held = false;
            Cancelled = false;
        }
    }
}