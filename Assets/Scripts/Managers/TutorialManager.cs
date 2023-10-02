using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private StageData[] _stages; // All the stages this tutorial contains
    [SerializeField] private int _stageIndex = 0; // DON'T CHANGE IN INSPECTOR
    [SerializeField] private float _currentBeat = 0; // DON'T CHANGE IN INSPECTOR
    [SerializeField] private float _startBeat = 0; // The beat on which this stage started
    private int _damageTaken = 0; // How much damage the player has taken during this stage
    [SerializeField] private List<SpawnBeat> _spawns;
    [SerializeField] private SongManager _songManager;
    [SerializeField] private SongMeta _song;

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

    private void Start()
    {
        _songManager.StartSong(_song, false, true);
    }

    private StageData CurrentStage {
        get { return _stageIndex < _stages.Length ? _stages[_stageIndex] : null; }
    }

    // Create a deep copy of all spawns for the current stage
    private void LoadStage()
    {
        if (CurrentStage == null) return;

        _spawns = new List<SpawnBeat>();
        foreach (SpawnBeat s in CurrentStage.Spawns)
        {
            _spawns.Add(s);
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
        if (CurrentStage == null) return;

        // Check whether the current stage is over or not
        if (Conductor.RawSongBeat >= _startBeat + CurrentStage.Length)
		{
            _startBeat += CurrentStage.Length; // Set the start beat for the new stage

            // If no damage was taken, move to the next stage
            if (_damageTaken == 0)
			{
                _stageIndex++;
                // If there is no next stage to move to, move to the main scene (start the game!)
                if (_stageIndex >= _stages.Length)
				{
                    Debug.Log("Final stage complete! Moving to the main game.");
                    SceneManager.LoadScene("TitleScene");
				}
                else
                {
                    Debug.Log($"Progressing to stage {_stageIndex + 1}");
                    LoadStage();
                }
            }
            else
			{
                Debug.Log("Damage was taken! Repeating this stage.");

                LoadStage();
                _damageTaken = 0;
            }
        }

        // Check whether to spawn enemies
        if (_spawns.Count == 0) return;
        if (_currentBeat < _spawns[0].beat + _startBeat) return;

        SpawnBeat spawnData = _spawns[0];
        _spawns.RemoveAt(0);

        SpawnData data = new SpawnData(spawnData.beat + _startBeat, spawnData.direction, spawnData.type);

        EventManager.EventTrigger(EventType.SPAWN, data);
    }
}
