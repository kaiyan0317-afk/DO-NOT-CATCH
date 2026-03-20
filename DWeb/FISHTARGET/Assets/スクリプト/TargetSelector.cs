using UnityEngine;
using System.Collections.Generic;

public class TargetSelector : MonoBehaviour
{
    public List<Fish> fishes;
    bool decided = false;

    void OnMouseDown()
    {
        if (decided) return;
        if (GameManager.Instance.State != GameManager.GameState.Ready) return;

        decided = true;

        int index = Random.Range(0, fishes.Count);
        fishes[index].SetTarget();

        GameManager.Instance.SetSelected();
    }
}
