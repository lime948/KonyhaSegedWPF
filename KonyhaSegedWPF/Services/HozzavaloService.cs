using KonyhaSegedWPF.ReceptModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace KonyhaSegedWPF.Services
{
    public class HozzavaloService : DatabaseService
    {
        // =====================================================================
        //  HOZZÁVALÓK  –  READ
        // =====================================================================

        /// <summary>
        /// Visszaadja az összes hozzávalót az adatbázisból.
        /// </summary>
        /// <returns>Egy Hozzavalo objektumokat tartalmazó listát</returns>
        public List<Hozzavalo> ReadHozzavalok()
        {
            // Tárolólista létrehozása
            List<Hozzavalo> hozzavalok = new List<Hozzavalo>();

            // Hozzavalok lekérése
            try
            {
                // Csatlakozás az adatbázishoz
                using (MySqlConnection myConnection = GetOpenConnection())
                {
                    // MySQL parancs létrehozása
                    using (MySqlCommand cmd = new MySqlCommand
                    {
                        Connection = myConnection,
                        CommandText = "SELECT recept_id, alapanyag_id, mennyiseg FROM hozzavalok;"
                    })
                    {
                        // Hozzavalo objektum létrehozása a beolvasott adatokból
                        using (MySqlDataReader myReader = cmd.ExecuteReader())
                        {
                            while (myReader.Read())
                            {
                                int recept_id = myReader.GetInt32("recept_id");
                                int alapanyag_id = myReader.GetInt32("alapanyag_id");
                                string mennyiseg = myReader.GetString("mennyiseg");

                                hozzavalok.Add(new Hozzavalo(recept_id, alapanyag_id, mennyiseg));
                            }
                        }
                    }
                }
            }

            // Sikertelen lekérés esetén
            catch (MySqlException ex)
            {
                // Hibaüzenet kiírása
                Console.WriteLine(ex.Message);
            }

            // Lista visszaadása
            return hozzavalok;
        }

        /// <summary>
        /// Visszaadja egy adott recept összes hozzávalóját.
        /// </summary>
        public List<Hozzavalo> ReadHozzavalokByRecept(int receptId)
        {
            // Tárolólista létrehozása
            List<Hozzavalo> hozzavalok = new List<Hozzavalo>();

            // Hozzavalok lekérése recept szerint
            try
            {
                // Csatlakozás az adatbázishoz
                using (MySqlConnection myConnection = GetOpenConnection())
                {
                    // MySQL parancs létrehozása paraméterrel
                    using (MySqlCommand cmd = new MySqlCommand
                    {
                        Connection = myConnection,
                        CommandText = "SELECT recept_id, alapanyag_id, mennyiseg FROM hozzavalok WHERE recept_id = @recept_id;"
                    })
                    {
                        // Paraméter hozzáadása (SQL injection ellen védve)
                        cmd.Parameters.AddWithValue("@recept_id", receptId);

                        // Hozzavalo objektum létrehozása a beolvasott adatokból
                        using (MySqlDataReader myReader = cmd.ExecuteReader())
                        {
                            while (myReader.Read())
                            {
                                int r_id = myReader.GetInt32("recept_id");
                                int a_id = myReader.GetInt32("alapanyag_id");
                                string mennyiseg = myReader.GetString("mennyiseg");

                                hozzavalok.Add(new Hozzavalo(r_id, a_id, mennyiseg));
                            }
                        }
                    }
                }
            }

            // Sikertelen lekérés esetén
            catch (MySqlException ex)
            {
                // Hibaüzenet kiírása
                Console.WriteLine(ex.Message);
            }

            // Lista visszaadása
            return hozzavalok;
        }

        // =====================================================================
        //  HOZZÁVALÓK  –  CREATE
        // =====================================================================

        /// <summary>
        /// Új hozzávaló kapcsolat beszúrása egy recepthez.
        /// </summary>
        /// <returns>true, ha sikeres volt</returns>
        public bool CreateHozzavalo(int receptId, int alapanyagId, string mennyiseg)
        {
            // Hozzavalo beszúrása paraméterezett paranccsal
            return ExecuteNonQuery(
                "INSERT INTO hozzavalok (recept_id, alapanyag_id, mennyiseg) " +
                "VALUES (@recept_id, @alapanyag_id, @mennyiseg);",
                new MySqlParameter("@recept_id", receptId),
                new MySqlParameter("@alapanyag_id", alapanyagId),
                new MySqlParameter("@mennyiseg", mennyiseg)) > 0;
        }

        // =====================================================================
        //  HOZZÁVALÓK  –  UPDATE
        // =====================================================================

        /// <summary>
        /// Hozzávaló mennyiségének módosítása.
        /// A hozzavalok táblának nincs saját kulcsa, ezért a (recept_id, alapanyag_id)
        /// párost használjuk azonosításra.
        /// </summary>
        /// <returns>true, ha sikeres volt</returns>
        public bool UpdateHozzavalo(int receptId, int alapanyagId, string mennyiseg)
        {
            // Hozzavalo módosítása paraméterezett paranccsal
            return ExecuteNonQuery(
                "UPDATE hozzavalok SET mennyiseg = @mennyiseg " +
                "WHERE recept_id = @recept_id AND alapanyag_id = @alapanyag_id;",
                new MySqlParameter("@mennyiseg", mennyiseg),
                new MySqlParameter("@recept_id", receptId),
                new MySqlParameter("@alapanyag_id", alapanyagId)) > 0;
        }

        // =====================================================================
        //  HOZZÁVALÓK  –  DELETE
        // =====================================================================

        /// <summary>
        /// Hozzávaló kapcsolat törlése.
        /// </summary>
        /// <returns>true, ha sikeres volt</returns>
        public bool DeleteHozzavalo(int receptId, int alapanyagId)
        {
            // Hozzavalo törlése paraméterezett paranccsal
            return ExecuteNonQuery(
                "DELETE FROM hozzavalok " +
                "WHERE recept_id = @recept_id AND alapanyag_id = @alapanyag_id;",
                new MySqlParameter("@recept_id", receptId),
                new MySqlParameter("@alapanyag_id", alapanyagId)) > 0;
        }
    }
}
