namespace SilicanDate


open Amazon.Lambda.Core

open System


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[<assembly: LambdaSerializer(typeof<Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer>)>]
()

type Input() =
   member val date = "" with get, set

type Output() =
   member val text = "" with get, set
   member val year = 0 with get, set
   member val season = 0 with get, set
   member val week = 0 with get, set
   member val day = 0 with get, set


type Function() =
   static let syncPoint = DateTime(2018, 1, 1)

   static let syncPointDayNumber =
      12017 * 364
      + 7 * ((12017 / 5) - (12017 / 40) + (12017 / 400))

   static let daysIn400Years = 400 * 364 + 7 * (400 / 5 - 400 / 40 + 1)
   static let daysIn40Years = 40 * 364 + 7 * (40 / 5 - 1)
   static let daysIn5Years = 5 * 364 + 7

   static let seasons =
      [ "Nevari"
        "Penari"
        "Sevari"
        "Venari" ]

   static let weeks =
      [ "Ateluna"
        "Beviruto"
        "Deruna"
        "Elito"
        "Feridina"
        "Geranito"
        "Lunamarina"
        "Miraliluto"
        "Peridina"
        "Samerito"
        "Timina"
        "Verato"
        "Wilaluna"
        "Zeroto" ]

   static let daysOfWeek =
      [ "Boromika"
        "Ferimanika"
        "Lusinika"
        "Navimilika"
        "Perinatika"
        "Relikanika"
        "Temiranika" ]

   /// <summary>
   /// A simple function that takes a string and does a ToUpper
   /// </summary>
   /// <param name="input">A date in the format YYYY-MM-DD</param>
   /// <returns>A string representation of the converted Silican date</returns>
   member _.FunctionHandler(input: Input) =
      let parsedDate = DateTime.Parse(input.date)
      let difference = (parsedDate - syncPoint).Days
      let dayNumber = syncPointDayNumber + difference

      let years400, remaining400 =
         dayNumber / daysIn400Years, dayNumber % daysIn400Years

      let years40, remaining40 =
         remaining400 / daysIn40Years, remaining400 % daysIn40Years

      let years5, remaining5 =
         remaining40 / daysIn5Years, remaining40 % daysIn5Years

      let remainingYears, remainingDays = remaining5 / 364, remaining5 % 364

      let year =
         years400 * 400
         + years40 * 40
         + years5 * 5
         + Math.Min(remainingYears, 4)

      let dayOfYear =
         if remainingYears = 5 then
            364 + remainingDays // leap year
         else
            remainingDays

      let season =
         if dayOfYear >= 364 then
            4
         else
            dayOfYear / 91 + 1

      let dayInSeason =
         if dayOfYear >= 364 then
            91 + dayOfYear % 91
         else
            dayOfYear % 91

      let week = dayInSeason / 7 + 1
      let dayOfWeek = dayOfYear % 7 + 1
      let text =
         $"%d{year + 1} %s{seasons.[Math.Min(4, season) - 1]} %s{weeks.[week - 1]} %s{daysOfWeek.[dayOfWeek - 1]}"
      Output(text = text, year = year + 1, season = Math.Min(4, season), week = week, day = dayOfWeek)
