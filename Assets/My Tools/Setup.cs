using UnityEditor;
using UnityEngine;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace MyTools
{
    public static class Setup
    {
        [MenuItem("Tools/Setup/Create Default Folders")]
        public static void CreateDefaultFolders()
        {
            Folders.CreateDefault("_Project", "Animation", "Art", "Materials", "Prefabs", "Scripts/ScriptableObjects", "Scripts/UI");
            Folders.CreateDefault("External");
            Refresh();
        }

        [MenuItem("Tools/Setup/Import My Favorite Assets")]
        public static void ImportMyFavoriteAsset()
        {
            Assets.ImportAsset("DOTween HOTween v2.unitypackage", "Demigiant/Editor ExtensionsAnimation");
            Assets.ImportAsset("vFavorites 2.unitypackage", "kubacho lab/Editor ExtensionsUtilities");
            Assets.ImportAsset("Scenes Switcher.unitypackage", "GameDev Tools/Editor ExtensionsUtilities");
            Assets.ImportAsset("Replace Selected.unitypackage", "Staggart Creations/Editor ExtensionsUtilities");
            Assets.ImportAsset("Selection History.unitypackage", "Staggart Creations/Editor ExtensionsUtilities");
        }

        [MenuItem("Tools/Setup/Install Netcode for GameObjects")]
        public static void InstallNetcodeForGameObjects()
        {
            Packages.InstallPackages(new[]
            {
                "com.unity.multiplayer.tools",
                "com.unity.netcode.gameobjects"
            });
        }

        [MenuItem("Tools/Setup/Install Unity AI Navigation")]
        public static void InstallUnityAINavigation()
        {
            Packages.InstallPackages(new[]
            {
                "com.unity.ai.navigation"
            });
        }

        [MenuItem("Tools/Setup/Install My Favorite Open Source")]
        public static void InstallOpenSource()
        {
            Packages.InstallPackages(new[]
            {
                "git+https://github.com/KyleBanks/scene-ref-attribute",
                //"git+https://github.com/starikcetin/Eflatun.SceneReference.git#3.1.1"
            });
        }

        static class Folders
        {
            public static void CreateDefault(string root, params string[] folders)
            {
                var fullpath = Path.Combine(Application.dataPath, root);
                if (!Directory.Exists(fullpath))
                {
                    Directory.CreateDirectory(fullpath);
                }
                foreach (var folder in folders)
                {
                    CreateSubFolders(fullpath, folder);
                }
            }

            private static void CreateSubFolders(string rootPath, string folderHierarchy)
            {
                var folders = folderHierarchy.Split('/');
                var currentPath = rootPath;
                foreach (var folder in folders)
                {
                    currentPath = Path.Combine(currentPath, folder);
                    if (!Directory.Exists(currentPath))
                    {
                        Directory.CreateDirectory(currentPath);
                    }
                }
            }
        }

        static class Packages
        {
            static AddRequest _request;
            static readonly Queue<string> PackagesToInstall = new();

            public static void InstallPackages(string[] packages)
            {
                foreach (var package in packages)
                {
                    PackagesToInstall.Enqueue(package);
                }

                // Start the installation of the first package
                if (PackagesToInstall.Count > 0)
                {
                    _request = Client.Add(PackagesToInstall.Dequeue());
                    EditorApplication.update += Progress;
                }
            }

            static async void Progress()
            {
                    if (_request.IsCompleted)
                    {
                        if (_request.Status == StatusCode.Success)
                            Debug.Log("Installed: " + _request.Result.packageId);
                        else if (_request.Status >= StatusCode.Failure)
                            Debug.Log(_request.Error.message);

                        EditorApplication.update -= Progress;

                        // If there are more packages to install, start the next one
                        if (PackagesToInstall.Count > 0)
                        {
                            // Add delay before next package install
                            await Task.Delay(1000);
                            _request = Client.Add(PackagesToInstall.Dequeue());
                            EditorApplication.update += Progress;
                        }
                    }
            }
        }

        static class Assets
        {
            public static void ImportAsset(
                string asset, string subfolder
                , string folder = "/Users/macmini/Library/Unity/Asset Store-5.x"
            )
            {
                ImportPackage(Combine(folder, subfolder, asset), false);
            }
        }
    }
}