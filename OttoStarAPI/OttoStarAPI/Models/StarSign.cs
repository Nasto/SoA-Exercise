using System;

namespace OttoStar.Models
{
    public class StarSign
    {
        /// <summary>
        /// The unique Name of a Zodiac Sign
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The date on which this StarSign becomes the current one
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The last date on which this StarSign is the current one.
        /// </summary>
        public DateTime EndDate { get; set; }

    }
}