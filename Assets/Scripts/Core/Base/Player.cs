﻿using System;
using System.Collections;
using System.Text;
using Core.Bonuses;
using Data;
using Data.Core.Segments;
using DG.Tweening;
using ObjectPool;
using Sound;
using TMPro;
using UI.InfinityUI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Core
{
    public class Player : MonoBehaviour
    {
        private const string SEGMENT = "Segment";
        private const string IDLE = "Idle";
        private const string BOUNCE = "Bounce";
        private const string BOUNCE_TRIGGER = "BounceTrigger";

        private bool isRotation;
        private float startTime;
        private Quaternion target;
        private float speedRotation = 0.2f;
        private float timeCount;
        
        public event Action failed;
        [SerializeField] private ParticleSystem particleSystem;
        [SerializeField] private PlayerSounds playerSounds;
        [SerializeField] private PlatformSound platformSound;
        [SerializeField] private TubeMover tubeMover;
        [SerializeField] private EffectsPool effectsPool;
        [SerializeField] private GameObject shieldEffect;
        [SerializeField] private PlatformMover platformMover;
        [SerializeField] private MagnetPlayer magnetPlayer;
        [SerializeField] private Animator animator;
        [SerializeField] private GameObject fallingTrail;
        [SerializeField] private GameObject trail;
        [SerializeField] private GameManager gameManager;
        [SerializeField] private VisualController visualController;
        [SerializeField] private BonusController bonusController;
        [SerializeField] private CoinPanel coinPanel;
        [SerializeField] private CrystalPanel crystalPanel;
        [SerializeField] private KeyPanel keyPanel;
        [SerializeField] private FailedInfinityUI failedInfinityUI;
        [SerializeField] private SessionData sessionData;
        [SerializeField] private FreeSpeedIncrease freeSpeedIncrease;
        [SerializeField] private GameObject fireEffect;
        [SerializeField] private GameObject sled;
        [SerializeField] private GameObject fireBacklight;
        
        private bool triggerStay;

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        public void SetActiveFireEffect(bool value)
        {
            fireEffect.SetActive(value);
            fireBacklight.SetActive(value);
            if (!value) playerSounds.StopFireSound();
        }

        public void MaximumFire()
        {
            playerSounds.PlayFireSound();
            var fireParticle = fireEffect.GetComponent<ParticleSystem>();
            var main = fireParticle.main;
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 1, 1, 1f), new Color(1, 1, 1, 0.5f));
            main.startSpeed = -3;
            var backlightParticle = fireBacklight.GetComponent<ParticleSystem>();
            var backlightMain = backlightParticle.main;
            backlightMain.startColor = new ParticleSystem.MinMaxGradient(new Color(backlightMain.startColor.color.r,
                backlightMain.startColor.color.g, backlightMain.startColor.color.b, 0.25f));
        }
        
        public void RandomRotate()
        {
            var randomX = Random.Range(-35, 35);
            var randomY = Random.Range(-35, 60);
            var randomZ = Random.Range(0, 35);
            isRotation = true;
            target = Quaternion.Euler(new Vector3(randomX, randomY, randomZ));
            startTime = Time.time;
            timeCount = 0;
            //transform.DORotate(new Vector3(randomX, randomY, randomZ), 0.5f);
        }

        private void Update()
        {
            Rotation();
            ProcessPlatformCollisions();
        }

        private void Rotation()
        {
            if (!isRotation) return;
            //var distCovered = (Time.time - startTime) * 30;
            //var fractionOfJourney = distCovered / journeyLength;
            transform.rotation = Quaternion.Lerp(transform.rotation, target, timeCount * speedRotation);
            timeCount = timeCount + Time.deltaTime;
            //if (fractionOfJourney >= 1) isRotation = false;
        }
        
        public void SpawnBonusCollectingEffect()
        {
            var effect = effectsPool.GetBonusCollectingEffect();
            effect.transform.SetParent(null);
            effect.transform.position = transform.position;
            effect.GetComponent<ParticleSystem>().Play();
        }
        
        public void SpawnCoinCollectingEffect()
        {
            var effect = effectsPool.GetCoinCollectingEffect();
            effect.transform.SetParent(null);
            effect.transform.position = transform.position;
            effect.GetComponent<ParticleSystem>().Play();
        }
        
        public void SpawnCrystalCollectingEffect()
        {
            var effect = effectsPool.GetCrystalCollectingEffect();
            effect.transform.SetParent(null);
            effect.transform.position = transform.position;
            effect.GetComponent<ParticleSystem>().Play();
        }
        
        public void SpawnKeyCollectingEffect()
        {
            var effect = effectsPool.GetKeyCollectingEffect();
            effect.transform.SetParent(null);
            effect.transform.position = transform.position;
            effect.GetComponent<ParticleSystem>().Play();
        }

        public void IncreaseFireEffect6()
        {
            playerSounds.PlayFireSound();
            var fireParticle = fireEffect.GetComponent<ParticleSystem>();
            var main = fireParticle.main;
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 1, 1, 1f), new Color(1, 1, 1, 0.5f));
            main.startSpeed = -3;
            var backlightParticle = fireBacklight.GetComponent<ParticleSystem>();
            var backlightMain = backlightParticle.main;
            backlightMain.startColor = new ParticleSystem.MinMaxGradient(new Color(backlightMain.startColor.color.r,
                backlightMain.startColor.color.g, backlightMain.startColor.color.b, 0.25f));

        }

        public void IncreaseFireEffect5()
        {
            var fireParticle = fireEffect.GetComponent<ParticleSystem>();
            var main = fireParticle.main;
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 1, 1, 0.25f), new Color(1, 1, 1, 0.2f));
            main.startSpeed = -2.5f;
            var backlightParticle = fireBacklight.GetComponent<ParticleSystem>();
            var backlightMain = backlightParticle.main;
            backlightMain.startColor = new ParticleSystem.MinMaxGradient(new Color(backlightMain.startColor.color.r,
                backlightMain.startColor.color.g, backlightMain.startColor.color.b, 0.1f));
        }
        
        public void IncreaseFireEffect4()
        {
            var fireParticle = fireEffect.GetComponent<ParticleSystem>();
            var main = fireParticle.main;
            main.startColor = new ParticleSystem.MinMaxGradient(new Color(1, 1, 1, 0), new Color(1, 1, 1, 0.2f));
            main.startSpeed = -2f;
            var backlightParticle = fireBacklight.GetComponent<ParticleSystem>();
            var backlightMain = backlightParticle.main;
            backlightMain.startColor = new ParticleSystem.MinMaxGradient(new Color(backlightMain.startColor.color.r,
                backlightMain.startColor.color.g, backlightMain.startColor.color.b, 0.05f));
        }

        private void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();
        }

        public void SetActiveMagnet(bool value)
        {
            magnetPlayer.gameObject.SetActive(value);
        }

        public void ChangeTheme()
        {
            meshFilter.mesh = visualController.GetPlayerMesh();
            meshRenderer.material = visualController.GetPlayerMaterial();
            
            //Destroy(trail);
           // Destroy(fallingTrail);
            
            //trail = Instantiate(visualController.GetTrail(), transform.position, Quaternion.identity, transform);
           // fallingTrail = Instantiate(visualController.GetFallingTrail(), new Vector3(0, 0.15f, -0.7f), Quaternion.identity,
             //   transform.parent);
        }

        public void TryOnPlayerSkin(Mesh _mesh, Material _material)
        {
            meshFilter.mesh = _mesh;
            meshRenderer.material = _material;
        }

        public void TryOnTrailSkin(GameObject _trail)
        {
            Destroy(trail);
            trail = Instantiate(_trail, transform.position, Quaternion.identity, transform);
        }

        public void TryOnFallingTrailSkin(GameObject _fallingTrail)
        {
            Destroy(fallingTrail);
            fallingTrail = Instantiate(_fallingTrail, new Vector3(0, 0.15f, -0.7f), Quaternion.identity,
                transform);
        }
        
        public void PlayIdleAnim()
        {
            animator.Play(IDLE);
        }

        public void DisableTrail()
        {
            trail.SetActive(false);
        }

        public void EnableTrail()
        {
            trail.SetActive(trail);
        }

        public void PlayBounceAnim()
        {
            animator.Play(BOUNCE);
        }

        public void SetTriggerStay(bool _value)
        {
            triggerStay = _value;
        }

        public void ContinueGame()
        {
            SetFallingTrailState();
        }

        public void MovePlatforms()
        {
            tubeMover.MoveTube();
            platformMover.MovePlatforms();
        }

        

        private void ProcessPlatformCollisions()
        {
            if (triggerStay) return;

            var position = transform.position;
            var centerRay = new Ray(position, Vector3.down);

            if (Physics.Raycast(centerRay, out var centerHit, 0.105f))
            {
                if (!centerHit.transform.CompareTag(SEGMENT)) return;

                var segment = centerHit.collider.GetComponent<Segment>();

                /*if (platformMover.platformMovementSpeed >= 6 && segment.segmentData.segmentType != SegmentType.Hole && !bonusController.accelerationIsActive)
                {
                    //GetComponent<Animator>().Play("SpecialBounce");
                    SetTriggerStay(true);
                    //SetDefaultState();
                    
                    platformMover.DestroyPlatform(true);
                    
                    freeSpeedIncrease.ResetSpeed();
                    return;
                }*/

                switch (segment.segmentData.segmentType)
                {
                    case SegmentType.Ground:
                        if (bonusController.accelerationIsActive)
                        {
                            SetTriggerStay(true);
                            platformMover.DestroyPlatform(true);
                            bonusController.StepAcceleration();
                        }
                        else
                        {
                            SetTriggerStay(true);
                            SetDefaultState();
                            if (gameManager.gameStarted) segment.IncreasePlatformTouchCounter();
                            freeSpeedIncrease.ResetSpeed();
                            var instance = effectsPool.GetTouchEffect();
                            var parent = centerHit.collider.transform.parent;
                            parent.GetComponent<Platform>().touchEffect = instance;
                            instance.transform.position =
                                new Vector3(centerHit.point.x, centerHit.point.y + 0.01f, centerHit.point.z);
                            instance.transform.rotation = Quaternion.Euler(-90, 0, 0);
                            instance.transform.SetParent(segment.transform);
                            playerSounds.PlayTouchSound();

                            var particleSystems = effectsPool.GetPlayerParticles();
                            parent.GetComponent<Platform>().playerParticles = particleSystems;
                            var transform1 = particleSystems.transform;
                            transform1.position = new Vector3(centerHit.point.x,
                                centerHit.point.y + 0.01f, centerHit.point.z);
                            transform1.rotation = Quaternion.identity;
                            particleSystems.transform.SetParent(segment.transform);
                            platformSound.ResetPitch();
                        }

                        break;
                    case SegmentType.Hole:
                        if (!gameManager.gameStarted) return;
                        if (bonusController.accelerationIsActive)
                        {
                            SetTriggerStay(true);
                            platformMover.DestroyPlatform(true);
                            bonusController.StepAcceleration();
                        }
                        else
                        {
                            freeSpeedIncrease.IncreaseSpeed();
                            SetTriggerStay(true);
                            platformMover.DestroyPlatform(true);
                        }

                        break;
                    case SegmentType.Let:
                        if (!gameManager.gameStarted) return;
                        if (bonusController.accelerationIsActive)
                        {
                            SetTriggerStay(true);
                            platformMover.DestroyPlatform(true);
                            bonusController.StepAcceleration();
                            return;
                        }

                        if (bonusController.shieldIsActive)
                        {
                            bonusController.DeactivateBonus(BonusType.Shield);
                            SetTriggerStay(true);
                            freeSpeedIncrease.ResetSpeed();
                            SetDefaultState();
                            return;
                        }

                        platformSound.ResetPitch();
                        freeSpeedIncrease.ResetSpeed();
                        platformMover.Failed();
                        SetTriggerStay(true);
                        SetFailedState();
                        break;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(BOUNCE_TRIGGER)) SetTriggerStay(false);
        }

        public void SetFailedState()
        {
            PlayIdleAnim();
          //  DisableFallingTrail();
            DisableTrail();
        }

        public void SetFallingTrailState()
        {
            PlayIdleAnim();
          //  EnableFallingTrail();
            DisableTrail();
        }

        public void SetDefaultState()
        {
            PlayBounceAnim();
            EnableTrail();
           // DisableFallingTrail();
        }

        public void CollectCrystal(int count)
        {
            crystalPanel.AddCrystals(count);
            sessionData.AddCrystals(count);
        }
        
        public void CollectCoin(int count)
        {
            coinPanel.AddCoins(count);
            sessionData.AddCoins(count);
        }
        
        public void CollectKey(int count)
        {
            keyPanel.AddKeys(count);
            sessionData.AddKeys(count);
        }
    }
}

