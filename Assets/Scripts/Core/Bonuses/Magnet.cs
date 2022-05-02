﻿using System.Collections;
using Common;
using Cysharp.Threading.Tasks;
using Data.Core.Segments.Content;
using ObjectPool;
using Sound;
using UnityEngine;

namespace Core.Bonuses
{
    public class Magnet : MonoBehaviour
    {
        private BonusController bonusController;
        private SegmentContentPool segmentContentPool;
        
        private Transform startMarker;
        public Transform endMarker;

        public float speed = 1.0F;
        private float startTime;
        private float journeyLength;
        private bool ismove = true;
        private Coroutine coroutine;
        private BonusSoundManager bonusSoundManager;
        
        public void BindAudio(BonusSoundManager bonusSoundManager)
        {
            this.bonusSoundManager = bonusSoundManager;
        }
        
        public void Construct(BonusController _bonusController, SegmentContentPool segmentContentPool)
        {
            bonusController = _bonusController;
            this.segmentContentPool = segmentContentPool;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(Constants.TAGS.PLAYER)) return;
            
            other.GetComponent<Player>().SetActiveMagnet(true);
            other.GetComponent<Player>().SpawnBonusCollectingEffect();
            bonusController.ActivateBonus(BonusType.Magnet);
            bonusSoundManager.PlayMagnetSound();
            segmentContentPool.ReturnObjectToPool(SegmentContent.Magnet, gameObject);
        }
        
        public void MoveToTargetTransform(Transform _transform)
        {
            startMarker = transform;
            endMarker = _transform;
            startTime = Time.time;
            journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
            Move();
        }
    
        private async UniTask Move()
        {
            while (startMarker.position != endMarker.position) 
            {
                var distCovered = (Time.time - startTime) * speed;
                var fractionOfJourney = distCovered / journeyLength;
                startMarker.position = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney);
                await UniTask.Yield();
            }
        }
    }
}