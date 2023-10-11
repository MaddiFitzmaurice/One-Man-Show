using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class StatManager : MonoBehaviour
{
	public static uint misses = 0;
	public static uint hits = 0;

	public static uint damage_taken = 0;

	// This is a cumulative tracker of the absolute input offset from the attack time
	public static double time_inaccuracy = 0.0;

	public static double avg_time_inaccuracy
	{
		get => hits != 0 ? time_inaccuracy / (double)hits : 0.0;
	}

	private void Start()
	{
		ResetStats();
	}

	private void OnEnable()
	{
		EventManager.EventSubscribe(EventType.PARRY_MISS, OnMiss  );
		EventManager.EventSubscribe(EventType.PARRY_HIT,  OnHit   );
		EventManager.EventSubscribe(EventType.PLAYER_HIT, OnDamage);
	}

	private void OnDisable()
	{
		EventManager.EventUnsubscribe(EventType.PARRY_MISS, OnMiss  );
		EventManager.EventUnsubscribe(EventType.PARRY_HIT,  OnHit   );
		EventManager.EventUnsubscribe(EventType.PLAYER_HIT, OnDamage);
	}

	private void OnMiss(object data)
	{
		misses++;
	}

	private void OnHit(object data)
	{
		if (data == null) return;
		hits++;
		time_inaccuracy += Math.Abs((((StageDirection, double))data).Item2);
	}

	private void OnDamage(object data)
	{
		damage_taken++;
	}

	public void ResetStats()
	{
		misses = 0;
		hits = 0;
		damage_taken = 0;
		time_inaccuracy = 0.0;
	}
}
