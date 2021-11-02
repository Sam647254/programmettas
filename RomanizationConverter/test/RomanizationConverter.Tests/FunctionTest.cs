using Xunit;
using Amazon.Lambda.TestUtilities;

namespace RomanizationConverter.Tests {
   public class FunctionTest {
      [Fact]
      public void TestPinyin() {
         // Invoke the lambda function and confirm the string was upper cased.
         var function = new Function();
         
         Assert.Equal("yen", function.FunctionHandler("yan").FromPinyin);
         Assert.Equal("leu", function.FunctionHandler("lü").FromPinyin);
         Assert.Equal("jyuen", function.FunctionHandler("juan").FromPinyin);
         Assert.Equal("jeun", function.FunctionHandler("jun").FromPinyin);
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
            Assert.Equal(expected, function.FunctionHandler(input).FromJyutping);
         }
      }
   }
}