using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace LegacyNetworking
{
    public static partial class Network
    {
        public static string MainScene {
            get; set;
        }

        public static ushort NextId {
            get; set;
        }
        public static Dictionary<ushort, NetworkView> Views {
            get; set;
        } = new();
        public static int AllocateView(NetworkView view) {
            if (Views.ContainsValue(view)) {
                return view.viewId;
            }
            Views.Add(NextId, view);
            view.viewId = NextId;
            NextId++;
            return view.viewId;
        }

        public static NetworkView GetView(ushort viewId) {
            return Views[viewId];
        }
    }
}
