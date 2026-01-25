using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace LegacyNetworking
{
    public static partial class Network
    {
        public static Action<string, string, LoadSceneMode> SceneLoaded;
        public static KeyValuePair<string, string> MainScene {
            get; set;
        }
        public static Dictionary<string, string> LoadedScenes = new();


        public static ushort NextId {
            get; set;
        }
        public static Dictionary<ushort, NetworkView> Views {
            get; set;
        }
        public static int AllocateView(NetworkView view) {
            Views.Add(NextId, view);
            view.ViewID = NextId;
            NextId++;
            return view.ViewID;
        }

        public static NetworkView GetView(ushort viewId) {
            return Views[viewId];
        }
    }
}
