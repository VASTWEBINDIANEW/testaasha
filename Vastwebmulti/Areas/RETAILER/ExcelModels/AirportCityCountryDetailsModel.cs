using LinqToExcel.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Vastwebmulti.Areas.RETAILER.ExcelModels
{
    public class AirportCityCountryDetailsModel
    {
        [ExcelColumn("Airport Name")]
        public string AirportName { get; set; }
        [ExcelColumn("Aiport Code")]
        public string AirportCode { get; set; }
        [ExcelColumn("City Name")]
        public string CityName { get; set; }
        [ExcelColumn("City Code")]
        public string CityCode { get; set; }
        [ExcelColumn("Country Code")]
        public string CountryCode { get; set; }
        [ExcelColumn("Country name")]
        public string CountryName { get; set; }
        [ExcelColumn("Nationalty")]
        public string Nationalty { get; set; }
        [ExcelColumn("Currency")]
        public string Currency { get; set; }

    }
}