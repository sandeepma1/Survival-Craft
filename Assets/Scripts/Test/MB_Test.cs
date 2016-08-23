using UnityEngine;
using System.Collections;

public class MB_Test : MonoBehaviour
{
	public MessageTypee msgTyp;

	// Use this for initialization
	public void StartMsg ()
	{
		Message msg = new Message ();
		msg.Type = msgTyp;
		MessageBus.Instance.SendMessage (msg);
	}

}
