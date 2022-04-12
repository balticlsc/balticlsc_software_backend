using System;
using System.Globalization;
using System.Text.Json.Serialization;

namespace Baltic.Server.Controllers.Models
{
    public class EarnedCreditRequest
    {
        private DateTime _startDate;

        [JsonPropertyName("StartDate")]
        public string StartDateProperty
        {
            get => _startDate.ToString("R", DateTimeFormatInfo.InvariantInfo);
            set => _startDate = DateTime.Parse(value, DateTimeFormatInfo.InvariantInfo) ;
        }

        [JsonIgnore]    //should be ignored by swagger but isn`t!!!
        public DateTime StartDate
        {
            get => _startDate;
            set => _startDate = value;
        }

        private DateTime _endDate;

        [JsonPropertyName("EndDate")]
        public string EndDateProperty
        {
            get => _endDate.ToString("R", DateTimeFormatInfo.InvariantInfo);
            set => _endDate = DateTime.Parse(value, DateTimeFormatInfo.InvariantInfo);
        }

        [JsonIgnore]    //should be ignored by swagger but isn`t!!!
        public DateTime EndDate
        {
            get => _endDate;
            set => _endDate = value;
        }

        public int Interval { get; set; }
    }
}
