using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
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

            Dictionary<string, int> ageRating = new Dictionary<string, int>(); /* Tickets Sold Dictionary */
            ageRating.Add("rush", 15);
            ageRating.Add("hiln", 15);
            ageRating.Add("thor", 12);
            ageRating.Add("filth", 18);
            ageRating.Add("planes", 0);

            Dictionary<string, List<List<string>>> seatingArrangements = new Dictionary<string, List<List<string>>>();
            for (int movieNum = 1; movieNum <=5; movieNum++)
            {
             
                List<string> movieList = new List<string> {"rush","hiln","thor","filth","planes"};
                string movie = movieList[movieNum - 1];

                seatingArrangements.Add(movie, new List<List<string>> {});

                for (int row = 1; row <= 5; row++)
                {
                    seatingArrangements[movie].Add(new List<string> {});
                    for (int column = 1; column <= 9; column++)
                    {
                        Random rng = new Random();
                        int randomNum = rng.Next(1, 10 + 1);

                        if (randomNum <= 4)
                        {
                            seatingArrangements[movie][row-1].Add("X");
                        }
                        else 
                        {
                            seatingArrangements[movie][row-1].Add("O");
                        }
                    }
                }
            }

            /*------------------------------------------------------*/
            
            RESTART: Console.Clear();

            /*------------------------------------------------------*/
            /* Main Program                                         */
            /*------------------------------------------------------*/

            Console.WriteLine(Menu(ticketsSold));

            Console.Write("Enter the number of the film you wish to see: ");
            int option = (int) Validate(Console.ReadLine(), "System.Int32", "Enter a number: ");
            option = RangeCheck(option, 1, 5);

            ClearLines(1);

            Console.Write("Enter your age: ");
            int age = (int) Validate(Console.ReadLine(), "System.Int32", "Enter a number: ");
            bool validAge = AgeCheck(age, ageRating.ElementAt(option-1).Value, 116);

            if (!validAge) { goto RESTART; }

            for (int row = 1; row <=5; row++)
            {
                for (int column = 1; column <=9; column++)
                {
                    Console.Write(seatingArrangements.ElementAt(option - 1).Value[row - 1][column-1]);
                }
                Console.Write("\n");
            }

            /*------------------------------------------------------*/

        }

        static string Menu(Dictionary<string, int> ticketsSold)
        {
            string menuLayout = "Welcome to Aquinas Multiplex                   \n" +
                                "We are presently showing:                      \n" +
                                "1. Rush (15) Tickets Sold: {0}                 \n" +
                                "2. How I Live Now (15) Tickets Sold: {1}       \n" +
                                "3. Thor: The Dark World (12) Tickets Sold: {2} \n" +
                                "4. Filth (18) Tickets Sold: {3}                \n" +
                                "5. Planes (U) Tickets Sold: {4}                \n" ;

            return string.Format(menuLayout, ticketsSold["rush"], ticketsSold["hiln"], ticketsSold["thor"], ticketsSold["filth"], ticketsSold["planes"]);
        }

        static void ClearLines(int numLines)
        {
            for (int linesCleared = 0; linesCleared <= numLines; linesCleared++)
            {
                Console.SetCursorPosition(0, Console.CursorTop -1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop -1);
            }
            Console.Write("\n");
        }

        static int RangeCheck(int num, int Min, int Max)
        {
            bool valid = true;

            do
            {
                valid = true;
                if (num < Min || num > Max)
                {
                    ClearLines(1);
                    Console.Write("Enter a valid number:  ");
                    num = (int) Validate(Console.ReadLine(), "System.Int32", "Enter a number: ");
                    valid = false;
                }

            } while (valid == false);

            return num;
        }

        static bool AgeCheck(int num, int Min, int Max)
        {
            bool valid = true;

            if (num < Min || num > Max)
            {
                Console.Write("Access Denied - Invalid Age ");
                Console.ReadLine();
                valid = false;
            }

            return valid;
        }

        static object Validate(object input, string targetType, string errorMessage)
        {
            Type type = Type.GetType(targetType);

            bool valid = false;

            while (valid == false)
            {
                try
                {
                    input = Convert.ChangeType(input, type);
                    valid = true;
                }
                catch
                {
                    ClearLines(1);
                    Console.Write(errorMessage);
                    input = Console.ReadLine();
                }
            }
            return input;
        }
    }
}
