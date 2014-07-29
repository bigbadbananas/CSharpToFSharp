namespace FSharpLib

module LocationCodeParser = 
    open System
    open System.Linq
    open System.Text.RegularExpressions
    open Interop
    open DataContracts
    open LocationCodeParser.LocationPatterns
    
    /// <summary>
    /// Entry point to the parser.  Ensures a non-null location code is what goes through parsing.
    /// </summary>
    /// <param name="code">The location code to parse</param>
    let parseLocationCode (code: string) (allDistricts: bool) = 
        // ===== Active patterns ============================
        let (|StartsWith|_|) needle (haystack: string) = 
            if haystack.StartsWith(needle) then Some()
            else None
    
        let (|Contains|_|) needle (haystack: string) = 
            if haystack.Contains(needle) then Some()
            else None
    
        let (|MatchesPattern|_|) pattern code = 
            let matches = Regex.Matches(code, pattern, RegexOptions.Compiled ||| RegexOptions.IgnoreCase)
            if matches.Count > 0 then 
                Some [ for m in matches -> m.Value ]
            else None

        let (|NumericString|_|) string =
            let didParse, value = Decimal.TryParse(string)
            if didParse then Some(value) else None
            
        // ===== Lanes and Districts ========================

        // Parses the district IDs out of the Location Code, placing them into
        // the filter.  Returns the mutated code and filter.
        // String, LocationFilterMessage -> (String, LocationFilterMessage)
        let parseDistricts code (filter: LocationFilterMessage) =
            let mat = Regex.Match(code, LocationPatterns.districtIdsPattern)
            filter.DistrictIds <- mat.Groups.Item(LocationPatterns.DISTRICT_IDS_KEY)
                                            .ToString()
                                            .Split(',')
                                            .Select(fun d -> d.Trim())
                                            .ToList()
            (mat.Groups.Item(LocationPatterns.PREDECESSOR_GROUP_KEY).Value, filter)

        // String, LocationFilterMessage -> (String, LocationFilterMessage)
        // Returns a location code with any lanes affected removed and
        // a mutated LocationFilterMessage with lanes assigned to it.
        let parseLanes code (filter: LocationFilterMessage) =
            // String -> List<string>
            // Uses a single string representing a list of lanes
            // to make a list of lane designation strings.
            let parseOneSideLanes (str: string) =
                Regex.Split(str.Trim(), LocationPatterns.lanesAffectedSingle)
                     .ToList()
            
            let mat = Regex.Match(code, LocationPatterns.lanesAffectedPattern)
            mat.Groups.Item(LocationPatterns.LANES_AFFECTED_KEY).Value.Split(':')
            |> function
                | [| one; two; |] -> filter.LanesAffectedLeft <- parseOneSideLanes one
                                     filter.LanesAffectedRight <- parseOneSideLanes two
                | [| one; |] -> filter.LanesAffectedUnknown <- parseOneSideLanes one
                | _ -> ()
            (mat.Groups.Item(LocationPatterns.PREDECESSOR_GROUP_KEY).Value, filter)
            
        // ===== Cross Feature ==============================
        let parseLeftSide str (filter: LocationFilterMessage) =
            match str with
            | MatchesPattern LocationPatterns.highwayPattern li -> filter.HighwayNumber <- str
            | MatchesPattern LocationPatterns.routePattern li -> filter.RouteNumber <- str
            | _ -> filter.Alias <- str
            filter 

        // String, LocationFilterMessage -> LocationFilterMessage
        let parseRightSide str (filter: LocationFilterMessage) =
            // First trim districts and lanes from the location code
            // and assign the parsed values to the filter
            let trimmedCode, newFilter =
                parseDistricts str filter
                |> function
                    | (one, two) -> parseDistricts one two
                |> function
                    | (one, two) -> parseLanes one two
            
            // Assign the trimmed location code
            newFilter.LocationCode <- trimmedCode

            // Assign road direction code, and put the remainder in cross feature.
            let mat = Regex.Match(trimmedCode, LocationPatterns.roadDirCodePattern)
            newFilter.RoadDirectionCode <- mat.Groups.Item(LocationPatterns.ROAD_DIRECTION_CODE_KEY).Value
            newFilter.CrossFeature <- (mat.Groups.Item(LocationPatterns.PREDECESSOR_GROUP_KEY).Value)
            newFilter

        // String, LocationFilterMessage -> LocationFilterMessage
        // Parses a cross feature based location code.
        let parseXFeature (code: string) (filter: LocationFilterMessage) = 
            code.Split('/')
            |> function
                | [| one; two |] -> parseLeftSide one filter
                                    |> parseRightSide two
                | _ -> filter

        // ===== Lat/Long ===================================
        let numberOrDefault str =
            let didParse, value = Decimal.TryParse(str)
            if didParse then new Nullable<decimal>(value)
            else new Nullable<decimal>()

        let parseLatLng (code: string) (filter: LocationFilterMessage) =
            // Remove the leading "=" and split on the comma.
            code.Replace("=", "")
                .Split(',')
            |> function
                | [| one; two |] -> filter.Latitude <- numberOrDefault one
                                    filter.Longitude <- numberOrDefault two
                | _ -> ()

            filter
    
        // ===== Milepoint ==================================
        let parseZMP (zmp: string) =
            match zmp.Substring(1) with
            | Contains "-" -> zmp.Split('-')
                              |> function
                                 | [|one; two;|] -> (numberOrDefault one, numberOrDefault two)
                                 | _ -> (new Nullable<decimal>(), new Nullable<decimal>())
            | _ ->  (new Nullable<decimal>(), new Nullable<decimal>())


        let parseMP code (filter: LocationFilterMessage) = 
            let codeWithoutDists, filter = parseDistricts code filter
            let codeForMPParse, filter = parseLanes codeWithoutDists filter

            let mat = Regex.Match(codeForMPParse, LocationPatterns.begMPPattern)
            let startMP = mat.Groups.Item(LocationPatterns.MILEPOINT_GROUP_KEY).Value
            match startMP with
            | NumericString mp -> filter.BeginMilepoint <- new Nullable<decimal>(mp)
            | StartsWith "z" -> filter.IsZMilepoint <- true
                                filter.BeginMilepoint, filter.EndMilepoint <- parseZMP startMP

            // TODO: lots more to go
            filter

        // ===== Districts ==================================
        let handleDistricts allDistricts filter = filter
    
        // ===== Top-level logic=============================
        let parse code (filter: LocationFilterMessage) = 
            match code with
            | Contains "/" () -> parseXFeature code filter
            | StartsWith "=" () -> parseLatLng code filter
            | MatchesPattern LocationPatterns.begMPPattern mps -> parseMP mps filter
            | _ -> new LocationFilterMessage()

        let filter = new LocationFilterMessage()
        match code with
        | Null code -> filter
        | _ -> parse (code.Trim()) filter
               |> handleDistricts allDistricts