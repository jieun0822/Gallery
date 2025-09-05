using Spine.Unity;
using UnityEngine;

public class PlayOnEnableAnimation : MonoBehaviour
{
    public string flowerName;

    private void OnEnable() 
    { 
        var skeleton = GetComponent<SkeletonAnimation>(); 
        skeleton.AnimationState.SetAnimation(0, flowerName + "_grow_up", false); 
    }
}
