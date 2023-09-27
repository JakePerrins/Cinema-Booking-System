using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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

            double adultPrice = 7.00;
            double childPrice = adultPrice / 2;

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

            Dictionary<string, List<List<string>>> seatingArrangementsSaveState = new Dictionary<string, List<List<string>>>();

            Dictionary<string, List<List<string>>> seatingArrangements = new Dictionary<string, List<List<string>>>();

            Random rng = new Random();

            for (int movieNum = 1; movieNum <=5; movieNum++)
            {
             
                List<string> movieList = new List<string> {"rush","hiln","thor","filth","planes"};
                string movie = movieList[movieNum - 1];

                seatingArrangements.Add(movie, new List<List<string>> {});
                seatingArrangementsSaveState.Add(movie, new List<List<string>> { });

                for (int row = 1; row <= 5; row++)
                {
                    seatingArrangements[movie].Add(new List<string> {});
                    seatingArrangementsSaveState[movie].Add(new List<string> { });

                    for (int column = 1; column <= 9; column++)
                    {
                        int randomNum = rng.Next(1, 10 + 1);

                        if (randomNum <= 4)
                        {
                            seatingArrangements[movie][row-1].Add("X");
                            seatingArrangementsSaveState[movie][row-1].Add("X");
                        }
                        else 
                        {
                            seatingArrangements[movie][row-1].Add("O");
                            seatingArrangementsSaveState[movie][row-1].Add("O");
                        }
                    }
                }
            }

            /*------------------------------------------------------*/

            RESTART: Console.Clear();

            /*------------------------------------------------------*/
            /* Main Program                                         */
            /*------------------------------------------------------*/

            int numBooking = (int) DeclareInput("How many are being booked for? ", "System.Int32", "Enter a number: ");

            ClearLines(1);

            Console.WriteLine(Menu(ticketsSold));

            int option = (int)DeclareInput("Enter the number of the film you wish to see: ", "System.Int32", "Enter a number: ");
            option = RangeCheck(option, 1, 5);

            Console.Clear();

            int emptySeats = WriteSeats(seatingArrangements, option, true).Replace("X", "").Length;

            if (numBooking > emptySeats)
            {
                Console.WriteLine("Access denied – Too many people");
                Console.ReadLine();
                goto RESTART;
            }

            List<int> ageList = new List<int>();

            for (int person = 1; person <= numBooking; person++)
            {
                string agePrompt = $"Enter Person {person}'s age: ";
                if (numBooking == 1) { agePrompt = "Enter your age: "; }

                int age = (int)DeclareInput(agePrompt, "System.Int32", "Enter a number");
                bool validAge = AgeCheck(age, ageRating.ElementAt(option - 1).Value, 116);

                if (!validAge) { goto RESTART; }

                ageList.Add(age);
                ClearLines(1);
            }

            Console.WriteLine(WriteSeats(seatingArrangements, option, true));

            List<List<int>> chosenSeats = new List<List<int>>();

            for (int person = 1; person <= numBooking; person++)
            {
                chosenSeats.Add(new List<int>());

                string seatPrompt = $"Please enter the row and column where Person {person} wants to sit ?";
                if (numBooking == 1) { seatPrompt = "Please enter the row and column where you want to sit ?"; }

                Console.WriteLine(seatPrompt);
                int userRow;
                int userColumn;

                bool firstPass = true;
                do
                {
                    if (firstPass == false)
                    {
                        ClearLines(2);
                        Console.WriteLine("Sorry, this seat is occupied");
                    }

                    userRow = (int)DeclareInput("Row: ", "System.Int32", "Enter a number: ");
                    userRow = RangeCheck(userRow, 1, 5);

                    userColumn = (int)DeclareInput("Column: ", "System.Int32", "Enter a number: ");
                    userColumn = RangeCheck(userColumn, 1, 9);

                    firstPass = false;

                } while (seatingArrangements.ElementAt(option - 1).Value[userRow - 1][userColumn - 1] == "X");

                chosenSeats[person - 1].Add(userRow);
                chosenSeats[person - 1].Add(userColumn);

                seatingArrangements.ElementAt(option - 1).Value[userRow - 1][userColumn - 1] = "X";

                ClearLines(11);
                Console.WriteLine(WriteSeats(seatingArrangements, option, true));
            }

            ClearLines(8);

            DateTime date = (DateTime) DeclareInput("What date: ", "System.DateTime", "Enter in the format dd/mm/yyyy");

            if  ( (date - DateTime.Today > DateTime.Now.AddDays(7) - DateTime.Now) || (date < DateTime.Today) )
            {
                Console.WriteLine("Access denied – date is invalid");
                Console.ReadLine();
                seatingArrangements = seatingArrangementsSaveState;
                goto RESTART;
            }

            ClearLines(1);

            string seatsString = "";
            for (int person = 1 - 1; person <= numBooking - 1; person++)
            {
                seatsString += chosenSeats[person][0] + ",";
                seatsString += chosenSeats[person][1] + " : ";
            }

            Console.Write("--------------------" + "\n"                                    +
                              "Aquinas Multiplex\n"                                            +
                              "Film : " + seatingArrangements.ElementAt(option - 1).Key + "\n" +
                              "Date : " + date.ToShortDateString() + "\n"                      +
                              "Seats : " + seatsString + "\n\n"                                +
                              "Enjoy the film" + "\n"                                          +
                              "--------------------"                                           );

            Console.ReadLine();

            seatingArrangementsSaveState = seatingArrangements;
            goto RESTART;
            
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

        static string WriteSeats(Dictionary<string, List<List<string>>> seatingArrangements, int option, bool toPrint)
        {
            string seatString = ""; 
            
            if (toPrint) { seatString += "  123456789\n"; }

            for (int row = 1; row <= 5; row++)
            {
                if (toPrint) { seatString += row + " "; }

                for (int column = 1; column <= 9; column++)
                {
                    seatString += seatingArrangements.ElementAt(option - 1).Value[row - 1][column - 1];
                }
                if (toPrint) { seatString += "\n"; }
            }

            if (toPrint) { seatString += "\n"; }

            return seatString;
        }

        static void ClearLines(int numLines)
        {
            for (int linesCleared = 0; linesCleared < numLines; linesCleared++)
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.SetCursorPosition(0, Console.CursorTop - (Console.WindowWidth >= Console.BufferWidth ? 1 : 0));
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop - (Console.WindowWidth >= Console.BufferWidth ? 1 : 0));
            }
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

        static object DeclareInput(string prompt, string targetType, string errorMessage)
        {
            Type type = Type.GetType(targetType);

            Console.Write(prompt);

            object variable = Convert.ChangeType(Validate(Console.ReadLine(), targetType, errorMessage), type);

            return variable;
        }
    }
}
