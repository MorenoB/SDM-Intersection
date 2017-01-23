using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(WaypointAgent))]
public class AnimationActivator : MonoBehaviour
{

    public Animation animationComponent;

    [Header("Settable animations.")]
    public List<AnimationClip> idleAnimations = new List<AnimationClip>();
    public List<AnimationClip> movingAnimations = new List<AnimationClip>();

    private WaypointAgent agent;
    private WaypointAgent Agent
    {
        get
        {
            if (agent == null)
                agent = GetComponent<WaypointAgent>();


            return agent;


        }
    }

    private bool idleAnimationPlaying;
    private bool movingAnimationPlaying;

    private void OnDisable()
    {
        if (Agent != null)
            Agent.OnWaypointsystemActivatedChanged -= OnWaypointsystemActivatedChanged;
    }

    private void OnEnable()
    {
        Agent.OnWaypointsystemActivatedChanged += OnWaypointsystemActivatedChanged;

        idleAnimationPlaying = false;
        movingAnimationPlaying = false;

        animationComponent.Stop();
    }

    private void OnWaypointsystemActivatedChanged()
    {
        if (!Agent.WaypointSystemActivated)
        {
            if (idleAnimationPlaying)
            {
                animationComponent.Stop();
                idleAnimationPlaying = false;
            }

            animationComponent.clip = GetRandomMovingAnimation();
            animationComponent.Play();

            movingAnimationPlaying = true;
            return;
        }

        if (movingAnimationPlaying)
        {
            animationComponent.Stop();
            movingAnimationPlaying = false;
        }

        animationComponent.clip = GetRandomIdleAnimation();
        animationComponent.Play();

        idleAnimationPlaying = true;
    }

    private AnimationClip GetRandomIdleAnimation()
    {
        int randomIndex = Random.Range(0, idleAnimations.Count);

        if (idleAnimations.Count < 0)
            return null;

        return idleAnimations[randomIndex];
    }

    private AnimationClip GetRandomMovingAnimation()
    {
        int randomIndex = Random.Range(0, movingAnimations.Count);

        if (movingAnimations.Count < 0)
            return null;

        return movingAnimations[randomIndex];
    }
}
