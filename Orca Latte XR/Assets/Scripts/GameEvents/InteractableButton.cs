using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]
public class InteractableButton : MonoBehaviour {

    public Sprite activeSprite;
    public Sprite inactiveSprite;

    private InteractableObject parentObject;
    private SpriteRenderer _renderer;

    private void Start()
    {
        parentObject = GetComponentInParent<InteractableObject>();
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void OnMouseDown()
    {
        parentObject.Select(this);
    }

    public void SetActive(bool active)
    {
        _renderer.sprite = active ? activeSprite : inactiveSprite;
    }
}
