namespace DealFinder.Response
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    public class DepartureDateAndDurations
    {
        public DepartureDateAndDurations() { }
        public DepartureDateAndDurations(DateTime departureDate, string durationsCsv)
        {
            DepartureDate = departureDate;

            if (!string.IsNullOrEmpty(durationsCsv))
            {
                var durations = new List<int>();
                foreach (string dur in durationsCsv.Split(','))
                    if (int.TryParse(dur, out int duration))
                        durations.Add(duration);

                Durations = durations.ToArray();
            }
            else
            {
                Durations = Array.Empty<int>();
            }
        }

        public DateTime DepartureDate { get; set; }
        [XmlArrayItem("Duration")]
        public int[] Durations { get; set; }
    }
}
