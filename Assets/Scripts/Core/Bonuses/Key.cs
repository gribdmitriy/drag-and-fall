﻿using System.Collections;
using Data.Core.Segments.Content;
using ObjectPool;
using Sound;
using UnityEngine;

namespace Core.Bonuses
{
    public class Key : MonoBehaviour
    {
        private BonusController bonusController;
        private SegmentContentPool segmentContentPool;

        private Transform startMarker;
        public Transform endMarker;

        private bool isMoving;
        public float speed = 1.0F;
        private float startTime;
        private float journeyLength;
        private bool ismove = true;
        private Coroutine coroutine;
        private KeySound keySound;

        public void BindAudio(KeySound keySound)
        {
            this.keySound = keySound;
        }

        public void Construct(BonusController _bonusController, SegmentContentPool segmentContentPool)
        {
            bonusController = _bonusController;
            this.segmentContentPool = segmentContentPool;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Player>().CollectKey(1);
                other.GetComponent<Player>().SpawnKeyCollectingEffect();
                keySound.Play();
                segmentContentPool.ReturnObjectToPool(SegmentContent.Key, gameObject);
            }
        }

        
        public void SetMovingFalse()
        {
            isMoving = false;
        }
        
        public void MoveToTargetTransform(Transform _transform)
        {
            startMarker = transform;
            endMarker = _transform;
            startTime = Time.time;
            journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
            isMoving = true;
        }

        private void FixedUpdate()
        {
            if (!isMoving) return;
            var distCovered = (Time.time - startTime) * speed;
            var fractionOfJourney = distCovered / journeyLength;
            startMarker.position = Vector3.Lerp(startMarker.position, endMarker.position, fractionOfJourney);
        }
    }
}