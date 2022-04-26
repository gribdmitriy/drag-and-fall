﻿using Core.Bonuses;
using Core.Effects;
using Data.Core;
using ObjectPool;
using Sound;
using UI;
using UnityEngine;

namespace Core
{
    public class Platform : MonoBehaviour
    {
        private const string BREAK_ANIMATION_IDENTIFIER = "Break";
        
        private bool destroy;
        public int countTouches = 0;
        private TutorialUI tutorialUI;
        [HideInInspector] public TouchEffect touchEffect;
        [HideInInspector] public PlayerParticles playerParticles;
        public PatternData patternData;
        private Player player;
        private PlatformMover platformMover;
        public Segment[] segments;
        private GainScore gainScore;
        private SegmentContentPool segmentContentPool;
        private TubeMover tubeMover;
        private PlatformSound audioSource;
        private PlatformPool platformPool;
        private MeshCollider[] meshColliders;
        private Animator animator;
        
        public void Construct(PlatformPool platformPool, PlatformMover platformMover,
            Player player, BonusController bonusController, GainScore gainScore,
            SegmentContentPool segmentContentPool,
            TubeMover tubeMover, TutorialUI tutorialUI, PlatformSound audioSource)
        {
            meshColliders = new MeshCollider[12];
            this.platformPool = platformPool;
            this.segmentContentPool = segmentContentPool;
            this.platformMover = platformMover;
            this.gainScore = gainScore;
            this.tubeMover = tubeMover;
            this.tutorialUI = tutorialUI;
            this.audioSource = audioSource;
            this.player = player;
            
            for (var i = 1; i <= patternData.segmentsData.Length; i++)
            {
                segments[i - 1].Construct(platformMover, 
                    this, bonusController, segmentContentPool);
                meshColliders[i - 1] = segments[i - 1].GetComponent<MeshCollider>();
            }
            
            animator = GetComponent<Animator>();
        }

        public void Initialize(PatternData patternData)
        {
            this.patternData = patternData;
            
            for (var i = 1; i <= patternData.segmentsData.Length; i++)
                segments[i - 1].Initialize(patternData.segmentsData[i - 1]);
            
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 
                transform.rotation.eulerAngles.y + patternData.segmentRotationBias, 
                transform.rotation.eulerAngles.z);
        }

        public void IncreaseTouchCounter()
        {
            countTouches++;
            
            /*if (countTouches == 1)
            {
                for (var i = 0; i < 12; i++)
                    segments[i].GetComponent<Segment>().ChangeColor(1);
            }

            if (countTouches == 2)
            {
                for (var i = 0; i < 12; i++)
                    segments[i].GetComponent<Segment>().ChangeColor(2);
            }*/
            
            if (countTouches == 2) platformMover.ResetConcentration();
            if (countTouches == 3) DestroyPlatform(true);
        }
        
        public void DestroyPlatform(bool platformsIsMoving)
        {
            if (!tutorialUI.secondStepComplete && platformMover.concentration.slider.value >= 4) 
                tutorialUI.ShowSecondStep();

            /*for (var i = 0; i < 12; i++)
                segments[i].GetComponent<Segment>().ChangeColor(3);*/
            
            
            if (platformsIsMoving)
            {
                audioSource.PlayDestroySound();
                tubeMover.MoveTube(platformMover.platformMovementSpeed);
                platformMover.MovePlatforms();
                //var gainScore = effectsPool.GetGainScoreEffect();
                gainScore.SetText(1);
                gainScore.Animate();
                player.SetFallingTrailState();
                platformMover.IncreaseConcentration();
                platformMover.LevelStep();
            }

            if (platformMover.isLevelMode && patternData.isLast)
            {
                platformMover.FinishLevel(platformMover.gameManager.gameMode.levelMode.level);
            }

            for (var i = 0; i < segments.Length; i++)
            {
                //segments[i].ReturnTouchEffectToPool();
                segments[i].ReturnSegmentContentToPool();
            }
            
            if (platformsIsMoving)
                BreakDownPlatform();
            else
                DestroyAfterBreakAnimation();
            
            player.SetTriggerStay(false);
            
        }

        public void SetEnableMeshColliders(bool value)
        {
            for (var i = 0; i < meshColliders.Length; i++)
                meshColliders[i].enabled = value;
        }

        public void DestroyAfterBreakAnimation()
        {
            if (touchEffect != null) touchEffect.transform.SetParent(null);
            if (playerParticles != null) playerParticles.transform.SetParent(null);
            
            transform.position = platformPool.transform.position;

            platformPool.ReturnToPool(this);
        }

        private void BreakDownPlatform()
        {
            SetEnableMeshColliders(false);
            animator.Play(BREAK_ANIMATION_IDENTIFIER);
        }
    }
}