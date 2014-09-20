﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Timers;

using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Common.ObjectBuilders.Voxels;
using Sandbox.Common.ObjectBuilders.VRageData;

using SEModAPI.API;
using SEModAPI.API.Definitions;
using SEModAPI.Support;

using SEModAPIInternal.API;
using SEModAPIInternal.API.Common;
using SEModAPIInternal.API.Entity;
using SEModAPIInternal.API.Entity.Sector;
using SEModAPIInternal.API.Server;
using SEModAPIInternal.Support;

using VRage.Common.Utils;
using SEModAPIExtensions.API.IPC;
using System.ServiceModel.Web;
using System.IdentityModel.Selectors;
using System.ServiceModel.Security;

namespace SEModAPIExtensions.API
{
    class Permission
    {


        

        #region "Attributes"

        private int m_permRank;
        private string m_permName;
        private List<ulong> m_playerList;

        #endregion

        #region "Constructors and Initializers"

        public Permission(string name, int rank)
        {
            m_permName = name;
            m_permRank = rank;
            m_playerList = new List<ulong>();
        }

        #endregion

        #region "Properties"

        public int RankInteger
        {
            get
            {
                return m_permRank;
            }
        }

        public string Name
        {
            get
            {
                return m_permName;
            }
        }

        public List<ulong> PlayerSIDList
        {
            get
            {
                return m_playerList;
            }
        }

        public List<PlayerMap.InternalPlayerItem> PlayerList
        {
            get
            {
                List<PlayerMap.InternalPlayerItem> plys = new List<PlayerMap.InternalPlayerItem>();

                foreach (ulong plysid in m_playerList)
                {
                    long pid = PlayerMap.Instance.GetPlayerIdsFromSteamId(plysid, false)[0];
                    Object plyObj = PlayerMap.Instance.GetPlayerItemFromPlayerId(pid);
                    PlayerMap.InternalPlayerItem plyItem = new PlayerMap.InternalPlayerItem(plyObj);
                    plys.Add(plyItem);
                }

                return plys;
            }
        }

        #endregion

        #region "Methods"

        public bool AddPlayer(Object ply)
        {
            ulong plySid;
            if (ply is long)
            {
                plySid = PlayerMap.Instance.GetPlayerItemFromPlayerId((long)ply).SteamId;
            }
            if (ply is ulong)
            {
                plySid = (ulong)ply;
            }
            else
            {
                PlayerMap.InternalPlayerItem plyObj = new PlayerMap.InternalPlayerItem(ply);
                plySid = plyObj.steamId;
            }
            m_playerList.Add(plySid);
            return this.HasPlayer(plySid);
        }

        public bool RemovePlayer(Object ply)
        {
            ulong plySid;
            if (ply is long)
            {
                plySid = PlayerMap.Instance.GetPlayerItemFromPlayerId((long)ply).SteamId;
            }
            if (ply is ulong)
            {
                plySid = (ulong)ply;
            }
            else
            {
                PlayerMap.InternalPlayerItem plyObj = new PlayerMap.InternalPlayerItem(ply);
                plySid = plyObj.steamId;
            }
            return m_playerList.Remove(plySid) && !this.HasPlayer(plySid);
        }

        public bool HasPlayer(Object ply)
        {
            ulong plySid;
            if (ply is long)
            {
                plySid = PlayerMap.Instance.GetPlayerItemFromPlayerId((long)ply).SteamId;
            }
            if (ply is ulong)
            {
                plySid = (ulong)ply;
            }
            else
            {
                PlayerMap.InternalPlayerItem plyObj = new PlayerMap.InternalPlayerItem(ply);
                plySid = plyObj.steamId;
            }

            if (plySid == null) return false;
            return m_playerList.Contains(plySid);
        }

        public bool ChangePlayerPermission(Object ply, Permission perm)
        {
            if (RemovePlayer(ply) == null)
            {
                return false;
            } 
            else
            {
                perm.AddPlayer(ply);
                return perm.HasPlayer(ply);
            }
        }

        #endregion
    }

    class PermissionManager
    {
        #region "Attributes"

        private static PermissionManager m_instance;

        private List<Permission> m_permList;
        private int m_adminRank;

        #endregion

        #region "Constructors and Initializers"

        protected PermissionManager()
        {
            m_instance = this;

            m_permList = new List<Permission>();

            Console.WriteLine("Finished loading PermissionManager");
        }

        #endregion

        #region "Properties"

        public static PermissionManager Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new PermissionManager();

                return m_instance;
            }
        }

        public List<Permission> PermissionList
        {
            get
            {
                return m_permList;
            }
        }

        public int AdminRank
        {
            get
            {
                return m_adminRank;
            }
        }

        #endregion

        #region "Methods"

        public List<ulong> GetAdmins()
        {
            return new List<ulong>();
        }

        #endregion
    }
}
