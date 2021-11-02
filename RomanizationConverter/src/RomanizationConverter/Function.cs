using System.Collections.Generic;
using System.Linq;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.SystemTextJson;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(DefaultLambdaJsonSerializer))]

namespace RomanizationConverter {
   public class Function {
      private static readonly Dictionary<string, string> PinyinSyllabicConsonants = new() {
         {"zhi", "jh"},
         {"chi", "ch"},
         {"shi", "sh"},
         {"zi", "zh"},
         {"ci", "tszh"},
         {"si", "szh"},
         {"ri", "rh"},
         {"n", "nh"},
         {"m", "mh"},
         {"ng", "ngh"},
      };

      private static readonly Dictionary<string, string> PinyinInitials = new() {
         {"b", "b"},
         {"p", "p"},
         {"m", "m"},
         {"f", "f"},
         {"d", "d"},
         {"t", "t"},
         {"n", "n"},
         {"l", "l"},
         {"j", "j"},
         {"q", "ch"},
         {"x", "sh"},
         {"z", "z"},
         {"c", "ts"},
         {"s", "s"},
         {"zh", "j"},
         {"ch", "ch"},
         {"sh", "sh"},
         {"r", "r"},
         {"g", "g"},
         {"k", "k"},
         {"h", "h"},
      };

      private static readonly Dictionary<string, string> PinyinFinalsNormalized = new() {
         {"a", "a"},
         {"ê", "ae"},
         {"ai", "ai"},
         {"an", "an"},
         {"ang", "ang"},
         {"ei", "ei"},
         {"en", "en"},
         {"eng", "eng"},
         {"er", "er"},
         {"ü", "yu"},
         {"i", "yi"},
         {"ong", "ong"},
         {"ou", "ou"},
         {"ia", "ya"},
         {"ian", "yan"},
         {"ie", "ye"},
         {"iao", "yao"},
         {"in", "yin"},
         {"ing", "ying"},
         {"iu", "you"},
         {"iang", "yang"},
         {"u", "wu"},
         {"ua", "wa"},
         {"uo", "wo"},
         {"o", "wo"},
         {"uai", "wai"},
         {"ui", "wei"},
         {"un", "wen"},
         {"uang", "wang"},
         {"weng", "weng"},
         {"üe", "yue"},
         {"uan", "yuan"},
      };

      private static readonly Dictionary<string, string> PinyinFinals = new() {
         {"ao", "au"},
         {"wu", "u"},
         {"yan", "yen"},
         {"yao", "yau"},
         {"yi", "i"},
         {"yin", "in"},
         {"ying", "ing"},
         {"yu", "eu"},
         {"yuan", "yuen"},
         {"yun", "eun"},
      };

      private static readonly List<string> JyutpingInitials = new() {
         "ng", "b", "p", "m", "f", "d", "t", "n", "l", "h",
         "gw", "kw", "g", "k", "w", "z", "c", "s", "j",
      };
      public ConversionResult FunctionHandler(string input) {
         var pinyinResult = ParsePinyin(input);
         var jyutpingResult = ParseJyutping(input);
         return new ConversionResult {
            FromPinyin = pinyinResult,
            FromJyutping = jyutpingResult,
         };
      }

      private static string? ParsePinyin(string input) {
         if (PinyinSyllabicConsonants.ContainsKey(input)) {
            return PinyinSyllabicConsonants[input];
         }
         var initial = PinyinInitials.FirstOrDefault(i => input.StartsWith(i.Key));
         if (initial.Key == null) {
            return PinyinFinals.ContainsKey(input) ? PinyinFinals[input] : null;
         }

         var remaining = input[initial.Key.Length..];
         var normalized = initial.Key is "j" or "q" or "x" && remaining.StartsWith("u") ? "y" + remaining :
            PinyinFinalsNormalized.ContainsKey(remaining) ? PinyinFinalsNormalized[remaining] :
            PinyinFinalsNormalized.ContainsValue(remaining) ? remaining : null;
         if (normalized != null) {
            return initial.Value + PinyinFinals.GetValueOrDefault(normalized, normalized);
         }
         return null;
      }

      private static string? ParseJyutping(string input) {
         if (input is "m" or "ng") {
            return input + "h";
         }
         
         var initial = JyutpingInitials.FirstOrDefault(input.StartsWith);
         if (initial == null) return ParseJyutpingFinal(input);
         var remaining = input[initial.Length..];
         var final = ParseJyutpingFinal(remaining);
         if (final == null) return null;

         return initial switch {
            "z" when final.StartsWith("oe") || final.StartsWith("eu") => "j" + final,
            "c" when final.StartsWith("oe") || final.StartsWith("eu") => "ch" + final,
            "c" => "ts" + final,
            "j" when final.StartsWith("i") || final.StartsWith("eu") => final,
            "j" => "y" + final,
            "w" when final.StartsWith("u") => final,
            _ => initial + final,
         };
      }

      private static string? ParseJyutpingFinal(string input) {
         if (input.StartsWith("aa")) {
            return input[1..];
         }

         if (input.StartsWith("a")) {
            return "e" + input;
         }

         if (input.StartsWith("e")) {
            return "a" + input;
         }

         if (input == "ung") {
            return "ong";
         }

         if (input.StartsWith("i") || input.StartsWith("u") || input.StartsWith("oe")) {
            return input;
         }

         if (input.StartsWith("o")) {
            return "oa" + input[1..];
         }

         if (input.StartsWith("eo")) {
            return "oe" + input[2..];
         }

         if (input.StartsWith("yu")) {
            return "eu" + input[2..];
         }

         return null;
      }
   }
}