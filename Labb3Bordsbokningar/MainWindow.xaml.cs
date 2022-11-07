using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.IO;

namespace Labb3Bordsbokningar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Booking> Bookings = new List<Booking>();
        public MainWindow()
        {
            InitializeComponent();
            Bookings = new List<Booking>() { new Booking("Jonatan", new DateTime(2022,11,15), "19.00", 3 ),
            new Booking("Fredrik", new DateTime(2022,11,19), "19.00", 5 ),
            new Booking("Mathilda", new DateTime(2022,11,19), "21.00", 1 )};
            UpdateFile();
        }
        //Kod för (bokning)knappen
        private void BtnBook(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime date = datePicker.SelectedDate.Value;
                string name = tbxName.Text;
                string time = cbxTime.Text;
                int tableNr = Int32.Parse(cbxTable.Text);

                BookTable(date, name, time, tableNr);
                UpdateFile();
            }
            catch
            {
                MessageBox.Show("Bokningen misslyckades, se till att fylla i allt korrekt!", "Felmeddelande");
            }
        }
        //Kod för (visa bokningar)knappen
        private void BtnShowBookings(object sender, RoutedEventArgs e)
        {
            ShowBookings();
        }
        //Kod för (ta bort bokning)knappen
        private void BtnCancelBooking(object sender, RoutedEventArgs e)
        {
            CancelBooking();
            UpdateFile();
        }
        //Metod för att boka ett bord
        public void BookTable(DateTime date, string name, string time, int tableNr)
        {
            bool isAvaibible = CheckIfAvaibible(date, name, time, tableNr);

            if (isAvaibible && name != "" && date >= DateTime.Today)
            {
                var booking = new Booking(name, date, time, tableNr);
                Bookings.Add(booking);
                lbxBokningar.Items.Clear();
                MessageBox.Show("Bokningen lyckades!", "Meddelande");
            }
            else if (date < DateTime.Today)
            {
                MessageBox.Show("Detta datum har redan passerat, vänligen välj ett nytt datum", "Felmeddelande");
            }
            else if (name == "")
            {
                MessageBox.Show("Du måste fylla i namn för att kunna boka", "Felmeddelande");
            }
        }
        // Metod för att kolla om bordet är tillgängligt
        public bool CheckIfAvaibible(DateTime date, string name, string time, int tableNr)
        {
            var query = Bookings
                  .Where(bookings => bookings.Date == datePicker.SelectedDate && bookings.Time == cbxTime.Text)
                  .Select(bookings => bookings.Time);

            foreach (var booking in Bookings)
            {
                if (booking.Date == date && booking.Time == time && booking.TableNumber == tableNr)
                {
                    MessageBox.Show("Detta bord är redan bokat den här tiden, försök igen", "Felmeddelande");
                    return false;
                }
            }
            if (query.Count() > 4)
            {
                MessageBox.Show("Max antal bord är bokade för den här tiden!\nVänlingen välj en ny tid eller nytt datum", "Felmeddelande");
                return false;
            }
            return true;
        }
        //Metod för att ta bort en bokning från både lista och listboxen
        public void CancelBooking()
        {
            if (Bookings.Count > 0)
            {
                int index = lbxBokningar.SelectedIndex;

                Bookings = Bookings.OrderBy(x => x.Date).ThenBy(x => x.Time).ThenBy(x => x.TableNumber).ToList();

                Bookings.RemoveAt(index);

                foreach (string booking in lbxBokningar.SelectedItems.OfType<string>().ToList())
                {
                    lbxBokningar.Items.Remove(booking);
                }

                MessageBox.Show($"Du har nu avbokat bokningen", "Meddelande");
            }
            else
            {
                MessageBox.Show("Det finns inga bokningar att avboka", "Felmeddelande");
            }
        }
        //Metod för att visa bokningar
        public void ShowBookings()
        {
            if (Bookings.Count <= 0)
            {
                lbxBokningar.Items.Clear();
                MessageBox.Show("Det finns inga bokningar att visa", "Felmeddelande");
            }
            else
            {
                lbxBokningar.Items.Clear();

                string line = "";

                using (StreamReader file = new StreamReader("BokningsLista.txt"))

                    while ((line = file.ReadLine()) != null)
                    {
                        lbxBokningar.Items.Add(line);
                    }
            }
        }
        //Metod för att updatera textfilen med bokningar
        public void UpdateFile()
        {
            File.WriteAllText("BokningsLista.txt", String.Empty);

            var query =
                    from booking in Bookings
                    orderby booking.Date ascending, booking.Time ascending, booking.TableNumber ascending
                    group booking by booking.Date;

            foreach (var bookingroup in query)
            {
                foreach (var booking in bookingroup)
                {
                    using StreamWriter streamWriter = new StreamWriter("BokningsLista.txt", true);
                    {
                        streamWriter.WriteLine("Datum: " + booking.Date.ToShortDateString() +
                            ", " + booking.Time + " Namn: " + booking.Name + " Bord: " + booking.TableNumber);
                    }
                }
            }
        }
    }
}