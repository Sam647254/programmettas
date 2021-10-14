namespace SilicanDate.Tests


open Xunit
open Amazon.Lambda.TestUtilities

open SilicanDate


module FunctionTest =
    [<Fact>]
    let ``Invoke Lambda Function``() =
        // Invoke the lambda function and confirm the string was upper cased.
        let lambdaFunction = Function()
        let syncPoint = lambdaFunction.FunctionHandler (Input(date = "2018-01-01"))

        Assert.Equal("12018 Nevari Ateluna Boromika", syncPoint)
        
        let test2 = lambdaFunction.FunctionHandler (Input(date = "2018-01-07"))
        Assert.Equal("12018 Nevari Ateluna Temiranika", test2)

    [<EntryPoint>]
    let main _ = 0
