using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SilicanDate {
   public class Function {
      /// <summary>
      /// A simple function that takes a string and does a ToUpper
      /// </summary>
      /// <param name="input"></param>
      /// <param name="context"></param>
      /// <returns></returns>
      public Output FunctionHandler(Input input, ILambdaContext context) {
         var parsedDate = DateTime.Parse(input.date);
         var difference = (parsedDate - SyncPoint).Days;
         var dayNumber = SyncPointDayNumber + difference;
         
         var (years400, remaining400) =
            (dayNumber / DaysIn400Years, dayNumber % DaysIn400Years);
         var (years40, remaining40) =
            (remaining400 / DaysIn40Years, remaining400 % DaysIn40Years);
         var (years5, remaining5) =
            (remaining40 / DaysIn5Years, remaining40 % DaysIn5Years);
         var (remainingYears, remainingDays) = (remaining5 / 364, remaining5 % 364);

         var year = years400 * 400 + years40 * 40 + years5 * 5 + Math.Min(remainingYears, 4);
         var dayOfYear = remainingYears == 5 ? 364 + remainingDays : remainingDays;
         var season = dayOfYear >= 364 ? 4 : dayOfYear / 91 + 1;
         var dayInSeason = dayOfYear >= 364 ? 91 + dayOfYear % 91 : dayOfYear % 91;
         var week = dayInSeason / 7 + 1;
         var dayOfWeek = dayOfYear % 7 + 1;
         var text = $"{year + 1} {Seasons[Math.Min(4, season) - 1]} {Weeks[week - 1]} {DaysOfWeek[dayOfWeek - 1]}";
         return new Output {
            text = text,
            year = year + 1,
            season = Math.Min(4, season),
            week = week,
            day = dayOfWeek,
         };
      }

      private static readonly DateTime SyncPoint = new DateTime(2018, 1, 1);

      private const int SyncPointDayNumber = 12017 * 364 + 7 * (12017 / 5 - 12017 / 40 + 12017 / 400);
      private const int DaysIn400Years = 400 * 364 + 7 * (400 / 5 - 400 / 40 + 1);
      private const int DaysIn40Years = 40 * 364 + 7 * (40 / 5 - 1);
      private const int DaysIn5Years = 5 * 364 + 7;

      private static readonly string[] Seasons = {
         "Nevari",
         "Penari",
         "Sevari",
         "Venari",
      };

      private static readonly string[] Weeks = {
         "Ateluna",
         "Beviruto",
         "Deruna",
         "Elito",
         "Feridina",
         "Geranito",
         "Lunamarina",
         "Miraliluto",
         "Peridina",
         "Samerito",
         "Timina",
         "Verato",
         "Wilaluna",
         "Zeroto",
      };

      private static readonly string[] DaysOfWeek = {
         "Boromika",
         "Ferimanika",
         "Lusinika",
         "Navimilika",
         "Perinatika",
         "Relikanika",
         "Temiranika",
      };
   }

   public class Input {
      public string date { get; set; } = "";
   }

   public class Output {
      public string text { get; set; } = "";
      public int year { get; set; } = 0;
      public int season { get; set; } = 0;
      public int week { get; set; } = 0;
      public int day { get; set; } = 0;
   }
}