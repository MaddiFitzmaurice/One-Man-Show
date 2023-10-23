using System.Collections;
using System.Collections.Generic;
using UnityEngine;	

public class TutorialManager : MonoBehaviour
{
	[SerializeField] private StageData[] _stages; // All the stages this tutorial contains
	[SerializeField] private int _stageIndex = 0; // DON'T CHANGE IN INSPECTOR
	[SerializeField] private float _currentBeat = 0; // DON'T CHANGE IN INSPECTOR
	[SerializeField] private float _startBeat = 0; // The beat on which this stage started
	private uint _damageTaken = 0; // How much damage the player has taken during this stage
	private uint _misses = 0;      // How many times the player missed a parry during this stage
	private uint _enemies_spawned = 0;
	[SerializeField] private bool _repeatOnDamage = true;
	[SerializeField] private bool _repeatOnMiss = true;
	[SerializeField] private List<SpawnBeat> _spawns;
	[SerializeField] private SongManager _songManager;
	[SerializeField] private SongMeta _song;
	[SerializeField] private Fader _promptKeysFader;

	private void OnEnable()
	{
		EventManager.EventSubscribe(EventType.BEAT, BeatHandler);
		EventManager.EventSubscribe(EventType.PLAYER_HIT, TrackDamage);
		EventManager.EventSubscribe(EventType.PARRY_MISS, TrackMiss);
		EventManager.EventSubscribe(EventType.PARRY_HIT, RemoveEnemy);
	}

	private void OnDisable()
	{
		EventManager.EventUnsubscribe(EventType.BEAT, BeatHandler);
		EventManager.EventUnsubscribe(EventType.PLAYER_HIT, TrackDamage);
		EventManager.EventUnsubscribe(EventType.PARRY_MISS, TrackMiss);
		EventManager.EventUnsubscribe(EventType.PARRY_HIT, RemoveEnemy);
	}

	private void RemoveEnemy(object data)
	{
		_enemies_spawned--;
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
		_enemies_spawned--;
	}

	public void TrackMiss(object data)
	{
		if (_enemies_spawned == 0) return;
		_misses++;
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
			if (_repeatOnDamage && _damageTaken != 0)
			{
				Debug.Log("Damage was taken! Repeating this stage.");

				LoadStage();
			}
			else if(_repeatOnMiss && _misses != 0)
			{
				Debug.Log("Parry was missed! Repeating this stage.");

				LoadStage();
			}
			else
			{
				_stageIndex++;
				// Since it's no longer the first stage, fade out the key prompts if they're still there
				if (_promptKeysFader != null)
				{
					_promptKeysFader.enabled = true;
				}

				// If there is no next stage to move to, move to the main scene (start the game!)
				if (_stageIndex >= _stages.Length)
				{
					Debug.Log("Final stage complete! Moving to the main game.");
					EventManager.EventTrigger(EventType.SONG_END, null);
				}
				else
				{
					Debug.Log($"Progressing to stage {_stageIndex + 1}");
					LoadStage();
				}
			}

			_damageTaken = 0;
			_misses = 0;
		}

		// Check whether to spawn enemies
		if (_spawns.Count == 0) return;
		if (_currentBeat < _spawns[0].beat + _startBeat) return;

		SpawnBeat spawnData = _spawns[0];
		_spawns.RemoveAt(0);

		SpawnData data = new SpawnData(spawnData.beat + _startBeat, spawnData.direction, spawnData.type);

		EventManager.EventTrigger(EventType.SPAWN, data);
		_enemies_spawned++;
	}
}
