using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FateGames;
using DG.Tweening;
using TMPro;

public class LevitatingText : MonoBehaviour, IPooledObject
{
    [SerializeField] private float duration = 1f;

    private TextMeshProUGUI text;
    private CanvasGroup canvasGroup;
    private void Awake()
    {
        Transform = transform;
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }
    
    public Transform Transform { get; private set; }

    public void OnObjectSpawn()
    {
        Levitate();
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    private void Levitate()
    {
        float currentHeight = Transform.position.y;
        Transform.DOMoveY(currentHeight + 8, duration);
        canvasGroup.alpha = 1;
        DOTween.To((val) =>
        {
            canvasGroup.alpha = val;
        }, 2, 0, duration).OnComplete(Deactivate);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }


}
