namespace KonyhaSegedWPF.ReceptModels
{
    /// <summary>
    /// Alapanyag osztály az `alapanyagok` tábla alapján
    /// </summary>
    public class Alapanyag
    {
        /// <summary>
        /// Az alapanyag azonosítója
        /// </summary>
        private int alapanyag_id;

        /// <summary>
        /// Az alapanyag neve
        /// </summary>
        private string nev;

        /// <summary>
        /// Az alapanyag azonosítója
        /// </summary>
        public int Alapanyag_id { get => alapanyag_id; set => alapanyag_id = value; }

        /// <summary>
        /// Az alapanyag neve
        /// </summary>
        public string Nev { get => nev; set => nev = value; }

        /// <summary>
        /// Alapanyag konstruktora paraméterekkel
        /// </summary>
        /// <param name="alapanyag_id">Az alapanyag azonosítója</param>
        /// <param name="nev">Az alapanyag neve</param>
        public Alapanyag(int alapanyag_id, string nev)
        {
            Alapanyag_id = alapanyag_id;
            Nev = nev;
        }

        /// <summary>
        /// Alapanyag konstruktora paraméterek nélkül
        /// </summary>
        public Alapanyag()
        {

        }

        /// <summary>
        /// Egy formázott stringet ad vissza az alapanyag adataival
        /// </summary>
        /// <returns>Egy formázott stringet</returns>
        public override string ToString()
        {
            return $"{Alapanyag_id}, {Nev}\n";
        }
    }
}
