using System.Collections.Generic;
using LightGameServer.Database.Utils;
using MySql.Data.MySqlClient;

namespace LightGameServer.Database
{
    class QueryRepository
    {
        #region Singleton

        private static QueryRepository _instance;

        public static QueryRepository Get()
        {
            if (_instance == null) _instance = new QueryRepository();
            return _instance;
        }

        #endregion

        private readonly DBConnector _dbConnector;
        private readonly ModelCreator _modelCreator;

        public QueryRepository()
        {
            _dbConnector = new DBConnector();
            _modelCreator = new ModelCreator(this);
        }


        #region Wrappers

        private List<Dictionary<string, object>> ExecuteSelect(string query, params KeyValuePair<string, object>[] parameters)
        {
            List<Dictionary<string, object>> valueList = new List<Dictionary<string, object>>();
            using (MySqlConnection con = _dbConnector.OpenConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    foreach (var paramater in parameters)
                    {
                        cmd.Parameters.AddWithValue(paramater.Key, paramater.Value);
                    }

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Dictionary<string, object> values = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (values.ContainsKey(reader.GetName(i)))
                                {
                                    continue;
                                }
                                values.Add(reader.GetName(i), reader.GetValue(i));
                            }
                            valueList.Add(values);
                        }

                        return valueList;
                    }
                }
            }
        }

        private void ExecuteNonQuery(string query, params KeyValuePair<string, object>[] parameters)
        {
            using (MySqlConnection con = _dbConnector.OpenConnection())
            {
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    foreach (var paramater in parameters)
                    {
                        cmd.Parameters.AddWithValue(paramater.Key, paramater.Value);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }

        #endregion


        public PlayerData CreatePlayerData(string name)
        {
            if (name.Length > 30)
            {
                name = name.Substring(0, 30);
            }

            string query = @"INSERT INTO player_data (name) VALUES (?name);
                            SELECT LAST_INSERT_ID() as player_id";

            var reader = ExecuteSelect(query, Pair.Of("name", name));

            ulong newPlayerId = reader[0].Get<ulong>("player_id");

            query = @"INSERT INTO device_id_player_id (player_id) VALUES (?player_id);
                      SELECT LAST_INSERT_ID() as device_id;";

            reader = ExecuteSelect(query, Pair.Of("player_id", newPlayerId));
            ulong newDeviceId = reader[0].Get<ulong>("device_id");

            return GetPlayerData(Encryptor.EncryptDeviceId(newDeviceId));
        }

        public PlayerData GetPlayerData(string deviceId)
        {
            ulong decryptedDeviceId = Encryptor.DecryptDeviceId(deviceId);

            string query = @"SELECT *
                             FROM player_data 
                             JOIN device_id_player_id AS d
                             ON d.player_id=player_data.player_id
                             WHERE d.device_id=?device_id";

            var reader = ExecuteSelect(query, Pair.Of("device_id", decryptedDeviceId));
            return reader.Count > 0 ? _modelCreator.CreatePlayerData(reader[0]) : null;
        }
    }
}
