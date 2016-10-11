using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

namespace Devdog.InventorySystem
{
	[HelpURL ("http://devdog.nl/documentation/serialization-saving-loading/")]
	public abstract class CollectionSaverLoaderBase : SaverLoaderBase
	{
		[NonSerialized]
		private ItemCollectionBase _collection;

		protected ItemCollectionBase collection {
			get {
				if (_collection == null) {
					_collection = GetComponent<ItemCollectionBase> ();
				}

				return _collection;
			}
		}

		public override string saveName {
			get {
				return SaveNamePrefix + "Collection_" + collection.collectionName.ToLower ().Replace (" ", "_");
			}
		}

		public override void Save ()
		{
			try {
				SaveItems (serializer.SerializeCollection (collection), (bool saved) => {
					//Debug.Log ("Saved collection " + collection.collectionName);
				});
			} catch (SerializedObjectNotFoundException e) {
				Debug.LogWarning (e.Message + e.StackTrace);
			}
			//catch(Exception e)
			//{
			//    Debug.LogWarning(e.Message + e.StackTrace);                    
			//}
		}

		public override void Load ()
		{
			try {
				LoadItems ((object data) => {
					Debug.Log ("Loaded collection " + collection.collectionName);

					var model = serializer.DeserializeCollection (data);
					model.FillCollectionUsingThis (collection);
				});
			} catch (SerializedObjectNotFoundException e) {
				Debug.LogWarning (e.Message + e.StackTrace);
			}
			//catch(Exception e)
			//{
			//    Debug.LogWarning(e.Message + e.StackTrace);
			//}
		}

		//        public abstract void SaveItems(object serializedData, Action<bool> callback);
		//        public abstract void LoadItems(Action<object> callback);
	}
}
