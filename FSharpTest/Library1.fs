namespace ClientUtils.FsharpLib

module SomeMath = 
    
    /// <summary>
    /// Naive factorial.
    /// </summary>
    let rec factorial1 n =
        match n with
        | 0 | 1 -> 1
        | _ -> n * factorial1 (n - 1)

    /// <summary>
    /// Factorial with single-threaded tail-recursion.
    /// </summary>
    let factorial2 n = 
        let rec loop i acc = 
            match i with
            | 0 | 1 -> acc
            | _ -> loop (i - 1) (acc * i)
        loop n 1

    /// <summary>
    /// Tail-recursive with built-in multithreading and caching.
    /// </summary>
    let factorial3 n = 
        [1..n] |> List.fold (*) 1

    /// <summary>
    /// Same as factorial3 but uses different method call.
    /// </summary>
    let factorial4 n = 
        [1..n] |> List.reduce (*)


module Parser = 

    open DataContracts

    /// <summary>
    /// Given a code to parse, will return a new Result Message.
    /// </summary>
    let getMessage (s:string) = 
        let listOfString = 
            [ "one"; "two"; "three..." ]
            |> List.append [s]

        let message = 
            FsToCsResultMessage(IsThisTheRealLife = false,
                                IsThisJustFantasy = true,
                                CaughtInALandSlide = 20.0,
                                NoEscapeFromReality = listOfString)
        message