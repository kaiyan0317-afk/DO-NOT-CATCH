using UnityEngine;

public class TargetFishSelector : MonoBehaviour
{
    [SerializeField] SpriteRenderer targetDisplay;
    [SerializeField] Transform fishRoot;

    public static GameObject CurrentTargetFish;

    void Start()
    {
        // ƒŒƒxƒ‹1F‹›‚Í1•C‚¾‚¯
        if (fishRoot.childCount == 0) return;

        Transform fish = fishRoot.GetChild(0);
        CurrentTargetFish = fish.gameObject;

        SpriteRenderer sr = fish.GetComponentInChildren<SpriteRenderer>();
        if (sr && targetDisplay)
        {
            targetDisplay.sprite = sr.sprite;
        }
    }
}
