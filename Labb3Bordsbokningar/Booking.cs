using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Labb3Bordsbokningar
{
    internal class Booking
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public int TableNumber { get; set; }


        public Booking(string name, DateTime date, string time, int tableNumber)
        {
            Name = name;
            Date = date;
            Time = time;
            TableNumber = tableNumber;
        }

        public void AddBooking(DateTime date, string name, string time, int tableNumber)
        {
            using StreamWriter streamWriter = new StreamWriter("BokningsLista.txt", true);
            {
                streamWriter.WriteLine("Datum: " + date.ToShortDateString() + ", " + time + " Namn: " + name + " Bord: " + tableNumber);
            }
        }

    }

}


