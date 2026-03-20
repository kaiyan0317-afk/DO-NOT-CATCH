using UnityEngine;

public class GoalArea : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        FishMove fm = other.GetComponent<FishMove>();
        if (fm == null) return;

        FishGameManager.Instance.SetCurrentTarget(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        FishMove fm = other.GetComponent<FishMove>();
        if (fm == null) return;

        FishGameManager.Instance.ClearCurrentTarget(other.gameObject);
    }
}
