﻿using System;
using UnityEngine;

[RequireComponent(typeof(Animation))]
public class StandardButtonAnimator : MonoBehaviour
{
    public AnimationClip IdleAnimation;
    public AnimationClip OnDownAnimation;
    public AnimationClip PressedAnimation;
    public AnimationClip OnUpAnimation;

    public float IdleAnimationTimeScale = 1;
    public float OnDownAnimationTimeScale = 1;
    public float PressedAnimationTimeScale = 1;
    public float OnUpAnimationTimeScale = 1;

    private Animation animation;

    public enum EAnimType
    {
        Idle,
        OnDown,
        Pressed,
        OnUp
    }

    private void Awake()
    {
        animation = GetComponent<Animation>();
    }

    private void SetAnimationSpeed(AnimationClip clip, float speed)
    {
        if (!clip)
        {
            return;
        }

        animation[clip.name].speed = speed;
    }

    public void PlayAnimation(EAnimType animType)
    {
        var clip = GetClip(animType);
        var speed = GetSpeed(animType);

        if (!clip)
        {
            return;
        }

        if (animation.isPlaying)
        {
            animation.Stop();
        }

        SetClipToStart(animType);
        SetAnimationSpeed(clip, speed);
        animation.Play(clip.name);
    }

    public void QueueAnimation(EAnimType animType)
    {
        var clip = GetClip(animType);
        var speed = GetSpeed(animType);

        if (!clip)
        {
            return;
        }

        SetClipToStart(animType);
        SetAnimationSpeed(clip, speed);
        animation.PlayQueued(clip.name);
    }

    private AnimationClip GetClip(EAnimType animType)
    {
        switch (animType)
        {
            case EAnimType.Idle:
                return IdleAnimation;
            case EAnimType.OnDown:
                return OnDownAnimation;
            case EAnimType.Pressed:
                return PressedAnimation;
            case EAnimType.OnUp:
                return OnUpAnimation;
            default:
                throw new ArgumentOutOfRangeException(nameof(animType), animType, null);
        }
    }

    private float GetSpeed(EAnimType animType)
    {
        switch (animType)
        {
            case EAnimType.Idle:
                return IdleAnimationTimeScale;
            case EAnimType.OnDown:
                return OnDownAnimationTimeScale;
            case EAnimType.Pressed:
                return PressedAnimationTimeScale;
            case EAnimType.OnUp:
                return OnUpAnimationTimeScale;
            default:
                throw new ArgumentOutOfRangeException(nameof(animType), animType, null);
        }
    }

    public void SetClipToStart(EAnimType animType)
    {
        var clip = GetClip(animType);
        if (!clip)
        {
            return;
        }

        animation[clip.name].normalizedTime = animation[clip.name].speed >= 0f ? 0f : 1f;
    }

    public void SetClipToEnd(EAnimType animType)
    {
        var clip = GetClip(animType);
        if (!clip)
        {
            return;
        }

        animation[clip.name].normalizedTime = animation[clip.name].speed >= 0f ? 1f : 0f;
    }
}