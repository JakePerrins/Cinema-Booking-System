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

            List<string> movieList = new List<string> { "rush", "hiln", "thor", "filth", "planes" }; // List of movie names (Normally easier to use ElementAt)
            double adultPrice = 7.00;               // Price declaration
            double childPrice = adultPrice / 2;

            Dictionary<string, int> ticketsSold = new Dictionary<string, int>(); /* Tickets Sold Dictionary */
            ticketsSold.Add("rush", 0);
            ticketsSold.Add("hiln", 0);
            ticketsSold.Add("thor", 0);
            ticketsSold.Add("filth", 0);
            ticketsSold.Add("planes", 0);

            Dictionary<string, int> ageRating = new Dictionary<string, int>(); /* Age Rating Dictionary */
            ageRating.Add("rush", 15);
            ageRating.Add("hiln", 15);
            ageRating.Add("thor", 12);
            ageRating.Add("filth", 18);
            ageRating.Add("planes", 0);

            Dictionary<string, List<List<string>>> seatingArrangementsSaveState = new Dictionary<string, List<List<string>>>(); /* Declaring Save State for seating arrangements to revert changes
                                                                                                                                if access is denied after seats are chosen.*/

            Dictionary<string, List<List<string>>> seatingArrangements = new Dictionary<string, List<List<string>>>(); // Creating a 3d list to store all seating arrangements of movies.

            Random rng = new Random(); // Declaring rng

            for (int movieNum = 1; movieNum <= 5; movieNum++) // Loop for each movie
            {
                string currentMovie = movieList[movieNum - 1]; // Initialising movie as the current movie name.

                seatingArrangements.Add(currentMovie, new List<List<string>> { }); // Declaring the dictionary of Key: movie name, and Value: Empty 2d list.
                seatingArrangementsSaveState.Add(currentMovie, new List<List<string>> { }); // Copy for save state.

                for (int row = 1; row <= 5; row++) // Loop for each row.
                {
                    seatingArrangements[currentMovie].Add(new List<string> { }); // Declaring the list for each row.
                    seatingArrangementsSaveState[currentMovie].Add(new List<string> { }); // Copy for save state.

                    for (int column = 1; column <= 9; column++) // Loop for each column in the current row.
                    {
                        int randomNum = rng.Next(1, 10 + 1); // Generating a random number between one and 10 to decide if the seat is empty or not.

                        if (randomNum <= 4) // 40% chance of seat being taken.
                        {
                            seatingArrangements[currentMovie][row - 1].Add("X"); // Setting current seat to taken
                            seatingArrangementsSaveState[currentMovie][row - 1].Add("X"); // Copy for save state.
                        }
                        else // 60% chance of being empty.
                        {
                            seatingArrangements[currentMovie][row - 1].Add("O"); // Setting current seat to empty.
                            seatingArrangementsSaveState[currentMovie][row - 1].Add("O"); // Copy for save state.
                        }
                    }
                }
            }

            /*------------------------------------------------------*/

            RESTART: Console.Clear(); // Flag for restarting upon access denied or completion.

            /*------------------------------------------------------*/
            /* Main Program                                         */
            /*------------------------------------------------------*/

            int numBooking = (int)DeclareInput("How many are being booked for? ", "System.Int32", "Enter a number: "); // Asking user for the group size.

            if (numBooking <= 0)
            {
                Console.WriteLine("Access denied – Not enough people");
                Console.ReadLine();
                goto RESTART;
            }

            ClearLines(1);

            Console.WriteLine(Menu(ticketsSold)); // Writing the Menu to console.

            int option = (int)DeclareInput("Enter the number of the film you wish to see: ", "System.Int32", "Enter a number: "); // Asking user for which movie.
            option = RangeCheck(option, 1, 5); // Checking if input is between 1 and 5 until it is.

            Console.Clear();

            int emptySeats = WriteSeats(seatingArrangements, option, false).Replace("X", "").Length; /* Manipulating seating arrangements to be a list of all the empty seats so its length can be
                                                                                                     taken as the value. */

            if (numBooking > emptySeats) // Checking if there are enough seats left.
            {
                Console.WriteLine("Access denied – Too many people");
                Console.ReadLine();
                goto RESTART;
            }

            List<int> ageList = new List<int>(); // Declaring a list for the age of each person.

            for (int person = 1; person <= numBooking; person++) // Looping for group size.
            {
                string agePrompt = $"Enter Person {person}'s age: "; // Prompt if there is more than 1 person.
                if (numBooking == 1) { agePrompt = "Enter your age: "; } // Prompt if there is only one person.

                int age = (int)DeclareInput(agePrompt, "System.Int32", "Enter a number: "); // Asking user for the current person's age.
                bool validAge = AgeCheck(age, ageRating.ElementAt(option - 1).Value, 116); // Bool value for if the age is between the age rating and the oldest living person.

                if (!validAge) { goto RESTART; } // Restarting if anyone is too young.

                ageList.Add(age); // Adding the current persons age to the list.
                ClearLines(1);
            }

            Console.WriteLine(WriteSeats(seatingArrangements, option, true)); // Writing the seating for the current movie to Console.

            List<List<int>> chosenSeats = new List<List<int>>(); // Declaring a 2d list for the person and their seat row , column.

            for (int person = 1; person <= numBooking; person++) // Loop for group size.
            {
                chosenSeats.Add(new List<int>()); // Declaring the list for each person.

                string seatPrompt = $"Please enter the row and column where Person {person} wants to sit ?"; // Prompt for more than 1 person.
                if (numBooking == 1) { seatPrompt = "Please enter the row and column where you want to sit ?"; } // Promgt if there is only one person.

                Console.WriteLine(seatPrompt); // Writing prompt.
                int userRow; // Declaring variable for row input.
                int userColumn; // Declaring variable for column input.

                bool firstPass = true; // Bool to skip error message
                do
                {
                    if (firstPass == false) // Printing error message if it isnt the first pass.
                    {
                        ClearLines(2);
                        Console.WriteLine("Sorry, this seat is occupied");
                    }

                    userRow = (int)DeclareInput("Row: ", "System.Int32", "Enter a number: "); // Asking user for the row of the seat they want.
                    userRow = RangeCheck(userRow, 1, 5); // Checking if input is between 1 and 5 until it is.

                    userColumn = (int)DeclareInput("Column: ", "System.Int32", "Enter a number: "); // Asking user for the column of the seat they want.
                    userColumn = RangeCheck(userColumn, 1, 9); // Checking if input is between 1 and 9 until it is.

                    firstPass = false;

                } while (seatingArrangements.ElementAt(option - 1).Value[userRow - 1][userColumn - 1] == "X"); // Continuing loop if the seat chosen is taken.

                chosenSeats[person - 1].Add(userRow); // Adding chosen seat to list if it isnt taken
                chosenSeats[person - 1].Add(userColumn);

                seatingArrangements.ElementAt(option - 1).Value[userRow - 1][userColumn - 1] = "X"; // Changing the seating ararangement to reflect the taken seat.

                ClearLines(11);
                Console.WriteLine(WriteSeats(seatingArrangements, option, true)); // Rewriting seats for each loop.
            }

            Console.Clear();

            DateTime date = (DateTime)DeclareInput("What date: ", "System.DateTime", "Enter in the format dd/mm/yyyy: "); // Asking user for the date.

            if ((date - DateTime.Today > DateTime.Now.AddDays(7) - DateTime.Now) || (date < DateTime.Today)) // Checking if the date is within the valid range.
            {
                Console.WriteLine("Access denied – date is invalid"); // If so restarting program.
                Console.ReadLine();
                seatingArrangements = seatingArrangementsSaveState;
                goto RESTART;
            }

            ClearLines(1);

            string seatsString = ""; // Making a string to express the seats chosen.
            for (int person = 1 - 1; person <= numBooking - 1; person++) // Looping for group size.
            {
                seatsString += chosenSeats[person][0] + ",";
                seatsString += chosenSeats[person][1] + " : "; // In the format row,column : row,column
            }

            double price = 0.0; // Declaring the price as £0.0
            for (int person = 1; person <= numBooking; person++) // Looping for group size.
            {
                if (ageList[person-1] >= 18) // Adding the adult price to the total if they are over 18.
                {
                    price += adultPrice;
                }

                else // Otherwise adding the child price.
                {
                    price += childPrice;
                }
            }
                                                                                                     
            Console.Write("--------------------" + "\n"                                        +   
                              "Aquinas Multiplex\n"                                            +   
                              "Film : " + seatingArrangements.ElementAt(option - 1).Key + "\n" +    
                              "Date : " + date.ToShortDateString() + "\n"                      +   
                              "Seats : " + seatsString + "\n"                                  +   
                              "Price : £" + price + "\n\n"                                     + 
                              "Enjoy the film" + "\n"                                          +    
                              "--------------------"                                           );  
                                                                                                    
            Console.ReadLine();

            seatingArrangementsSaveState = seatingArrangements; // Changing the save state to the new changes upon completion.

            string movie = movieList[option - 1]; // Declaring movie to change tickets sold
            ticketsSold[movie] += numBooking; // Adding tickets sold to chosen movie.

            goto RESTART;
            
            /*------------------------------------------------------*/

        }

        static string Menu(Dictionary<string, int> ticketsSold) // Method for printing Menu with the tickets sold as a parameter.
        {
            string menuLayout = "Welcome to Aquinas Multiplex                   \n" +
                                "We are presently showing:                      \n" +
                                "1. Rush (15) Tickets Sold: {0}                 \n" +
                                "2. How I Live Now (15) Tickets Sold: {1}       \n" +
                                "3. Thor: The Dark World (12) Tickets Sold: {2} \n" +
                                "4. Filth (18) Tickets Sold: {3}                \n" +
                                "5. Planes (U) Tickets Sold: {4}                \n" ;

            return string.Format(menuLayout, ticketsSold["rush"], ticketsSold["hiln"], ticketsSold["thor"], ticketsSold["filth"], ticketsSold["planes"]); // Returning.
        }

        static string WriteSeats(Dictionary<string, List<List<string>>> seatingArrangements, int option, bool toPrint) // Method for printing seats or giving the string value.
        {
            string seatString = ""; // Initialising.
            
            if (toPrint) { seatString += "  123456789\n"; } // Only running if it is to print.

            for (int row = 1; row <= 5; row++) // Looping for rows.
            {
                if (toPrint) { seatString += row + " "; } // Only running if it is to print.

                for (int column = 1; column <= 9; column++) // Looping for columns.
                {
                    seatString += seatingArrangements.ElementAt(option - 1).Value[row - 1][column - 1]; // Adding current seat value.
                }
                if (toPrint) { seatString += "\n"; } // Only running if it is to print.
            }

            if (toPrint) { seatString += "\n"; } // Only running if it is to print.

            return seatString; // Returning.
        }

        static void ClearLines(int numLines) // Method for clearing a certain number of lines.
        {
            for (int linesCleared = 0; linesCleared < numLines; linesCleared++) // Looping for number of lines specified.
            {
                Console.SetCursorPosition(0, Console.CursorTop);                                                          // Deletes a single line.
                Console.SetCursorPosition(0, Console.CursorTop - (Console.WindowWidth >= Console.BufferWidth ? 1 : 0));
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop - (Console.WindowWidth >= Console.BufferWidth ? 1 : 0));
            }
        }

        static int RangeCheck(int num, int Min, int Max) // Method for checking if a variable is within a valid range and asking for a new input if not.
        {
            bool valid = true; // Assume true.

            do
            {
                valid = true; // Assume valid is true on each loop.
                if (num < Min || num > Max) // Range check inclusive. Asks for a new input if out of range.
                {
                    ClearLines(1);
                    Console.Write("Enter a valid number:  ");
                    num = (int) Validate(Console.ReadLine(), "System.Int32", "Enter a number: ");
                    valid = false; // Setting valid to false to continue loop.
                }

            } while (valid == false);

            return num; // Returning.
        }

        static bool AgeCheck(int num, int Min, int Max) // Method to check if age is from the given minimum to the given maximum.
        {
            bool valid = true; // Assume true

            if (num < Min || num > Max) // Range check inclusive. Returns false if out of range.
            {
                Console.Write("Access Denied - Invalid Age ");
                Console.ReadLine();
                valid = false;
            }

            return valid;
        }

        static object Validate(object input, string targetType, string errorMessage) // Validation for a variable of any type. Parameter are input of any type, target type and error message.
        {
            Type type = Type.GetType(targetType); // Gets the target type from the given string.

            bool valid = false; // Assumes false.

            while (valid == false) // Loop while false.
            {
                try // tries to convert the input to the target type.
                {
                    input = Convert.ChangeType(input, type);
                    valid = true; // Ends loop if possible.
                }
                catch // Asks for a new input if not possible.
                {
                    ClearLines(1);
                    Console.Write(errorMessage);
                    input = Console.ReadLine();
                }
            }
            return input; // Returns input.
        }

        static object DeclareInput(string prompt, string targetType, string errorMessage) // Method for declaring an input on one line.
        {
            Type type = Type.GetType(targetType); // Gets target type to pass to validate.

            Console.Write(prompt); // Asks using prompt.

            object variable = Convert.ChangeType(Validate(Console.ReadLine(), targetType, errorMessage), type); // Validates and then changes user input to the correct type after.

            return variable; // Returning.
        }
    }
}
