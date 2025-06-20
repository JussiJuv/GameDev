using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SelfDestructByAnim : MonoBehaviour
{
    void Start()
    {
        // Destroy after the length of the current animation clip
        var clipInfo = GetComponent<Animator>().runtimeAnimatorController.animationClips[0];
        Destroy(gameObject, clipInfo.length);
    }
}
