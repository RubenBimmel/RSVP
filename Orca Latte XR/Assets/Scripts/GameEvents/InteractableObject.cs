using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class InteractableObject : MonoBehaviour {

    public static InteractableObject activeObject;
    public GameObject textPrompt;
    public UnityEvent onClickEvent;
    public InteractableButton[] buttons;
    public int selected = -1;

    private float scaleSpeed = 7f;

    private void Start()
    {
        textPrompt.transform.rotation = Quaternion.LookRotation(Camera.main.transform.position - textPrompt.transform.position, Vector3.up);
        if (PlayerPrefs.HasKey(name)) {
            selected = PlayerPrefs.GetInt(name);
            UpdateButtonSprites();
        }

        textPrompt.transform.localScale = Vector3.zero;
    }

    void Update () {
        if (activeObject == this)
        {
            textPrompt.transform.localScale = Vector3.MoveTowards(textPrompt.transform.localScale, Vector3.one, scaleSpeed * Time.deltaTime);
        } else
        {
            textPrompt.transform.localScale = Vector3.MoveTowards(textPrompt.transform.localScale, Vector3.zero, scaleSpeed * Time.deltaTime);
        }
    }

    public void Select (InteractableButton button)
    {
        if (buttons[0] == button)
        {
            selected = 0;
        } else
        {
            selected = 1;
        }
        UpdateButtonSprites();
        NarrativeHandler.instance.OnInteractWithObject();
        PlayerPrefs.SetInt(name, selected);
    }

    private void UpdateButtonSprites ()
    {
        buttons[selected].SetActive(true);
        buttons[1 - selected].SetActive(false);
    }

}
