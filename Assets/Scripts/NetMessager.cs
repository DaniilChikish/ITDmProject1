using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace ITDmProject
{
    public class NetMessager : NetworkBehaviour
    {
        protected const int PutWord = 451;
        protected const int OperationRequest = 452;
        protected const int WordListData = 453;
        protected const int StopListData = 454;
        protected const int SettingsData = 455;
        public enum Operation { Settings, StopList, WordList };
    }
}