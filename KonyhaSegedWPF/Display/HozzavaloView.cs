namespace KonyhaSegedWPF.ReceptModels
{
    /// <summary>
    /// Megjelenítési segédosztály a hozzavalok DataGridhez (nevekkel).
    /// </summary>
    public class HozzavaloView
    {
        public int Recept_id { get; set; }
        public int Alapanyag_id { get; set; }
        public string ReceptNev { get; set; }
        public string AlapanyagNev { get; set; }
        public string Mennyiseg { get; set; }

        public override string ToString() => $"{ReceptNev} – {AlapanyagNev}: {Mennyiseg}";
    }
}