using Newtonsoft.Json;
using Overmind.Tactics.Model;
using System;
using System.IO;
using UnityEngine;

namespace Overmind.Tactics.UnityClient
{
	public class UnityContentProvider : ContentProvider
	{
		public UnityContentProvider(JsonSerializer serializer, string contentDirectory, string userDirectory)
			: base(serializer, contentDirectory, userDirectory)
		{ }

		protected override TData LoadContent<TData>(string path)
		{
			TextAsset asset = Resources.Load<TextAsset>(path);
			if (asset == null)
				throw new FileNotFoundException(String.Format("[UnityContentProvider] Resource not found {0}", path), path);

			using (StringReader stringReader = new StringReader(asset.text))
			using (JsonReader jsonReader = new JsonTextReader(stringReader))
				return serializer.Deserialize<TData>(jsonReader);
		}

		protected override void SaveContent<TData>(string path, TData data)
		{
			if (Application.isEditor == false)
				throw new InvalidOperationException(String.Format("[UnityContentProvider] Cannot save content when running outside the editor (Path: {0})", path));
			
			base.SaveContent(path, data);
		}
	}
}
