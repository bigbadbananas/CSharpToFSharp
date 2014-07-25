namespace Parser

/// <summary>
/// Contains Regex patterns used for parsing location codes.
/// </summary>
module LocationPatterns = 
    open System

    let ADDRESS_GROUP_KEY = "Address"
    let HOUSE_NUMBER_GROUP_KEY = "HouseNumber"
    let STREET_GROUP_KEY = "Street"
    let STREET_FIRST_WORD_KEY = "StreetFirstWord"
    let HIGHWAY_GROUP_KEY = "HighwayNumber"
    let LATLONG_GROUP_KEY = "LatLong"
    let MILEPOINT_GROUP_KEY = "Milepoint"
    let PREDECESSOR_GROUP_KEY = "Predecessor"
    let ROAD_DIRECTION_CODE_KEY = "RoadDirectionCode"
    let LANES_AFFECTED_KEY = "LanesAffected"
    let DISTRICT_IDS_KEY = "DistrictIds"
    let REGION_ID_KEY = "RegionCode"
    let ROUTE_GROUP_KEY = "Route"
    let SUCCESSOR_GROUP_KEY = "Successor"

    let rteToken = String.Format("^(?<{0}>", ROUTE_GROUP_KEY) + @"((OR|I|US)-*)?\d{1,3}\w?)$"

    let hwyNumToken = String.Format("^(?<{0}>", HIGHWAY_GROUP_KEY) + @"(H\d{1,3}))$"

    let hwyRteToken = String.Format("{0}|{1}", hwyNumToken, rteToken)

    let begMPToken = 
        String.Format("(?<{0}>", MILEPOINT_GROUP_KEY) +
        @"^z?([^@=\[^A-Ya-y\]](\d{1,3})?([.]\d{1,3})?)(-(\d{1,3})?([.]\d{1,3})?)?)" + 
        String.Format("(?<{0}>(.*))", SUCCESSOR_GROUP_KEY)

    let roadDirCodeToken = 
        String.Format("(?<{0}>(.*))", PREDECESSOR_GROUP_KEY) +
        String.Format("(?<{0}>", ROAD_DIRECTION_CODE_KEY) + @"(\b(NB|EB|WB|SB)\b))"

    let lanesAffectedSingle = @"OF|ON|RA|All|[A-FIMSX]"

    let lanesAffectedToken = 
        String.Format("(?<{0}>(.*))", PREDECESSOR_GROUP_KEY) +
        String.Format("(?<{0}>", LANES_AFFECTED_KEY) +
        @"(?<=\s)(OF|ON|RA|All|[A-FIMSX:])+(?=\s|$))"

    let districtIdsToken = 
        String.Format("(?<{0}>(.*))", PREDECESSOR_GROUP_KEY) +
        String.Format("(?<{0}>", DISTRICT_IDS_KEY) + @"(?<=\s)(-[\w\d]{1,2}(,\s?[\w\d]{1,2})*$))"

    let regionIdToken = String.Format("(?<{0}>", REGION_ID_KEY) + @"(?<=R)\d"

    let latLngToken = String.Format("^(?<{0}>", LATLONG_GROUP_KEY) + @"(=-?\d+(.\d+)?\s*,\s*-?\d+(.\d+)?))$"

    let addressToken = String.Format("^(?<{0}>", ADDRESS_GROUP_KEY) + @"(@[\w\d,.\s*]*))$"

    let houseNumberToken = String.Format("(?<{0}>", HOUSE_NUMBER_GROUP_KEY) + @"(?<=^@)[\d\w]{1,10})"

    let streetToken = String.Format("(?<{0}>", STREET_GROUP_KEY) + @"(?<=@[\d\w]{1,10}\s).*$)"

    let streetFirstWordToken = String.Format("(?<{0}>", STREET_FIRST_WORD_KEY) + @"^[^,]*)"

/// <summary>
/// Contains all Location Code Parsing logic.
/// </summary>
module LocationCodeParser = 
    open System
    open System.Text.RegularExpressions
    open Interop
    open LocationPatterns

    let (|XFeature|) (code:string) = code.Contains("/")

    let (|ParseMP|) (code:string) = Regex.IsMatch(code, "", RegexOptions.Compiled ||| RegexOptions.IgnoreCase)

    let (|StartsWith|_|) identifier (code:string) = if code.StartsWith(identifier) then Some() else None

    let (|Contains|_|) identifier (code:string) = if code.Contains(identifier) then Some() else None

    let (|MatchesPattern|_|) pattern code = 
        if Regex.IsMatch(code, pattern, RegexOptions.Compiled ||| RegexOptions.IgnoreCase)
        then Some()
        else None

    let parseXFeature code = 
        printfn ""

    let parseMP code =
        printfn ""

    let parseLatLng code =
        printfn ""

    let parseAddress code = 
        printfn ""

    let handleDistricts allDistricts code =
        printf ""

    ///<summary>
    /// The parser proper.
    ///<summary>
    ///<remarks>
    /// The parameter is now an f# type, thus it is immutable and non-null.
    ///</remarks>
    let parse allDistricts code = 
        match code with
        | StartsWith "/" () -> parseXFeature code
        | StartsWith "=" () -> parseLatLng code
        | StartsWith "@" () -> parseAddress code
        | MatchesPattern begMPToken () -> parseMP code
        | _ -> printfn "lolwhoops"
    
    let parseLocationCode (code:string, allDistricts:bool) = 
        match code with
        | Null code -> printfn ""
        | _ -> code.Trim() 
               |> parse 
               |> handleDistricts allDistricts