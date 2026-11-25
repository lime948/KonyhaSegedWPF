using KonyhaSegedWPF.ReceptModels;
using KonyhaSegedWPF.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace KonyhaSegedWPF
{
    public partial class MainWindow : Window
    {
        private readonly ReceptService receptService = new ReceptService();
        private readonly HozzavaloService hozzavaloService = new HozzavaloService();
        private readonly AlapanyagService alapanyagService = new AlapanyagService();

        public ObservableCollection<Recept> Receptek { get; } = new ObservableCollection<Recept>();
        public ObservableCollection<Alapanyag> Alapanyagok { get; } = new ObservableCollection<Alapanyag>();
        public ObservableCollection<HozzavaloView> HozzavaloView { get; } = new ObservableCollection<HozzavaloView>();

        private Recept _kivalasztottRecept;
        private Alapanyag _kivalasztottAlapanyag;
        private HozzavaloView _kivalasztottHozzavalo;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            BetoltMinden();
        }

        // ================================================================
        //  BETÖLTÉS
        // ================================================================

        private void BetoltMinden()
        {
            BetoltReceptek();
            BetoltAlapanyagok();
            BetoltHozzavalok();
        }

        private void BetoltReceptek()
        {
            Receptek.Clear();
            foreach (var r in receptService.ReadReceptek())
                Receptek.Add(r);
        }

        private void BetoltAlapanyagok()
        {
            Alapanyagok.Clear();
            foreach (var a in alapanyagService.ReadAlapanyagok())
                Alapanyagok.Add(a);

            cbHozzavaloAlapanyag.ItemsSource = Alapanyagok;
        }

        private void BetoltHozzavalok()
        {
            HozzavaloView.Clear();

            var receptById = Receptek.ToDictionary(r => r.Recept_id, r => r.Nev);
            var alapanyagById = Alapanyagok.ToDictionary(a => a.Alapanyag_id, a => a.Nev);

            foreach (var h in hozzavaloService.ReadHozzavalok())
            {
                HozzavaloView.Add(new HozzavaloView
                {
                    Recept_id = h.Recept_id,
                    Alapanyag_id = h.Alapanyag_id,
                    ReceptNev = receptById.TryGetValue(h.Recept_id, out var rn) ? rn : $"#{h.Recept_id}",
                    AlapanyagNev = alapanyagById.TryGetValue(h.Alapanyag_id, out var an) ? an : $"#{h.Alapanyag_id}",
                    Mennyiseg = h.Mennyiseg
                });
            }

            cbHozzavaloRecept.ItemsSource = Receptek;
        }

        // ================================================================
        //  RECEPTEK
        // ================================================================

        private void dgReceptek_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _kivalasztottRecept = dgReceptek.SelectedItem as Recept;
            if (_kivalasztottRecept == null) return;

            tbReceptNev.Text = _kivalasztottRecept.Nev;
            tbReceptLeiras.Text = _kivalasztottRecept.Leiras;
            tbReceptIdo.Text = _kivalasztottRecept.Elkeszitesi_ido.ToString();
            tbReceptKategoria.Text = _kivalasztottRecept.Kategoria;
            tbReceptAllergenek.Text = _kivalasztottRecept.Allergenek;
        }

        private void btnReceptNew_Click(object sender, RoutedEventArgs e)
        {
            _kivalasztottRecept = null;
            dgReceptek.SelectedItem = null;
            tbReceptNev.Clear();
            tbReceptLeiras.Clear();
            tbReceptIdo.Clear();
            tbReceptKategoria.Clear();
            tbReceptAllergenek.Clear();
            tbReceptNev.Focus();
        }

        private void btnReceptSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbReceptNev.Text) ||
                !int.TryParse(tbReceptIdo.Text, out int ido))
            {
                MessageBox.Show("A név és az elkészítési idő (szám) kötelező.",
                    "Hiányos adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool ok;
            if (_kivalasztottRecept == null)
            {
                ok = receptService.CreateRecept(
                    tbReceptNev.Text.Trim(),
                    tbReceptLeiras.Text.Trim(),
                    ido,
                    tbReceptKategoria.Text.Trim(),
                    string.IsNullOrWhiteSpace(tbReceptAllergenek.Text) ? null : tbReceptAllergenek.Text.Trim());
            }
            else
            {
                ok = receptService.UpdateRecept(
                    _kivalasztottRecept.Recept_id,
                    tbReceptNev.Text.Trim(),
                    tbReceptLeiras.Text.Trim(),
                    ido,
                    tbReceptKategoria.Text.Trim(),
                    string.IsNullOrWhiteSpace(tbReceptAllergenek.Text) ? null : tbReceptAllergenek.Text.Trim());
            }

            if (ok) { BetoltReceptek(); BetoltHozzavalok(); btnReceptNew_Click(null, null); }
        }

        private void btnReceptDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_kivalasztottRecept == null) return;

            if (MessageBox.Show($"Biztosan törlöd: {_kivalasztottRecept.Nev}?",
                    "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            if (receptService.DeleteRecept(_kivalasztottRecept.Recept_id))
            {
                BetoltReceptek();
                BetoltHozzavalok();
                btnReceptNew_Click(null, null);
            }
        }

        private void btnReceptRefresh_Click(object sender, RoutedEventArgs e) => BetoltMinden();

        // ================================================================
        //  ALAPANYAGOK
        // ================================================================

        private void dgAlapanyagok_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _kivalasztottAlapanyag = dgAlapanyagok.SelectedItem as Alapanyag;
            if (_kivalasztottAlapanyag != null)
                tbAlapanyagNev.Text = _kivalasztottAlapanyag.Nev;
        }

        private void btnAlapanyagNew_Click(object sender, RoutedEventArgs e)
        {
            _kivalasztottAlapanyag = null;
            dgAlapanyagok.SelectedItem = null;
            tbAlapanyagNev.Clear();
            tbAlapanyagNev.Focus();
        }

        private void btnAlapanyagSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbAlapanyagNev.Text))
            {
                MessageBox.Show("A név kötelező.", "Hiányos adat",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            bool ok = _kivalasztottAlapanyag == null
                ? alapanyagService.CreateAlapanyag(tbAlapanyagNev.Text.Trim())
                : alapanyagService.UpdateAlapanyag(_kivalasztottAlapanyag.Alapanyag_id, tbAlapanyagNev.Text.Trim());

            if (ok) { BetoltAlapanyagok(); BetoltHozzavalok(); btnAlapanyagNew_Click(null, null); }
        }

        private void btnAlapanyagDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_kivalasztottAlapanyag == null) return;

            if (MessageBox.Show($"Biztosan törlöd: {_kivalasztottAlapanyag.Nev}? (A hozzá tartozó hozzávaló sorok is törlődnek.)",
                    "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            if (alapanyagService.DeleteAlapanyag(_kivalasztottAlapanyag.Alapanyag_id))
            {
                BetoltAlapanyagok();
                BetoltHozzavalok();
                btnAlapanyagNew_Click(null, null);
            }
        }

        private void btnAlapanyagRefresh_Click(object sender, RoutedEventArgs e) => BetoltMinden();

        // ================================================================
        //  HOZZÁVALÓK
        // ================================================================

        private void dgHozzavalok_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _kivalasztottHozzavalo = dgHozzavalok.SelectedItem as HozzavaloView;
            if (_kivalasztottHozzavalo == null) return;

            cbHozzavaloRecept.SelectedValue = _kivalasztottHozzavalo.Recept_id;
            cbHozzavaloAlapanyag.SelectedValue = _kivalasztottHozzavalo.Alapanyag_id;
            tbHozzavaloMennyiseg.Text = _kivalasztottHozzavalo.Mennyiseg;
        }

        private void btnHozzavaloNew_Click(object sender, RoutedEventArgs e)
        {
            _kivalasztottHozzavalo = null;
            dgHozzavalok.SelectedItem = null;
            cbHozzavaloRecept.SelectedIndex = -1;
            cbHozzavaloAlapanyag.SelectedIndex = -1;
            tbHozzavaloMennyiseg.Clear();
        }

        private void btnHozzavaloSave_Click(object sender, RoutedEventArgs e)
        {
            if (cbHozzavaloRecept.SelectedValue == null ||
                cbHozzavaloAlapanyag.SelectedValue == null ||
                string.IsNullOrWhiteSpace(tbHozzavaloMennyiseg.Text))
            {
                MessageBox.Show("Recept, alapanyag és mennyiség kötelező.",
                    "Hiányos adat", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int receptId = (int)cbHozzavaloRecept.SelectedValue;
            int alapanyagId = (int)cbHozzavaloAlapanyag.SelectedValue;
            string mennyiseg = tbHozzavaloMennyiseg.Text.Trim();

            bool ok;
            if (_kivalasztottHozzavalo == null)
            {
                ok = hozzavaloService.CreateHozzavalo(receptId, alapanyagId, mennyiseg);
            }
            else if (_kivalasztottHozzavalo.Recept_id == receptId &&
                     _kivalasztottHozzavalo.Alapanyag_id == alapanyagId)
            {
                // Csak a mennyiség változott
                ok = hozzavaloService.UpdateHozzavalo(receptId, alapanyagId, mennyiseg);
            }
            else
            {
                // A kapcsolat (recept, alapanyag) változott → törlés + új beszúrás
                ok = hozzavaloService.DeleteHozzavalo(
                        _kivalasztottHozzavalo.Recept_id, _kivalasztottHozzavalo.Alapanyag_id)
                     && hozzavaloService.CreateHozzavalo(receptId, alapanyagId, mennyiseg);
            }

            if (ok) { BetoltHozzavalok(); btnHozzavaloNew_Click(null, null); }
        }

        private void btnHozzavaloDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_kivalasztottHozzavalo == null) return;

            if (MessageBox.Show($"Törlöd: {_kivalasztottHozzavalo.ReceptNev} – {_kivalasztottHozzavalo.AlapanyagNev}?",
                    "Megerősítés", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            if (hozzavaloService.DeleteHozzavalo(_kivalasztottHozzavalo.Recept_id, _kivalasztottHozzavalo.Alapanyag_id))
            {
                BetoltHozzavalok();
                btnHozzavaloNew_Click(null, null);
            }
        }

        private void btnHozzavaloRefresh_Click(object sender, RoutedEventArgs e) => BetoltMinden();
    }
}