using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    private int hitCount = 0;
    private int missCount = 0;

    [SerializeField] private TMP_Text hitText;
    [SerializeField] private TMP_Text missText;

    private void OnEnable()
    {
        MusicManager.Instance.OnHit += IncrementHitCount;
        MusicManager.Instance.OnMiss += IncrementMissCount;
    }

    private void OnDisable()
    {
        MusicManager.Instance.OnHit -= IncrementHitCount;
        MusicManager.Instance.OnMiss -= IncrementMissCount;
    }

    private void IncrementHitCount()
    {
        hitCount++;
        UpdateUI();
    }

    private void IncrementMissCount()
    {
        missCount++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        hitText.text = $"Hits: {hitCount}";
        missText.text = $"Misses: {missCount}";
    }
}
