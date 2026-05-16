using System.Collections.Generic;
using UnityEngine;

namespace MuPegaso.Client.Data
{
    public enum ServerStatus
    {
        Available,
        Full,
        Saturated,
        Maintenance
    }

    [System.Serializable]
    public class ServerData
    {
        public int id;
        public string name;
        public ServerStatus status;
        public bool isNew;
        public int groupId;
    }

    /// <summary>Catálogo MU PEGASO con distribución por grupo.</summary>
    public static class ServerDataCatalog
    {
        public static List<ServerData> BuildDefault()
        {
            var list = new List<ServerData>();
            AddGroup(list, 1, 1, 13);
            AddGroup(list, 2, 14, 22);
            AddGroup(list, 3, 23, 30);
            return list;
        }

        static void AddGroup(List<ServerData> list, int groupId, int start, int end)
        {
            var total = end - start + 1;
            var rest = total - 1;
            var nFull = Mathf.FloorToInt(rest * 0.6f);
            var nSat = Mathf.FloorToInt(rest * 0.2f);
            var nMaint = rest - nFull - nSat;

            var pool = new List<ServerStatus>();
            for (var i = 0; i < nFull; i++) pool.Add(ServerStatus.Full);
            for (var i = 0; i < nSat; i++) pool.Add(ServerStatus.Saturated);
            for (var i = 0; i < nMaint; i++) pool.Add(ServerStatus.Maintenance);
            Shuffle(pool);

            var pi = 0;
            for (var num = start; num <= end; num++)
            {
                if (num == end)
                {
                    list.Add(new ServerData
                    {
                        id = num,
                        name = $"MU PEGASO {num}",
                        status = ServerStatus.Available,
                        isNew = true,
                        groupId = groupId
                    });
                }
                else
                {
                    list.Add(new ServerData
                    {
                        id = num,
                        name = $"MU PEGASO {num}",
                        status = pool[pi++],
                        isNew = false,
                        groupId = groupId
                    });
                }
            }
        }

        static void Shuffle(List<ServerStatus> list)
        {
            for (var i = list.Count - 1; i > 0; i--)
            {
                var j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
