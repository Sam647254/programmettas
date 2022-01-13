using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using SilicanDate;

namespace SilicanDate.Tests {
   public class FunctionTest {
      [Fact]
      public void TestToUpperFunction() {
         // Invoke the lambda function and confirm the string was upper cased.
         var function = new Function();
         var context = new TestLambdaContext();
         var output1 = function.FunctionHandler(new Input { date = "2018-01-01" }, context);
         Assert.Equal("12018 Nevari Ateluna Boromika", output1.text);

         var output2 = function.FunctionHandler(new Input { date = "2022-01-12" }, context);
         Assert.Equal("12022 Nevari Beviruto Lusinika", output2.text);
      }
   }
}