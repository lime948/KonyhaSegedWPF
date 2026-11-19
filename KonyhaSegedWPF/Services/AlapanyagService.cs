using KonyhaSegedWPF.ReceptModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace KonyhaSegedWPF.Services
{
    public class AlapanyagService : DatabaseService
    {
        // =====================================================================
        //  ALAPANYAGOK  –  READ
        // =====================================================================

        /// <summary>
        /// Visszaadja az összes alapanyagot az adatbázisból.
        /// </summary>
        /// <returns>Egy Alapanyag objektumokat tartalmazó listát</returns>
        public List<Alapanyag> ReadAlapanyagok()
        {
            // Tárolólista létrehozása
            List<Alapanyag> alapanyagok = new List<Alapanyag>();

            // Alapanyagok lekérése
            try
            {
                // Csatlakozás az adatbázishoz
                using (MySqlConnection myConnection = GetOpenConnection())
                {
                    // MySQL parancs létrehozása
                    using (MySqlCommand cmd = new MySqlCommand
                    {
                        Connection = myConnection,
                        CommandText = "SELECT alapanyag_id, nev FROM alapanyagok;"
                    })
                    {
                        // Alapanyag objektum létrehozása a beolvasott adatokból
                        using (MySqlDataReader myReader = cmd.ExecuteReader())
                        {
                            while (myReader.Read())
                            {
                                int alapanyag_id = myReader.GetInt32("alapanyag_id");
                                string nev = myReader.GetString("nev");
                                alapanyagok.Add(new Alapanyag(alapanyag_id, nev));
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
            return alapanyagok;
        }

        /// <summary>
        /// Visszaad egy alapanyagot azonosító alapján, vagy null-t, ha nincs ilyen.
        /// </summary>
        public Alapanyag ReadAlapanyagById(int alapanyagId)
        {
            // Alapanyag lekérése
            try
            {
                // Csatlakozás az adatbázishoz
                using (MySqlConnection myConnection = GetOpenConnection())
                {
                    // MySQL parancs létrehozása paraméterrel
                    using (MySqlCommand cmd = new MySqlCommand
                    {
                        Connection = myConnection,
                        CommandText = "SELECT alapanyag_id, nev FROM alapanyagok WHERE alapanyag_id = @alapanyag_id;"
                    })
                    {
                        // Paraméter hozzáadása (SQL injection ellen védve)
                        cmd.Parameters.AddWithValue("@alapanyag_id", alapanyagId);

                        // Alapanyag objektum létrehozása a beolvasott adatokból
                        using (MySqlDataReader myReader = cmd.ExecuteReader())
                        {
                            if (myReader.Read())
                            {
                                int id = myReader.GetInt32("alapanyag_id");
                                string nev = myReader.GetString("nev");
                                return new Alapanyag(id, nev);
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

            // Nincs találat
            return null;
        }

        // =====================================================================
        //  ALAPANYAGOK  –  CREATE
        // =====================================================================

        /// <summary>
        /// Új alapanyag beszúrása.
        /// </summary>
        /// <returns>true, ha sikeres volt</returns>
        public bool CreateAlapanyag(string nev)
        {
            // Alapanyag beszúrása paraméterezett paranccsal
            return ExecuteNonQuery(
                "INSERT INTO alapanyagok (nev) VALUES (@nev);",
                new MySqlParameter("@nev", nev)) > 0;
        }

        // =====================================================================
        //  ALAPANYAGOK  –  UPDATE
        // =====================================================================

        /// <summary>
        /// Meglévő alapanyag nevének módosítása.
        /// </summary>
        /// <returns>true, ha sikeres volt</returns>
        public bool UpdateAlapanyag(int alapanyagId, string nev)
        {
            // Alapanyag módosítása paraméterezett paranccsal
            return ExecuteNonQuery(
                "UPDATE alapanyagok SET nev = @nev WHERE alapanyag_id = @alapanyag_id;",
                new MySqlParameter("@nev", nev),
                new MySqlParameter("@alapanyag_id", alapanyagId)) > 0;
        }

        // =====================================================================
        //  ALAPANYAGOK  –  DELETE
        // =====================================================================

        /// <summary>
        /// Alapanyag törlése azonosító alapján.
        /// A hozzavalok sorok a FOREIGN KEY CASCADE miatt automatikusan törlődnek.
        /// </summary>
        /// <returns>true, ha sikeres volt</returns>
        public bool DeleteAlapanyag(int alapanyagId)
        {
            // Alapanyag törlése paraméterezett paranccsal
            return ExecuteNonQuery(
                "DELETE FROM alapanyagok WHERE alapanyag_id = @alapanyag_id;",
                new MySqlParameter("@alapanyag_id", alapanyagId)) > 0;
        }
    }
}
