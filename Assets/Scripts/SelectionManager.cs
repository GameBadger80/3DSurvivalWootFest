using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionManager : MonoBehaviour
{

    public GameObject interaction_Info_UI;
    public Camera playerView; // assign in Inspector or it will try to find by name "PlayerView"
    Text interaction_text;

    private void Start()
    {
        if (interaction_Info_UI == null)
        {
            Debug.LogWarning("SelectionManager: 'interaction_Info_UI' is not assigned in the Inspector.", this);
        }
        else
        {
            interaction_text = interaction_Info_UI.GetComponent<Text>();
            if (interaction_text == null)
            {
                Debug.LogWarning("SelectionManager: 'interaction_Info_UI' has no Text component.", interaction_Info_UI);
            }

            interaction_Info_UI.SetActive(false); // start hidden
        }

        if (playerView == null)
        {
            var go = GameObject.Find("PlayerView");
            if (go != null)
            {
                playerView = go.GetComponent<Camera>();
                if (playerView == null)
                    Debug.LogWarning("SelectionManager: GameObject 'PlayerView' was found but has no Camera component.", go);
            }
            else
            {
                Debug.LogWarning("SelectionManager: No Camera assigned and GameObject 'PlayerView' not found. Raycasts will not run.", this);
            }
        }
    }

    void Update()
    {
        if (playerView == null)
            return; // nothing to raycast from

        Ray ray = playerView.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var selectionTransform = hit.transform;
            if (selectionTransform == null)
            {
                if (interaction_Info_UI != null) interaction_Info_UI.SetActive(false);
                return;
            }

            if (selectionTransform.TryGetComponent<InteractableObject>(out var interactable) && interaction_text != null && interaction_Info_UI != null)
            {
                interaction_text.text = interactable.GetItemName();
                interaction_Info_UI.SetActive(true);
            }
            else
            {
                if (interaction_Info_UI != null) interaction_Info_UI.SetActive(false);
            }
        }
        else
        {
            if (interaction_Info_UI != null) interaction_Info_UI.SetActive(false);
        }
    }
}