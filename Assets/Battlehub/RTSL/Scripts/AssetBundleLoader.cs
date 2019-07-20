using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Battlehub.RTSL
{
    public delegate void LoadAssetBundleHandler(AssetBundle bundle);
    public delegate void ListAssetBundlesHandler(string[] bundleNames);
    public interface IAssetBundleLoader
    {
        void GetAssetBundles(ListAssetBundlesHandler assetBundles);
        void Load(string name, LoadAssetBundleHandler callback);
    }

    public class AssetBundleLoader : IAssetBundleLoader
    {
        public void GetAssetBundles(ListAssetBundlesHandler callback)
        {
            List<string> result = new List<string>();

            if(Directory.Exists(Application.streamingAssetsPath))
            {
                string[] manifestFiles = Directory.GetFiles(Application.streamingAssetsPath, "*.manifest");
                for (int i = 0; i < manifestFiles.Length; ++i)
                {
                    string assetBundleFile = Path.GetDirectoryName(manifestFiles[i]) + "/" + Path.GetFileNameWithoutExtension(manifestFiles[i]);
                    if (File.Exists(assetBundleFile))
                    {
                        result.Add(Path.GetFileName(assetBundleFile));
                    }
                }
            }
            else
            {
                Debug.Log("StreamingAssets folder does not exists. No asset bundles to load.");
            }

            callback(result.ToArray());
        }

        public void Load(string bundleName, LoadAssetBundleHandler callback)
        {
            if (!File.Exists(Application.streamingAssetsPath + "/" + bundleName))
            {
                callback(null);
                return;
            }

            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(Application.streamingAssetsPath + "/" + bundleName);
            if(request.isDone)
            {
                AssetBundle bundle = request.assetBundle;
                if (callback != null)
                {
                    callback(bundle);
                }
            }
            else
            {
                Action<AsyncOperation> completed = null;
                completed = result =>
                {
                    AssetBundle bundle = request.assetBundle;
                    if (callback != null)
                    {
                        callback(bundle);
                    }
                    request.completed -= completed;
                };
                request.completed += completed;
            }
        }
    }
}