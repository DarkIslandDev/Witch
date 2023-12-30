using System;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    // public PlayerComponents components;
    //
    //
    // [SerializeField] private Transform interactionPoint;
    // [SerializeField] private float interactionRadius;
    // [SerializeField] private LayerMask interactableLayer;
    //
    // private readonly Collider[] colliders = new Collider[5];
    // private int numFound;
    //
    // private void Update()
    // {
    //     InteractionWithObject();
    // }
    //
    // private void InteractionWithObject()
    // {
    //     numFound = Physics.OverlapSphereNonAlloc(interactionCollider.transform.position, interactionRadius, colliders,
    //         interactableLayer);
    //
    //     if (numFound > 0)
    //     {
    //         IPlayerInteraction interactable = colliders[0].GetComponent<IPlayerInteraction>();
    //
    //         interactable?.Interact(components, this);
    //     }
    // }
}