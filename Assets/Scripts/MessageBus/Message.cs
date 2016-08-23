using UnityEngine;

public enum MessageTypee
{
	NONE,
	LevelStart,
	LevelEnd,
	PlayerPosition,
	PointAdded
}

public struct Message
{
	public MessageTypee Type;
	public int IntValue;
	public float FloatValue;
	public Vector3 Vector3Value;
	public GameObject GameObjectValue;
}