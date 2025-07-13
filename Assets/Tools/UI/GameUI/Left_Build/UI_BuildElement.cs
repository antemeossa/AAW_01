using DG.Tweening;
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.VisualScripting;

public class UI_BuildElement : MonoBehaviour
{
    [SerializeField] private TMP_Text headerText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private TMP_Text alienTypeText;

    [SerializeField] private GameObject generalTypePanel;
    [SerializeField] private GameObject specializedTypePanel;


    [SerializeField] private List<GameObject> specializedButtons = new List<GameObject>();


    private Tween generalTween;
    private Tween specializedTween;



    private void OnEnable()
    {
        SetDefaults();
    }

    private void OnDisable()
    {
        // Kill any active tweens
        generalTween?.Kill();
        specializedTween?.Kill();
    }

    public void OnElementClicked()
    {

        generalTween = generalTypePanel.transform.DOScaleY(0f, .1f).OnComplete(() =>
         {
             generalTypePanel.SetActive(false);
             specializedTypePanel.SetActive(true);
             specializedTypePanel.transform.DOScaleY(1f, .1f);

             foreach (var button in specializedButtons)
             {
                 button.gameObject.SetActive(true);
                 button.transform.DOScale(1f, .1f);
             }
         });
    }

    public void OnCancelClicked()
    {
        foreach (var button in specializedButtons)
        {
            button.gameObject.SetActive(false);
        }

        specializedTween = specializedTypePanel.transform.DOScaleY(0f, .1f).OnComplete(() =>
         {
             specializedTypePanel.SetActive(false);
             generalTypePanel.SetActive(true);
             generalTypePanel.transform.DOScaleY(1f, .1f);
         });
    }

    public void SetDefaults()
    {
        // Activate general, deactivate specialized
        generalTypePanel.SetActive(true);
        specializedTypePanel.SetActive(false);

        foreach (var button in specializedButtons)
        {
            button.transform.localScale = Vector3.zero;
            button.SetActive(false);
        }

        //Set texts 
        alienTypeText.text = "Human";

        // Set their scale values accordingly
        generalTypePanel.transform.localScale = new Vector3(1f, 1f, 1f);
        specializedTypePanel.transform.localScale = new Vector3(1f, 0f, 1f);
    }

    public void OnAlienTypeSelected(string alienType)
    {
        alienTypeText.text = alienType;
    }
}
