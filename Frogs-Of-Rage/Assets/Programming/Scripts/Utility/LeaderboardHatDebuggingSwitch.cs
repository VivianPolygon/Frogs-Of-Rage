using UnityEngine;

public class LeaderboardHatDebuggingSwitch : MonoBehaviour
{
    [SerializeField] private HatsTester _hat;
    [SerializeField] private ScoreboardDebugging _score;
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(Screen.width - 200, 0, 200, 100));
        if(GUILayout.Button("Toggle Hat Debugging"))
        {
            _hat.enabled = true;
            _score.enabled = false;
        }
        if (GUILayout.Button("Toggle ScoreBoard Debugging"))
        {
            _hat.enabled = false;
            _score.enabled = true;
        }
        if (GUILayout.Button("Disable Both"))
        {
            _hat.enabled = false;
            _score.enabled = false;
        }
        GUILayout.EndArea();
    }
}
