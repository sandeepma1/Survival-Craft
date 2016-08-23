using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreTextHandler : MessageHandler
{
	public override void HandleMessage (Message message)
	{
		switch (message.Type) {
			case MessageTypee.LevelStart:
				{
					print ("levelStart");
				}
				break;
			default:
				break;
		}
	}
}
