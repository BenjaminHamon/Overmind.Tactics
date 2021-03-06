﻿using Newtonsoft.Json;
using Overmind.Tactics.Model;
using System;
using System.IO;
using UnityEngine;

namespace Overmind.Tactics.UnityClient
{
	public class UnityDataProvider : GameDataProvider
	{
		public UnityDataProvider(JsonSerializer serializer, string contentDirectory, string userDirectory)
			: base(serializer, contentDirectory, userDirectory)
		{ }

		public static TAsset LoadAsset<TAsset>(string path) where TAsset : UnityEngine.Object
		{
			TAsset asset = Resources.Load<TAsset>(path);
			if (asset == null)
				throw new FileNotFoundException(String.Format("[UnityDataProvider] Resource not found {0}", path), path);
			return asset;
		}

		protected override TData LoadContent<TData>(string path)
		{
			TextAsset asset = LoadAsset<TextAsset>(path);
			using (StringReader stringReader = new StringReader(asset.text))
			using (JsonReader jsonReader = new JsonTextReader(stringReader))
				return serializer.Deserialize<TData>(jsonReader);
		}

		protected override void SaveContent<TData>(string path, TData data)
		{
			if (Application.isEditor == false)
				throw new InvalidOperationException(String.Format("[UnityDataProvider] Cannot save content when running outside the editor (Path: {0})", path));
			
			base.SaveContent(path, data);
		}
	}
}
