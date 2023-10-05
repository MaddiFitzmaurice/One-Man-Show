public enum EventType
{
	PARRY_INPUT, // Broadcasts input direction                    (data: StageDirection)
	PARRY_HIT,   // Broadcasts when a parry hits an enemy         (data: StageDirection)
	PARRY_MISS,  // Broadcasts when a parry does not hit an enemy (data: StageDirection)
	BEAT,        // Broadcasts every half-beat                    (data: float (Nearest raw half beat))
	SFX,         // Broadcasts to play a sound effect             (data: SFXData)
	SPAWN,       // Broadcasts to spawn an enemy                  (data: SpawnData)
	PLAYER_HIT,  // Broadcasts when an enemy hits the player      (data: StageDirection)
	PLAYER_DIED, // Broadcasts when the player dies               (data: null)
	HEALTH_UI,   // Broadcasts when the player's health changes   (data: uint (Health value))
	SONG_START,  // Broadcasts on beat 0                          (data: null)
	SONG_END     // Broadcasts when the song ends                 (data: null)
};