using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using OttoStar.Models;
using Swashbuckle.Swagger.Annotations;

namespace OttoStar.Controllers
{
    public class StarSignController : ApiController
    {
        private const string FILENAME = "starsigns.json";
        private readonly GenericStorage _storage;

        public StarSignController()
        {
            _storage = new GenericStorage();
        }

        private async Task<IEnumerable<StarSign>> GetStarSigns()
        {
            var starSigns = await _storage.Get(FILENAME);

            if (starSigns == null)
            {
                await _storage.Save(new[]
                {
                    new StarSign
                    {
                        Name = "aquarius",
                        StartDate = new DateTime(2016, 01, 20),
                        EndDate = new DateTime(2016, 02, 19).AddSeconds(-1)
                    },
                    new StarSign
                    {
                        Name = "pisces",
                        StartDate = new DateTime(2016, 02, 19),
                        EndDate = new DateTime(2016, 03, 21).AddSeconds(-1)
                    },
                    new StarSign
                    {
                        Name = "aries",
                        StartDate = new DateTime(2016, 03, 21),
                        EndDate = new DateTime(2016, 04, 20).AddSeconds(-1)
                    },
                    new StarSign
                    {
                        Name = "taurus",
                        StartDate = new DateTime(2016, 04, 20),
                        EndDate = new DateTime(2016, 05, 21).AddSeconds(-1)
                    },
                    new StarSign
                    {
                        Name = "gemini",
                        StartDate = new DateTime(2016, 05, 21),
                        EndDate = new DateTime(2016, 06, 21).AddSeconds(-1)
                    },
                    new StarSign
                    {
                        Name = "cancer",
                        StartDate = new DateTime(2016, 06, 21),
                        EndDate = new DateTime(2016, 07, 23).AddSeconds(-1)
                    },
                    new StarSign
                    {
                        Name = "leo",
                        StartDate = new DateTime(2016, 07, 23),
                        EndDate = new DateTime(2016, 08, 23).AddSeconds(-1)
                    },
                    new StarSign
                    {
                        Name = "virgo",
                        StartDate = new DateTime(2016, 08, 23),
                        EndDate = new DateTime(2016, 09, 23).AddSeconds(-1)
                    },
                    new StarSign
                    {
                        Name = "libra",
                        StartDate = new DateTime(2016, 09, 23),
                        EndDate = new DateTime(2016, 10, 23).AddSeconds(-1)
                    },
                    new StarSign
                    {
                        Name = "scorpio",
                        StartDate = new DateTime(2016, 10, 23),
                        EndDate = new DateTime(2016, 11, 22).AddSeconds(-1)
                    },
                    new StarSign
                    {
                        Name = "sagittarius",
                        StartDate = new DateTime(2016, 11, 22),
                        EndDate = new DateTime(2016, 12, 22).AddSeconds(-1)
                    },
                    new StarSign
                    {
                        Name = "capricorn",
                        StartDate = new DateTime(2016, 12, 22),
                        EndDate = new DateTime(2016, 01, 20).AddSeconds(-1)
                    }
                }
                , FILENAME);
                starSigns = await _storage.Get(FILENAME);
            }



            return starSigns;
        }

        /// <summary>
        ///     Gets the list of starsigns
        /// </summary>
        /// <returns>The starsigns</returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Type = typeof(IEnumerable<StarSign>))]
        [Route("~/starsigns")]
        public async Task<IEnumerable<StarSign>> Get()
        {
            return await GetStarSigns();
        }

        /// <summary>
        ///     Gets a specific starsign
        /// </summary>
        /// <param name="name">Identifier for the starsign</param>
        /// <returns>The requested starsign</returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Description = "OK",
            Type = typeof(IEnumerable<StarSign>))]
        [SwaggerResponse(HttpStatusCode.NotFound,
            Description = "Star sign not found",
            Type = typeof(IEnumerable<StarSign>))]
        [SwaggerOperation("GetStarSignByName")]
        [Route("~/starsign/fromname/{name}")]
        public async Task<StarSign> GetFromName([FromUri] string name)
        {
            var starsigns = await GetStarSigns();
            return starsigns.FirstOrDefault(s => s.Name == name.ToLower());
        }

        /// <summary>
        ///     Gets the corresponding star sign to a date
        /// </summary>
        /// <param name="date">The date to get the star sign for</param>
        /// <returns>The corresponding star sign</returns>
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Type = typeof(IEnumerable<StarSign>))]
        [SwaggerOperation("GetStarSignFromDate")]
        [Route("~/starsign/bydate/{date}")]
        public async Task<StarSign> GetFromDate([FromUri] string date)
        {
            var searchDate = DateTime.Parse(date);
            searchDate = new DateTime(2016, searchDate.Month, searchDate.Day ); //dirty hack
            var starsigns = await GetStarSigns();
            return
                starsigns.FirstOrDefault(
                    s => s.StartDate.CompareTo(searchDate) < 0 && s.EndDate.CompareTo(searchDate) > 0);
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,
            Type = typeof(string))]
        [SwaggerOperation("GetHoroscope")]
        [Route("~/starsign/horoscope/{date}")]
        public async Task<string> GetHosroscope([FromUri] string date)
        {
            var starSign = GetFromDate(date).Result;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://widgets.fabulously40.com");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.GetAsync($"horoscope.json?sign={starSign.Name}");
                if (!response.IsSuccessStatusCode) return "could not retrieve horoscope";
                var horoscope = await response.Content.ReadAsAsync<Horoscope>();
                return horoscope.horoscope;
            }
        }
    }
}