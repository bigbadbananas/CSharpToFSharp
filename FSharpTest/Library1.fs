namespace ClientUtils.FsharpLib

module SomeMathShit = 
    
    // Naive recursion, not optimized
    let rec factorial1 n =
        match n with
        | 0 | 1 -> 1
        | _ -> n * factorial1 (n - 1)

    // Uses tail-recursion, optimized
    let factorial2 n = 
        let rec loop i acc = 
            match i with
            | 0 | 1 -> acc
            | _ -> loop (i - 1) (acc * i)
        loop n 1

    // Uses tail-recursion internally, but instead as built-in list/seq function
    let factorial3 n = 
        [1..n] |> List.fold (*) 1

    // Same as factorial3, but doesn't require second parameter
    let factorial4 n = 
        [1..n] |> List.reduce (*)
