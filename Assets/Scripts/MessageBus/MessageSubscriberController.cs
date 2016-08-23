using UnityEngine;
using System.Collections;

public class MessageSubscriberController : MonoBehaviour
{
	public MessageTypee[] MessageTypes;
	public MessageHandler Handler;

	void Start ()
	{
		MessageSubscriber subscriber = new MessageSubscriber ();
		subscriber.MessageTypes = MessageTypes;
		subscriber.Handler = Handler;

		MessageBus.Instance.AddSubscriber (subscriber);
	}
}
