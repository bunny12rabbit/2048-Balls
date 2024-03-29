﻿using UnityEngine;
using static Constants;

namespace Logic
{
    [RequireComponent(typeof(Collider2D))]
    public class DeathZoneHandler : MonoBehaviour
    {
        [SerializeField] private Collider2D trigger;

        [SerializeField] private TagsNames.Tags comparingTag;


        private void OnValidate()
        {
            CheckReference();
        }

        private void CheckReference()
        {
            if (trigger)
                return;

            trigger = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.gameObject.CompareTag(TagsNames.GetTag(comparingTag)))
                return;

            var parent = other.transform.parent;

            if (parent)
                parent.gameObject.PushBackToPool();
            else
                other.gameObject.PushBackToPool();
        }
    }
}