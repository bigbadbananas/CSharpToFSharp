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

        // ===== Utils ============================ 

        let convertToNullableDecimal str =
            let didParse, value = Decimal.TryParse(str)
            if didParse then new Nullable<decimal>(value)
            else new Nullable<decimal>()

        let getGroup (mat: Match) (key: string) = mat.Groups.Item(key).Value

        // ===== Active patterns ============================

        let (|StartsWith|_|) needle (haystack: string) = 
            if haystack.StartsWith(needle, StringComparison.OrdinalIgnoreCase) then Some()
            else None
    
        let (|Contains|_|) needle (haystack: string) = 
            if haystack.Contains(needle) then Some()
            else None
    
        let (|MatchesPattern|_|) pattern code = 
            let mat = Regex.Match(code, pattern, RegexOptions.Compiled ||| RegexOptions.IgnoreCase)
            if mat.Success then Some(getGroup mat)
            else None

        let (|NumericString|_|) string =
            let didParse, value = Decimal.TryParse(string)
            if didParse then Some(value) else None
            
        // ===== Lanes and Districts ========================

        // Parses the district IDs out of the Location Code, placing them into
        // the filter.  Returns the mutated code and filter.
        let parseDistricts (code, (filter: LocationFilterMessage)) =
            let mat = getGroup (Regex.Match(code, LocationPatterns.districtIdsPattern))
            filter.DistrictIds <- (mat LocationPatterns.DISTRICT_IDS_KEY)
                                       .ToString()
                                       .Split(',')
                                       .Select(fun d -> d.Trim())
                                       .ToList()
            (mat LocationPatterns.PREDECESSOR_GROUP_KEY, filter)

        // Returns a location code with any lanes affected removed and
        // a mutated LocationFilterMessage with lanes assigned to it.
        let parseLanes (code, (filter: LocationFilterMessage)) =
            // Uses a single string representing a list of lanes
            // to make a list of lane designation strings.
            let parseOneSideLanes (str: string) =
                Regex.Split(str.Trim(), LocationPatterns.lanesAffectedSingle)
                     .ToList()
            
            let mat = getGroup (Regex.Match(code, LocationPatterns.lanesAffectedPattern))
            (mat LocationPatterns.LANES_AFFECTED_KEY).Split(':')
            |> function
                | [| one; two; |] -> filter.LanesAffectedLeft <- parseOneSideLanes one
                                     filter.LanesAffectedRight <- parseOneSideLanes two
                | [| one; |] -> filter.LanesAffectedUnknown <- parseOneSideLanes one
                | _ -> ()
            (mat LocationPatterns.PREDECESSOR_GROUP_KEY, filter)
            
        // ===== Highway/Route/Alias ========================

        let parseHighwayRoute str (filter: LocationFilterMessage) =
            match str with
            | MatchesPattern LocationPatterns.highwayPattern li -> filter.HighwayNumber <- str
            | MatchesPattern LocationPatterns.routePattern li -> filter.RouteNumber <- str
            | _ -> filter.Alias <- str
            filter

        // ===== Cross Feature ==============================

        let parseRightSide str (filter: LocationFilterMessage) =
            // First trim districts and lanes from the location code
            // and assign the parsed values to the filter
            let trimmedCode, newFilter =
                (parseDistricts >> parseLanes) (str, filter)        
            newFilter.LocationCode <- trimmedCode

            // Assign road direction code, and put the remainder in cross feature.
            let mat = getGroup (Regex.Match(trimmedCode, LocationPatterns.roadDirCodePattern))
            newFilter.RoadDirectionCode <- mat LocationPatterns.ROAD_DIRECTION_CODE_KEY
            newFilter.CrossFeature <- mat LocationPatterns.PREDECESSOR_GROUP_KEY
            newFilter

        let parseXFeature (code: string) (filter: LocationFilterMessage) = 
            code.Split('/')
            |> function
                | [| one; two |] -> parseHighwayRoute one filter
                                    |> parseRightSide two
                | _ -> filter

        // ===== Lat/Long ===================================

        let parseLatLng (code: string) (filter: LocationFilterMessage) =
            code.Replace("=", "")
                .Split(',')
            |> function
                | [| one; two |] -> filter.Latitude <- convertToNullableDecimal one
                                    filter.Longitude <- convertToNullableDecimal two
                | _ -> ()
            filter

        // ===== Milepoint ==================================

        // Support function for parsing milepoints specifically.
        let parseMilePoints (mps: string) (filter: LocationFilterMessage) = 
            match mps.Split('-') with
            | [|one; two|] -> filter.BeginMilepoint <- convertToNullableDecimal one
                              filter.EndMilepoint <- convertToNullableDecimal two
            | [|one|] -> filter.BeginMilepoint <- convertToNullableDecimal one
            | _ -> ()
            filter

        // Parses entire milepoint / highway/route/alias location code.
        let parseMilePointCode code (filter: LocationFilterMessage) =
            // the parse methods mutate the filter but I am pretending they don't
            // ...because.
            let newCode, filter = (parseDistricts >> parseLanes) (code, filter)
            let m = (Regex.Match(newCode, LocationPatterns.begMPPattern))
            
            if m.Success then
                let startMP = getGroup m LocationPatterns.MILEPOINT_GROUP_KEY
                if (startMP.StartsWith("z", StringComparison.OrdinalIgnoreCase))
                    then filter.IsZMilepoint <- true

                parseMilePoints startMP filter
                |> parseHighwayRoute (getGroup m LocationPatterns.SUCCESSOR_GROUP_KEY)
            else filter

        // ===== Districts ==================================

        let handleDistricts allDistricts (filter: LocationFilterMessage) =
            if allDistricts then filter.DistrictIds <- [].ToList() // empty district list means no filtering
            else filter.DistrictIds <- [ "8"; "10"; "11"; ].ToList() // use some profile default in reality
            filter
    
        // ===== Top-level logic=============================

        let parse code (filter: LocationFilterMessage) = 
            match code with
            | Contains "/" () -> parseXFeature code filter
            | StartsWith "=" () -> parseLatLng code filter
            | _ -> parseMilePointCode code filter

        let filter = new LocationFilterMessage()
        match code with
        | Null -> filter
        | _ -> parse (code.Trim()) filter
               |> handleDistricts allDistricts