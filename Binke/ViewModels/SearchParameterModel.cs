using System.Collections.Generic;

namespace Binke.ViewModels
{
    public class AgeGroupsSeen
    {
        public int Female { get; set; }
        public int Male { get; set; }
        public int Pediatrics { get; set; }
        public int Teenagers { get; set; }
        public int Adults { get; set; }
        public int Geriatrics { get; set; }
    }

    public class Distance
    {
        public string DistanceType { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string SearchBox { get; set; }
        public string SearchLocation { get; set; }
    }

    public class AGS
    {
        public bool Female { get; set; }
        public bool Male { get; set; }
    }

    public class AGSFull
    {
        public bool Adults { get; set; }
        public bool Geriatrics { get; set; }
        public bool Pediatrics { get; set; }
        public bool Teenagers { get; set; }
    }

    public class SearchParameterModel
    {
        public Distance Distance { get; set; }
        public bool ANP { get; set; }
        public bool NTPA { get; set; }
        public bool PrimaryCare { get; set; }
         public int Specialties { get; set; }
        public string Specialization { get; set; }
        public List<int> Affiliations { get; set; }
        public List<int> Insurance { get; set; }
        //public AGS AGS { get; set; }
        public List<int> AgeGroup { get; set; }
        public List<int> AGS { get; set; }
        public int Language { get; set; }
        public string Sorting { get; set; }
        public string Search { get; set; }
        public string locationSearch { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public List<int> GenderTypeIds { get; set; }
    }

    public class SearchParamModel
    {
        public SearchParamModel()
        {
            Distance = new Distance();
        }

        public Distance Distance { get; set; }
        public string SearchBox { get; set; }
        public string Sorting { get; set; }
        public int Id { get; set; }
        public string entity { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SearchLocation { get; set; }
        public string Specialities { get; set; }
    }

    public class SearchDrugModel
    {
        public string SearchBox { get; set; }
        public string Sorting { get; set; }
    }


}
