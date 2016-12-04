using UnityEngine;
using System.Collections;

public static class GameEventManager
{
	public static Vector2 currentSelectedTilePosition = Vector2.zero;
	public const float baseStrengthWithTool = 1f;
	public static float baseStrengthWithoutTool = 2.5f;
	public static int numberOfislands = 0;
	public static float walkTowardsItemSafeDistance = 0.75f;
	public static float dropItemSize = 0.4f;
	public static float playerSpeedInDeepWater = 1.25f, playerSpeedInShallowWater = 1.75f;

	public delegate void GameEvent ();


	static E_STATES m_gameState = E_STATES.e_game;

	public static void SetState (E_STATES state)
	{
		m_gameState = state;
	}

	public static E_STATES GetState ()
	{
		return m_gameState;
	}

	public enum E_STATES
	{
		e_game,
		e_pause,
		e_inventory}

	;



	static E_MenuState m_menuState = E_MenuState.e_menuDown;

	public static void SetMenuState (E_MenuState state)
	{
		m_menuState = state;
	}

	public static E_MenuState GetMenuState ()
	{
		return m_menuState;
	}

	public enum E_MenuState
	{
		e_menuUp,
		e_menuDown}

	;


	static E_PlayerTerrianSTATES m_playerTerrianState = E_PlayerTerrianSTATES.land;

	public static void SetPlayerTerrianSTATES (E_PlayerTerrianSTATES state)
	{
		m_playerTerrianState = state;
	}

	public static E_PlayerTerrianSTATES GetPlayerTerrianSTATES ()
	{
		return m_playerTerrianState;
	}

	public enum E_PlayerTerrianSTATES
	{
		deepwater,
		water,
		sand,
		land,
		stone}

	;
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
	RegrowToStump,
	// Berries
	Renewable
}


