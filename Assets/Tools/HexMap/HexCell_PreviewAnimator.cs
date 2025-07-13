using DG.Tweening;
using UnityEngine;

public class HexCell_PreviewAnimator : MonoBehaviour
{
    private Vector3 originalScale;
    private float originalY;

    private void Awake()
    {
        originalScale = transform.localScale;
        originalY = transform.position.y;
    }

    private void OnEnable()
    {
        // Always reset to original before starting tweens
        transform.localScale = originalScale;
        var pos = transform.position;
        pos.y = originalY;
        transform.position = pos;

        transform.DOScale(1f, 0.1f);
        transform.DOMoveY(originalY, 0.1f);
    }

    private void OnDisable()
    {
        // Kill all tweens on this transform
        transform.DOKill();

        // Reset transform to original state
        transform.localScale = originalScale;

        var pos = transform.position;
        pos.y = originalY + 5;
        transform.position = pos;
    }
}
