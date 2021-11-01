using Xunit;
using Amazon.Lambda.TestUtilities;

namespace RomanizationConverter.Tests {
   public class FunctionTest {
      [Fact]
      public void TestToUpperFunction() {
         // Invoke the lambda function and confirm the string was upper cased.
         var function = new Function();
         var context = new TestLambdaContext();
         var ping = function.FunctionHandler("ping", context);

         Assert.Equal("ping", ping.FromPinyin);
         Assert.Equal("yen", function.FunctionHandler("yan", context).FromPinyin);
         Assert.Equal("leu", function.FunctionHandler("lü", context).FromPinyin);
         Assert.Equal("jyuen", function.FunctionHandler("juan", context).FromPinyin);
         Assert.Equal("jeun", function.FunctionHandler("jun", context).FromPinyin);
      }
   }
}