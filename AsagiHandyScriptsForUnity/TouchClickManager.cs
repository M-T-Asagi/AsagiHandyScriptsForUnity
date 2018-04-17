using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public bool Tapped { get; private set; }
    public bool Swiped { get; private set; }
    public bool Held { get; private set; }
    public bool Cancelled { get; private set; }

    const float tapToSwipeMoves = 0.1f;

    float tapStartTime = 0;
    Vector2 tapStartPos = Vector2.zero;
    Vector2 tapNowPos = Vector2.zero;
    float lastTime;

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

        if (!Cancelled && mouseClick || isTouch)
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
            }
            else if (!Held && distanceTapPos < tapToSwipeMoves)
            {
                Held = true;
                if (TapHoldIn != null)
                    TapHoldIn(this, new TapHoldInEventArgs(timeNow, tapNowPos));
            }
            else if (!Held && !Swiped && distanceTapPos >= tapToSwipeMoves)
            {
                Swiped = true;
                if (SwipeStart != null)
                    SwipeStart(this, new SwipeStartEventArgs(timeNow, tapStartPos));
            }
            else if (Held && !Swiped && distanceTapPos >= tapToSwipeMoves)
            {
                Tapped = false;
                Swiped = false;
                Held = false;
                Cancelled = true;

                if (TapHoldCancel != null)
                    TapHoldCancel(this, new TapHoldCancelEventArgs(timeNow, tapNowPos));
            }
            else if ()

        }
        else if (Cancelled && !mouseClick && !isTouch)
        {

        }
        else if (tapping)
        {
            float force = (tapStartPos.x - tapNowPos.x) / Screen.width * rotationPower / (Time.time - tapStartTime);
            float absolutedForce = Mathf.Abs(force);
            float absolutedTorqueY = Mathf.Abs(rigidbody.angularVelocity.normalized.y);

            if (absolutedForce < 0.01f)
            {
                force = 0;
                rigidbody.angularVelocity = Vector3.zero;
            }
            else if (absolutedForce > absolutedTorqueY || absolutedForce > absolutedTorqueY)
            {
                rigidbody.angularVelocity = Vector3.zero;
            }

            rigidbody.AddTorque(new Vector3(0, force, 0), ForceMode.Impulse);
            tapping = false;
        }
    }

    Vector2 GetPointerPosition(bool isMouse)
    {
        if (isMouse)
            return Input.mousePosition;
        else
            return Input.GetTouch(0).position;
    }
}
