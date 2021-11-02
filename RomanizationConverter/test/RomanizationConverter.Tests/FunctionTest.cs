using Xunit;
using Amazon.Lambda.TestUtilities;

namespace RomanizationConverter.Tests {
   public class FunctionTest {
      [Fact]
      public void TestPinyin() {
         // Invoke the lambda function and confirm the string was upper cased.
         var function = new Function();
         var tests = new[] {
            ("yan", "yen"),
            ("lü", "leu"),
            ("jyan", "jyuen"),
            ("jun", "jeun"),
            ("gong", "gong"),
         };

         foreach (var (input, expected) in tests) {
            Assert.Equal(expected, function.FunctionHandler(new FunctionInput { Input = input }).FromPinyin);
         }
      }

      [Fact]
      public void TestJyutping() {
         var function = new Function();
         var tests = new[] {
            ("ngo", "ngoa"),
            ("mut", "mut"),
            ("faat", "fat"),
            ("wut", "ut"),
            ("dak", "deak"),
            ("loi", "loai"),
            ("cyun", "cheun"),
            ("zoi", "zoai"),
            ("zi", "zi"),
         };

         foreach (var (input, expected) in tests) {
            Assert.Equal(expected, function.FunctionHandler(new FunctionInput { Input = input }).FromJyutping);
         }
      }
   }
}