using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema_Booking_System
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*------------------------------------------------------*/
            /* Variables                                            */
            /*------------------------------------------------------*/

            Dictionary <string, int> ticketsSold = new Dictionary<string, int>(); /* Tickets Sold Dictionary */
            ticketsSold.Add("rush", 0);
            ticketsSold.Add("hiln", 0);
            ticketsSold.Add("thor", 0);
            ticketsSold.Add("filth", 0);
            ticketsSold.Add("planes", 0);

            /*------------------------------------------------------*/


        }

        static string Menu(Dictionary<string, int> ticketsSold)
        {
            string menuLayout = "Welcome to Aquinas Multiplex                   \n"+
                                "We are presently showing:                      \n"+
                                "1. Rush (15) Tickets Sold: {0}                 \n"+
                                "2. How I Live Now (15) Tickets Sold: {1}       \n"+
                                "3. Thor: The Dark World (12) Tickets Sold: {2} \n"+
                                "4. Filth (18) Tickets Sold: {3}                \n"+
                                "5. Planes (U) Tickets Sold: {4}                \n"+
                                "                                               \n"+
                                "Enter the number of the film you wish to see: "   ;

            return string.Format(menuLayout, ticketsSold["rush"], ticketsSold["hiln"], ticketsSold["thor"], ticketsSold["filth"], ticketsSold["planes"]);
        }
    }
}
