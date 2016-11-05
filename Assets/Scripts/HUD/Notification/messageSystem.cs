using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class messageSystem : MonoBehaviour
{
		public int maxLines = 8;
		private Queue<string> queue = new Queue<string> ();
		private string Mytext = "";
		public TextMesh T1;
		float timer = 0;
		public float fadeOutTime;
	
		public void NewMessage (string message)
		{
				if (queue.Count >= maxLines)
						queue.Dequeue ();		
				queue.Enqueue (message);		
				Mytext = "";
				foreach (string st in queue)
						Mytext = Mytext + st + "\n";

		}
		/*void OnGUI ()
		{
				GUI.Label (new Rect (5, // x, left offset
		                   (Screen.height - 150), // y, bottom offset
		                   300f, // width
		                   150f), Mytext, GUI.skin.textArea); // height, text, Skin features}
		}*/
		void Update ()
		{				
				if (Input.GetKeyDown (KeyCode.Space)) {
						NewMessage ("Some" + Random.Range (0, 100));
						T1.text = Mytext;
						timer = 0;
				}
				if (T1.text != "") {
						timer += Time.deltaTime;
				}	
				print (timer);
				if (timer > fadeOutTime) {
						timer = 0;
						T1.text = "";
						queue.Clear ();
				}
		}
}
