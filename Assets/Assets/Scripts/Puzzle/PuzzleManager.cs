using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    [Header("Runes (in order)")]
    public List<GameObject> runes;
    public Animator gateAnimator;
    public Collider2D doorCollider;

    [Header("SFX")]
    public AudioClip solveSFX;

    private int _progress = 0;

    private void Start()
    {
        ResetPuzzle();
    }

    public void TryActivate(int runeIndex)
    {
        if (runeIndex == _progress)
        {
            // correct next rune ? light it
            runes[runeIndex].SetActive(true);
            _progress++;

            if (_progress >= runes.Count)
                SolvePuzzle();
        }
        else
        {
            // wrong rune ? reset everything
            ResetPuzzle();
        }
    }

    private void ResetPuzzle()
    {
        _progress = 0;
        foreach (var r in runes)
            r.SetActive(false);
    }

    private void SolvePuzzle()
    {
        AudioManager.Instance.PlaySFX(solveSFX);
        gateAnimator.SetTrigger("OpenGate");
        doorCollider.enabled = false;
        // disable all PillarInteraction so they stop responding
        foreach (var pi in FindObjectsByType<PillarInteraction>(FindObjectsSortMode.None))
             pi.enabled = false;
    }
}
