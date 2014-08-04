namespace LocationCodeParser.LocationPatterns

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

    let routePattern = String.Format("^(?<{0}>{1}", ROUTE_GROUP_KEY, @"((OR|I|US)-*)?\d{1,3}\w?)$")
    let highwayPattern = String.Format("^(?<{0}>{1}", HIGHWAY_GROUP_KEY, @"(H\d{1,3}))$")
    let highwayRoutePattern = String.Format("{0}|{1}", highwayPattern, routePattern)

    let begMPPattern = 
        String.Format("(?<{0}>{1}{2}", MILEPOINT_GROUP_KEY, 
                                       @"^z?([^@=\[^A-Ya-y\]](\d{1,3})?([.]\d{1,3})?)(-(\d{1,3})?([.]\d{1,3})?)?)",
                                       String.Format("(?<{0}>(.*))", SUCCESSOR_GROUP_KEY))

    let roadDirCodePattern = 
        String.Format("(?<{0}>(.*)){1}", PREDECESSOR_GROUP_KEY,
                                         String.Format("(?<{0}>{1}", ROAD_DIRECTION_CODE_KEY, 
                                                                     @"(\b(NB|EB|WB|SB)\b))"))

    let lanesAffectedSingle = @"OF|ON|RA|All|[A-FIMSX]"
    let lanesAffectedPattern = 
        String.Format("(?<{0}>(.*)){1}", PREDECESSOR_GROUP_KEY,
                                         String.Format("(?<{0}>{1}", LANES_AFFECTED_KEY,
                                                                     @"(?<=\s)(OF|ON|RA|All|[A-FIMSX:])+(?=\s|$))"))

    let districtIdsPattern = 
        String.Format("(?<{0}>(.*)){1}", PREDECESSOR_GROUP_KEY,
                                         String.Format("(?<{0}>{1}", DISTRICT_IDS_KEY,
                                                                     @"(?<=\s)(-[\w\d]{1,2}(,\s?[\w\d]{1,2})*$))"))
    let regionIdPattern = String.Format("(?<{0}>{1}", REGION_ID_KEY, @"(?<=R)\d")
    let latLngPattern = String.Format("^(?<{0}>{1}", LATLONG_GROUP_KEY, @"(=-?\d+(.\d+)?\s*,\s*-?\d+(.\d+)?))$")

    let addressToken = String.Format("^(?<{0}>{1}", ADDRESS_GROUP_KEY, @"(@[\w\d,.\s*]*))$")
    let houseNumberToken = String.Format("(?<{0}>{1}", HOUSE_NUMBER_GROUP_KEY, @"(?<=^@)[\d\w]{1,10})")
    let streetToken = String.Format("(?<{0}>{1}", STREET_GROUP_KEY, @"(?<=@[\d\w]{1,10}\s).*$)")
    let streetFirstWordToken = String.Format("(?<{0}>{1}", STREET_FIRST_WORD_KEY, @"^[^,]*)")