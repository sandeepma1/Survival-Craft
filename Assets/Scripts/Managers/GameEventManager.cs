﻿using UnityEngine;
using System.Collections;

public static class GameEventManager
{
	public static Vector2 currentSelectedTilePosition = Vector2.zero;
	public const float baseStrengthWithTool = 1f;
	public static float baseStrengthWithoutTool = 2.5f;
	public static int numberOfislands = 0;

	public delegate void GameEvent ();

	public enum E_STATES
	{
		e_game,
		e_pause,
		e_inventory}

	;

	public enum E_MenuState
	{
		e_menuUp,
		e_menuDown}

	;

	static E_STATES m_gameState = E_STATES.e_game;
	static E_MenuState m_menuState = E_MenuState.e_menuDown;

	public static void SetState (E_STATES state)
	{
		m_gameState = state;
	}

	public static E_STATES GetState ()
	{
		return m_gameState;
	}

	public static void SetMenuState (E_MenuState state)
	{
		m_menuState = state;
	}

	public static E_MenuState GetMenuState ()
	{
		return m_menuState;
	}
}

[SerializeField]
public struct item
{
	public sbyte id;
	public sbyte age;
	public GameObject GO;
}

[SerializeField]
public enum onHarvest
{
	//Carrots
	Destory,
	// Trees
	RegrowToZero,
	// Berries
	Renewable
}


