namespace LocationCodeParser.Parser

/// <summary>
/// Contains all Location Code Parsing logic.
/// </summary>
module LocationCodeParser = 
    open System
    open System.Text.RegularExpressions
    open Interop
    open LocationCodeParser.LocationPatterns
    
    let (|StartsWith|_|) needle (haystack : string) = 
        if haystack.StartsWith(needle) then Some()
        else None
    
    let (|Contains|_|) needle (haystack : string) = 
        if haystack.Contains(needle) then Some()
        else None
    
    let (|MatchesPattern|_|) pattern code = 
        let matches = Regex.Matches(code, pattern, RegexOptions.Compiled ||| RegexOptions.IgnoreCase)
        if matches.Count > 0 then 
            Some [ for m in matches -> m.Value ]
        else None
    
    let parseXFeature code = printfn ""
    let parseLatLng code = printfn ""
    let parseAddress code = printfn ""
    let parseMP code mps = printfn ""
    let handleDistricts allDistricts code = printf ""
    
    /// <summary>
    /// Top-level parsing logic.  Defers parsing work based on rudimentary structure of the location code.
    /// </summary>
    /// <param name="code">The location code to parse</param>
    let parse code = 
        match code with
        | StartsWith "/" () -> parseXFeature code
        | StartsWith "=" () -> parseLatLng code
        | StartsWith "@" () -> parseAddress code
        | MatchesPattern LocationPatterns.begMPPattern mps -> parseMP mps code
        | _ -> printfn "lolwhoops"
    
    /// <summary>
    /// Entry point to the parser.  Ensures a non-null location code is what goes through parsing.
    /// </summary>
    /// <param name="code">The location code to parse</param>
    /// <param name="allDistricts"></param>
    /// <remarks>
    /// Because of the nature of F# types, no null values can make it past this point.
    /// </remarks>
    let parseLocationCode (code : string, allDistricts : bool) = 
        match code with
        | Null code -> printfn ""
        | _ -> 
            code.Trim()
            |> parse
            |> handleDistricts allDistricts
