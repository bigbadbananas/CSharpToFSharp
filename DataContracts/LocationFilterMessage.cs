using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace OdotTad.Tocs.Shared.DataContracts.Location
{
    /// <summary>
    /// Represents the parameters of a location search.
    /// Location codes are parsed into these objects.
    /// </summary>
    [DataContract]
    public class LocationFilterMessage
    {
        public LocationFilterMessage()
        {
            DistrictIds = new List<string>();
            LanesAffectedLeft = new List<string>();
            LanesAffectedRight = new List<string>();
            LanesAffectedUnknown = new List<string>();
            LatLong = new Tuple<decimal?, decimal?>(null, null);
        }

        [DataMember]
        public string LocationCode { get; set; }

        /// <summary>
        /// Gets or sets a tuple representing the latitude and longitude respectively.
        /// </summary>
        [DataMember]
        public Tuple<decimal?, decimal?> LatLong { get; set; }

        [DataMember]
        public string HighwayNumber { get; set; }

        [DataMember]
        public string RouteNumber { get; set; }

        [DataMember]
        public List<string> DistrictIds { get; set; }

        [DataMember]
        public List<string> LanesAffectedLeft { get; set; }

        [DataMember]
        public List<string> LanesAffectedRight { get; set; }

        [DataMember]
        public List<string> LanesAffectedUnknown { get; set; }

        [DataMember]
        public string TocsUserId { get; set; }

        [DataMember]
        public string RoadDirectionCode { get; set; }

        [DataMember]
        public decimal? BeginMilepoint { get; set; }

        [DataMember]
        public decimal? EndMilepoint { get; set; }

        [DataMember]
        public bool IsZMilepoint { get; set; }

        [DataMember]
        public string CountyCode { get; set; }

        [DataMember]
        public string CityCode { get; set; }

        [DataMember]
        public string Alias { get; set; }

        [DataMember]
        public string CrossFeature { get; set; }

        [DataMember]
        public string LandmarkName { get; set; }

        [DataMember]
        public string HouseNumber { get; set; }

        [DataMember]
        public bool IncludeNonMainline { get; set; }
    }
}
