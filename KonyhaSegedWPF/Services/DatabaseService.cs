using MySql.Data.MySqlClient;
using System.Windows;

namespace KonyhaSegedWPF.Services
{
    public class DatabaseService
    {
        // =====================================================================
        //  KAPCSOLAT
        // =====================================================================

        private const string ConnectionString = "server=localhost;uid=root;pwd=;database=konyhaseged";

        /// <summary>
        /// Létrehoz és megnyit egy új MySQL kapcsolatot.
        /// A hívó felelőssége a using/Dispose.
        /// </summary>
        protected MySqlConnection GetOpenConnection()
        {
            try
            {
                MySqlConnection myConnection = new MySqlConnection(ConnectionString);
                myConnection.Open();
                return myConnection;
            }
            catch (System.Exception)
            {
                MessageBox.Show("Hiba történt az adatbázis kapcsolat létrehozása során. Ellenőrizze a kapcsolat beállításait.", "Adatbázis hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                throw;
            }
        }

        /// <summary>
        /// Végrehajt egy nem-lekérdező parancsot (INSERT/UPDATE/DELETE)
        /// a megadott paraméterekkel, és visszaadja az érintett sorok számát.
        /// </summary>
        /// <returns>Az érintett sorok száma, hiba esetén -1</returns>
        protected int ExecuteNonQuery(string commandText, params MySqlParameter[] parameters)
        {
            try
            {
                // Csatlakozás az adatbázishoz
                using (MySqlConnection myConnection = GetOpenConnection())
                {
                    // MySQL parancs létrehozása paraméterekkel (SQL injection ellen védve)
                    using (MySqlCommand cmd = new MySqlCommand
                    {
                        Connection = myConnection,
                        CommandText = commandText
                    })
                    {
                        // Paraméterek hozzáadása (@parameter placeholder-ekhez)
                        if (parameters != null && parameters.Length > 0)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }

                        // Parancs végrehajtása
                        return cmd.ExecuteNonQuery();
                    }
                }
            }

            // Sikertelen művelet esetén
            catch (MySqlException ex)
            {
                // Hibaüzenet kiírása
                MessageBox.Show($"Hiba történt az adatbázis művelet során: {ex.Message}", "Adatbázis hiba", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
        }
    }
}