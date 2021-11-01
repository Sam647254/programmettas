using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

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
         {"r", "r"}
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

      public ConversionResult FunctionHandler(string input, ILambdaContext context) {
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
         } else {
            var remaining = input[initial.Key.Length..];
            var normalized = initial.Key is "j" or "q" or "x" && remaining.StartsWith("u") ? "y" + remaining :
               PinyinFinalsNormalized.ContainsKey(remaining) ? PinyinFinalsNormalized[remaining] :
               PinyinFinalsNormalized.ContainsValue(remaining) ? remaining : null;
            if (normalized != null) {
               return initial.Value + PinyinFinals.GetValueOrDefault(normalized, normalized);
            }
         }
         return null;
      }

      private static string? ParseJyutping(string input) {
         return null;
      }
   }
}