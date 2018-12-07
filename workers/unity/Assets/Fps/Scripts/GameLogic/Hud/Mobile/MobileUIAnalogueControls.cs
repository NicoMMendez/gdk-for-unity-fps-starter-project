﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class MobileUIAnalogueControls : MonoBehaviour
{
    private bool areMoving;
    private bool areLooking;

    public Vector2 MoveDelta { get; private set; }
    public Vector2 MoveTotal { get; private set; }
    private int moveFingerId;
    private Vector2 moveStartPosition;
    private Vector2 moveLastPosition;

    public Vector2 LookDelta { get; private set; }
    public Vector2 LookTotal { get; private set; }
    private int lookFingerId;
    private Vector2 lookStartPosition;
    private Vector2 lookLastPosition;

    private readonly List<int> blacklistedFingerIds = new List<int>();

#if UNITY_EDITOR
    public bool DrawDebugInfo;
#endif
    private string debugGuiText;

    public void AddBlacklistedFingerId(int fingerId)
    {
        blacklistedFingerIds.Add(fingerId);
    }

    private void Update()
    {
        debugGuiText = "";

        CheckForStoppedTouches();
        CheckForStartedTouches();

        CheckMoveTouch();
        CheckLookTouch();

        if (areMoving)
        {
            debugGuiText +=
                $"\n\n" +
                $"MOVE id: {moveFingerId} \t" +
                $"phase: {GetTouchFromFingerId(moveFingerId).phase} \t" +
                $"pos: {GetTouchFromFingerId(moveFingerId).position}\n";
        }

        if (areLooking)
        {
            debugGuiText +=
                $"\n\n" +
                $"LOOK id: {lookFingerId} \t" +
                $"phase: {GetTouchFromFingerId(lookFingerId).phase} \t" +
                $"pos: {GetTouchFromFingerId(lookFingerId).position}\n";
        }
    }


    private void CheckForStoppedTouches()
    {
        if (areMoving)
        {
            if (!TouchExists(moveFingerId))
            {
                StopTrackingMove();
            }
            else
            {
                var phase = GetTouchFromFingerId(moveFingerId).phase;
                if (phase == TouchPhase.Ended || phase == TouchPhase.Canceled)
                {
                    StopTrackingMove();
                }
            }
        }

        if (areLooking)
        {
            if (!TouchExists(lookFingerId))
            {
                StopTrackingLook();
            }
            else
            {
                var phase = GetTouchFromFingerId(lookFingerId).phase;
                if (phase == TouchPhase.Ended || phase == TouchPhase.Canceled)
                {
                    StopTrackingLook();
                }
            }
        }
    }

    private void CheckForStartedTouches()
    {
        for (var i = 0; i < Input.touchCount; i++)
        {
            var touch = Input.touches[i];

            debugGuiText += $"id: {touch.fingerId} \tphase: {touch.phase} \t pos: {touch.position}\n";

            if (touch.phase == TouchPhase.Began)
            {
                if (blacklistedFingerIds.Contains(touch.fingerId))
                {
                    continue;
                }

                var leftOfScreen = touch.position.x < Screen.width / 2f;
                if (leftOfScreen)
                {
                    if (!areMoving)
                    {
                        StartTrackingMove(touch);
                    }
                    else if (!areLooking)
                    {
                        StartTrackingLook(touch);
                    }
                }
                else
                {
                    if (!areLooking)
                    {
                        StartTrackingLook(touch);
                    }
                    else if (!areMoving)
                    {
                        StartTrackingMove(touch);
                    }
                }
            }
        }

        blacklistedFingerIds.Clear();
    }


    private void CheckMoveTouch()
    {
        if (!areMoving)
        {
            return;
        }

        var position = GetTouchFromFingerId(moveFingerId).position;
        MoveTotal = position - moveStartPosition;
        MoveDelta = position - moveLastPosition;
        moveLastPosition = position;
    }

    private void CheckLookTouch()
    {
        if (!areLooking)
        {
            return;
        }

        var position = GetTouchFromFingerId(lookFingerId).position;
        LookTotal = position - lookStartPosition;
        LookDelta = position - lookLastPosition;
        lookLastPosition = position;
    }


    private void StartTrackingMove(Touch touch)
    {
        areMoving = true;
        moveFingerId = touch.fingerId;
        moveStartPosition = touch.position;
        moveLastPosition = moveStartPosition;
    }

    private void StartTrackingLook(Touch touch)
    {
        areLooking = true;
        lookFingerId = touch.fingerId;
        lookStartPosition = touch.position;
        lookLastPosition = lookStartPosition;
    }


    private void StopTrackingMove()
    {
        areMoving = false;
        MoveDelta = Vector2.zero;
        MoveTotal = Vector2.zero;
    }

    private void StopTrackingLook()
    {
        areLooking = false;
        LookDelta = Vector2.zero;
        LookTotal = Vector2.zero;
    }


    private bool TouchExists(int fingerId)
    {
        for (var i = 0; i < Input.touchCount; i++)
        {
            if (Input.touches[i].fingerId == fingerId)
            {
                return true;
            }
        }

        return false;
    }

    private Touch GetTouchFromFingerId(int fingerId)
    {
        for (var i = 0; i < Input.touchCount; i++)
        {
            if (Input.touches[i].fingerId == fingerId)
            {
                return Input.touches[i];
            }
        }

        throw NoTouchFoundException(fingerId);
    }

    private Exception NoTouchFoundException(int fingerId)
    {
        return new ArgumentException($"No touch found with fingerId {fingerId}");
    }

#if UNITY_EDITOR

    private void OnGUI()
    {
        if (!DrawDebugInfo)
        {
            return;
        }

        GUI.Label(new Rect(0, 0, Screen.width, Screen.height), debugGuiText);

        var wt = UnityEditor.EditorGUIUtility.whiteTexture;

        if (areLooking)
        {
            GUI.color = Color.green;
            GUI.DrawTexture(new Rect(lookStartPosition.x, Screen.height - lookStartPosition.y, 30, 30), wt);

            var lookAbsolute = lookStartPosition + LookTotal;
            GUI.DrawTexture(new Rect(lookAbsolute.x, Screen.height - lookAbsolute.y, 20, 20), wt);
        }

        if (areMoving)
        {
            GUI.color = Color.blue;
            GUI.DrawTexture(new Rect(moveStartPosition.x, Screen.height - moveStartPosition.y, 30, 30), wt);

            var moveAbsolute = moveStartPosition + MoveTotal;
            GUI.DrawTexture(new Rect(moveAbsolute.x, Screen.height - moveAbsolute.y, 20, 20), wt);
        }
    }

#endif
}