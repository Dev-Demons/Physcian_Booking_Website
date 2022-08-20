using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Binke.Api.ViewModels
{
    public class SelectIdValueModel
    {
        public int Id { get; set; }
        public string StrId { get; set; }
        public string Value { get; set; }
    }

    public class SearchParameterModel
    {
        //public Distance Distance { get; set; }
        public string DistanceType { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string SearchBox { get; set; }
        public bool ANP { get; set; }
        public bool NTPA { get; set; }
        public bool PrimaryCare { get; set; }
        public int Specialties { get; set; }
        public List<int> Affiliations { get; set; }
        public List<int> Insurance { get; set; }
        //public AGS AGS { get; set; }
        public List<int> AgeGroup { get; set; }
        public List<int> AGS { get; set; }
        public int Language { get; set; }
        public string Sorting { get; set; }
        public string Search { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
    //public class Distance
    //{
    //    public string DistanceType { get; set; }
    //    public string Latitude { get; set; }
    //    public string Longitude { get; set; }
    //    public string SearchBox { get; set; }
    //}

    public class SearchParamModel
    {
        //public SearchParamModel()
        //{
        //    Distance = new Distance();
        //}

        //public Distance Distance { get; set; }

        public string DistanceType { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string SearchBox { get; set; }
        public string Sorting { get; set; }
        public int Id { get; set; }
        public string entity { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string locationSearch { get; set; }
    }
}