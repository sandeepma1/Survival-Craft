using UnityEngine;
using System.Collections;

public class Struct2dArray : MonoBehaviour
{
	item[,] i;
	// Use this for initialization
	/*void Start ()
	{
		i = new item[5, 5];
		i [0, 1].id = 5;
		i [0, 1].age = 10;
		ES2.Save (i, "iTest.txt");
		StartCoroutine ("readText");
	}

	IEnumerator readText ()
	{
		yield return new WaitForSeconds (1);
		item[,] a = ES2.Load<item[,]> ("iTest.txt");
		print (a [0, 1].age);
	}*/

}

public struct item1
{
	public int id;
	public int age;
}
