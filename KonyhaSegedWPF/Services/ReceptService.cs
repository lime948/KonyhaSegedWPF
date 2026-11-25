using KonyhaSegedWPF.ReceptModels;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows;
using System;

namespace KonyhaSegedWPF.Services
{
    public class ReceptService : DatabaseService
    {
        // =====================================================================
        //  RECEPTEK  –  READ
        // =====================================================================

        /// <summary>
        /// Visszaadja az összes receptet az adatbázisból.
        /// </summary>
        /// <returns>Egy Recept objektumokat tartalmazó listát</returns>
        public List<Recept> ReadReceptek()
        {
            // Tárolólista létrehozása
            List<Recept> receptek = new List<Recept>();

            // Receptek lekérése
            try
            {
                // Csatlakozás az adatbázishoz
                using (MySqlConnection myConnection = GetOpenConnection())
                {
                    // MySQL parancs létrehozása
                    using (MySqlCommand cmd = new MySqlCommand
                    {
                        Connection = myConnection,
                        CommandText = "SELECT recept_id, nev, leiras, elkeszitesi_ido, kategoria, allergenek FROM receptek;"
                    })
                    {
                        // Recept objektum létrehozása a beolvasott adatokból
                        using (MySqlDataReader myReader = cmd.ExecuteReader())
                        {
                            while (myReader.Read())
                            {
                                int recept_id = myReader.GetInt32("recept_id");
                                string nev = myReader.GetString("nev");
                                string leiras = myReader.GetString("leiras");
                                int elkeszitesi_ido = myReader.GetInt32("elkeszitesi_ido");
                                string kategoria = myReader.GetString("kategoria");

                                // Az allergenek oszlop NULL-t is tartalmazhat (DEFAULT NULL)
                                string allergenek = myReader.IsDBNull(myReader.GetOrdinal("allergenek"))
                                    ? null
                                    : myReader.GetString("allergenek");

                                receptek.Add(new Recept(recept_id, nev, leiras, elkeszitesi_ido, kategoria, allergenek));
                            }
                        }
                    }
                }
            }

            // Sikertelen lekérés esetén
            catch (MySqlException ex)
            {
                // Hibaüzenet kiírása
                MessageBox.Show($"Hiba történt a receptek lekérése során: {ex.Message}", "Adatbázis hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Lista visszaadása
            return receptek;
        }

        /// <summary>
        /// Visszaad egy receptet azonosító alapján, vagy null-t, ha nincs ilyen.
        /// </summary>
        public Recept ReadReceptById(int receptId)
        {
            // Recept lekérése
            try
            {
                // Csatlakozás az adatbázishoz
                using (MySqlConnection myConnection = GetOpenConnection())
                {
                    // MySQL parancs létrehozása paraméterrel
                    using (MySqlCommand cmd = new MySqlCommand
                    {
                        Connection = myConnection,
                        CommandText = "SELECT recept_id, nev, leiras, elkeszitesi_ido, kategoria, allergenek " +
                                      "FROM receptek WHERE recept_id = @recept_id;"
                    })
                    {
                        // Paraméter hozzáadása (SQL injection ellen védve)
                        cmd.Parameters.AddWithValue("@recept_id", receptId);

                        // Recept objektum létrehozása a beolvasott adatokból
                        using (MySqlDataReader myReader = cmd.ExecuteReader())
                        {
                            if (myReader.Read())
                            {
                                int id = myReader.GetInt32("recept_id");
                                string nev = myReader.GetString("nev");
                                string leiras = myReader.GetString("leiras");
                                int elkeszitesi_ido = myReader.GetInt32("elkeszitesi_ido");
                                string kategoria = myReader.GetString("kategoria");

                                // Az allergenek oszlop NULL-t is tartalmazhat
                                string allergenek = myReader.IsDBNull(myReader.GetOrdinal("allergenek"))
                                    ? null
                                    : myReader.GetString("allergenek");

                                return new Recept(id, nev, leiras, elkeszitesi_ido, kategoria, allergenek);
                            }
                        }
                    }
                }
            }

            // Sikertelen lekérés esetén
            catch (MySqlException ex)
            {
                // Hibaüzenet kiírása
                MessageBox.Show($"Hiba történt az recept lekérése során: {ex.Message}", "Adatbázis hiba", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Nincs találat
            return null;
        }

        // =====================================================================
        //  RECEPTEK  –  CREATE
        // =====================================================================

        /// <summary>
        /// Új recept beszúrása. Az új recept_id-t az adatbázis generálja (AUTO_INCREMENT).
        /// </summary>
        /// <returns>true, ha sikeres volt</returns>
        public bool CreateRecept(string nev, string leiras, int elkeszitesiIdo, string kategoria, string allergenek)
        {
            return ExecuteNonQuery(
                "INSERT INTO receptek (nev, leiras, elkeszitesi_ido, kategoria, allergenek) " +
                "VALUES (@nev, @leiras, @elkeszitesi_ido, @kategoria, @allergenek);",
                new MySqlParameter("@nev", nev),
                new MySqlParameter("@leiras", leiras),
                new MySqlParameter("@elkeszitesi_ido", elkeszitesiIdo),
                new MySqlParameter("@kategoria", kategoria),
                new MySqlParameter("@allergenek", (object)allergenek ?? DBNull.Value)) > 0;
        }

        // =====================================================================
        //  RECEPTEK  –  UPDATE
        // =====================================================================

        /// <summary>
        /// Meglévő recept módosítása.
        /// </summary>
        /// <returns>true, ha sikeres volt</returns>
        public bool UpdateRecept(int receptId, string nev, string leiras, int elkeszitesiIdo, string kategoria, string allergenek)
        {
            return ExecuteNonQuery(
                "UPDATE receptek SET nev = @nev, leiras = @leiras, elkeszitesi_ido = @elkeszitesi_ido, " +
                "kategoria = @kategoria, allergenek = @allergenek " +
                "WHERE recept_id = @recept_id;",
                new MySqlParameter("@recept_id", receptId),
                new MySqlParameter("@nev", nev),
                new MySqlParameter("@leiras", leiras),
                new MySqlParameter("@elkeszitesi_ido", elkeszitesiIdo),
                new MySqlParameter("@kategoria", kategoria),
                new MySqlParameter("@allergenek", (object)allergenek ?? DBNull.Value)) > 0;
        }

        // =====================================================================
        //  RECEPTEK  –  DELETE
        // =====================================================================

        /// <summary>
        /// Recept törlése azonosító alapján.
        /// A hozzavalok sorok a FOREIGN KEY CASCADE miatt automatikusan törlődnek.
        /// </summary>
        /// <returns>true, ha sikeres volt</returns>
        public bool DeleteRecept(int receptId)
        {
            // Recept törlése paraméterezett paranccsal
            return ExecuteNonQuery(
                "DELETE FROM receptek WHERE recept_id = @recept_id;",
                new MySqlParameter("@recept_id", receptId)) > 0;
        }
    }
}
