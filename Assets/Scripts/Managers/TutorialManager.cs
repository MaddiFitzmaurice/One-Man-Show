using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private List<StageData> _stages; // All the stages this tutorial contains
    [SerializeField] private int _stageIndex = 0; // DON'T CHANGE IN INSPECTOR
    [SerializeField] private float _currentBeat = 0; // DON'T CHANGE IN INSPECTOR
    [SerializeField] private float _startBeat = 0; // The beat on which this stage started
    private int _damageTaken = 0; // How much damage the player has taken during this stage
    [SerializeField] private List<SpawnData> _spawns;

    private void OnEnable()
    {
        EventManager.EventSubscribe(EventType.BEAT, BeatHandler);
        EventManager.EventSubscribe(EventType.PLAYER_HIT, TrackDamage);
    }

    private void OnDisable()
    {
        EventManager.EventUnsubscribe(EventType.BEAT, BeatHandler);
    }

    private void Awake()
    {
        EventManager.EventInitialise(EventType.SPAWN);
        LoadStage();
    }

    private StageData CurrentStage {
        get { return _stages[_stageIndex]; }
    }

    // Create a deep copy of all spawns for the current stage
    private void LoadStage()
	{
        _spawns = new List<SpawnData>();
        foreach (SpawnData s in CurrentStage.Spawns)
		{
            SpawnData newSpawn = new SpawnData(s.Beat, s.Direction, s.Type);
            _spawns.Add(newSpawn);
		}
	}

    // Register that the player has been damaged
    public void TrackDamage(object data)
	{
        _damageTaken++;
    }

    // Update current beat
    public void BeatHandler(object data)
    {
        _currentBeat = (float)data;

        CheckBeat();
    }

    public void CheckBeat()
    {
        // Check whether the current stage is over or not
        if (Conductor.RawSongBeat >= _startBeat + CurrentStage.Length)
		{
            _startBeat += CurrentStage.Length; // Set the start beat for the new stage

            // If no damage was taken, move to the next stage
            if (_damageTaken == 0)
			{
                _stageIndex++;
                // If there is no next stage to move to, move to the main scene (start the game!)
                if (_stageIndex >= _stages.Count)
				{
                    Debug.Log("Final stage complete! Moving to the main game.");
                    //SceneManager.LoadScene("MainScene");
				}
            }
            else
			{
                Debug.Log("Damage was taken! Repeating this stage.");
			}

            LoadStage();
            _damageTaken = 0;
		}

        // Check whether to spawn enemies
        if (_spawns.Count > 0)
        {
            if (_currentBeat >= _spawns[0].Beat + _startBeat)
            {
                EventManager.EventTrigger(EventType.SPAWN, _spawns[0]);
                _spawns.RemoveAt(0);
            }
        }
    }
}
