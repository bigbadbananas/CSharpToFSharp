namespace ClientUtils.FsharpLib

module SomeMath = 

	let add x y = 
		x + y
		
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
		
	let StdDev items = 
		let mean items = List.sum(items) / items.Length
		
		let innerFunc item = (item - mean) * (item - mean)
		
		let stdDev = 
			List.sumBy(fun x -> innerFunc x) 
			|> float 
			|> System.Math.Sqrt


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