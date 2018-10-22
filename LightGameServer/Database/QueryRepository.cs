using System.Collections.Generic;
using LightEngineSerializeable.SerializableClasses.DatabaseModel;
using LightGameServer.Database.Utils;
using LightGameServer.Game.Model;
using MySql.Data.MySqlClient;

namespace LightGameServer.Database
{
    class QueryRepository
    {
        private readonly DbConnector _dbConnector;
        private readonly ModelCreator _modelCreator;
        private readonly int[] _defaultUnits = { 1, 2, 3, 4 };

        public QueryRepository()
        {
            _dbConnector = new DbConnector();
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

            var values = ExecuteSelect(query, Pair.Of("name", name));

            uint newPlayerId = values[0].Get<uint>("player_id");

            query = @"INSERT INTO device_id_player_id (player_id) VALUES (?player_id);
                      SELECT LAST_INSERT_ID() as device_id;";

            values = ExecuteSelect(query, Pair.Of("player_id", newPlayerId));
            uint newDeviceId = values[0].Get<uint>("device_id");

            query = @"INSERT INTO player_unit (player_id,unit_id) VALUES (?player_id,?unit_id)";
            foreach (var defaultUnit in _defaultUnits)
            {
                ExecuteNonQuery(query, Pair.Of("player_id", newPlayerId), Pair.Of("unit_id", defaultUnit));
            }

            query = @"INSERT INTO player_deck (player_id,unit_one_id,unit_two_id,unit_three_id,unit_four_id)
                      VALUES (?player_id,?unit_one_id,?unit_two_id,?unit_three_id,?unit_four_id);";
            ExecuteNonQuery(query,
                            Pair.Of("player_id", newPlayerId),
                            Pair.Of("unit_one_id", _defaultUnits[0]),
                            Pair.Of("unit_two_id", _defaultUnits[1]),
                            Pair.Of("unit_three_id", _defaultUnits[2]),
                            Pair.Of("unit_four_id", _defaultUnits[3]));


            return GetPlayerData(Encryptor.EncryptDeviceId(newDeviceId));
        }

        public PlayerData GetPlayerData(string deviceId)
        {
            uint decryptedDeviceId = Encryptor.DecryptDeviceId(deviceId);

            string query = @"SELECT *
                             FROM player_data 
                             JOIN device_id_player_id AS d
                             ON d.player_id=player_data.player_id
                             WHERE d.device_id=?device_id
                             LIMIT 1";

            var values = ExecuteSelect(query, Pair.Of("device_id", decryptedDeviceId));
            return values.Count > 0 ? _modelCreator.CreatePlayerData(values[0]) : null;
        }

        public UnitSettings GetUnitSettingsByName(string unitName)
        {
            string query = @"SELECT u.id,name,density,radius,push_force,hp,damage
                             FROM units AS u
                             INNER JOIN weight AS w ON u.weight=w.id
                             INNER JOIN size AS sz ON u.size=sz.id
                             INNER JOIN speed AS sp ON u.speed=sp.id
                             WHERE u.name=?name 
                             LIMIT 1;";

            var values = ExecuteSelect(query, Pair.Of("name", unitName));
            return values.Count > 0 ? _modelCreator.CreateUnitSettings(values[0]) : null;
        }

        public UnitSettings[] GetAllUnitSettings()
        {
            string query = @"SELECT u.id,name,density,radius,push_force,hp,damage,projectile_damage
                             FROM unit AS u
                             INNER JOIN weight AS w ON u.weight=w.id
                             INNER JOIN size AS sz ON u.size=sz.id
                             INNER JOIN speed AS sp ON u.speed=sp.id;";
            var values = ExecuteSelect(query);
            List<UnitSettings> units = new List<UnitSettings>();
            foreach (var value in values)
            {
                units.Add(_modelCreator.CreateUnitSettings(value));
            }

            return units.ToArray();
        }

        public SkillSettings[] GetAllSkillSettings()
        {
            string query = @"SELECT *
                             FROM skill";

            var values = ExecuteSelect(query);
            List<SkillSettings> skills = new List<SkillSettings>();
            foreach (var value in values)
            {
                skills.Add(_modelCreator.CreateSkillSettings(value));
            }

            return skills.ToArray();
        }

        public PlayerUnit[] GetPlayerUnits(uint playerId)
        {
            List<PlayerUnit> playerUnits = new List<PlayerUnit>();
            string query = @"SELECT t.unit_id, t.amount
                            FROM player_unit t
                            WHERE t.player_id = ?player_id";
            var values = ExecuteSelect(query, Pair.Of("player_id", playerId));
            foreach (var value in values)
            {
                playerUnits.Add(_modelCreator.CreatePlayerUnit(value));
            }

            return playerUnits.ToArray();
        }

        public PlayerDeck GetPlayerDeck(uint playerId)
        {
            string query = @"SELECT unit_one_id,unit_two_id,unit_three_id,unit_four_id
                             FROM player_deck WHERE player_id=?player_id LIMIT 1;";

            var values = ExecuteSelect(query, Pair.Of("player_id", playerId));
            return _modelCreator.CreatePlayerDeck(values[0]);
        }
    }
}
