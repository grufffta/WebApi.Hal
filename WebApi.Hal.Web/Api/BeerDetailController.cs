﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Hal.Web.Api.Resources;
using WebApi.Hal.Web.Data;

namespace WebApi.Hal.Web.Api
{
    [Route("[controller]")]
    public class BeerDetailController : Controller
    {
        readonly IBeerDbContext beerDbContext;

        public BeerDetailController(IBeerDbContext beerDbContext)
        {
            this.beerDbContext = beerDbContext;
        }

        [HttpGet("{id}")]
        // GET beerdetail/5
        public BeerDetailRepresentation Get(int id)
        {
            var beer = beerDbContext.Beers
                                    .Include("Brewery") // lazy loading isn't on for this query; force loading
                                    .Include("BeerStyle")
                                    .Single(br => br.Id == id);

            var reviews = beerDbContext.Reviews
                                       .Where(r => r.Beer_Id == id)
                                       .ToList()
                                       .Select(s => new ReviewRepresentation
                                                    {
                                                        Id = s.Id,
                                                        Beer_Id = s.Beer_Id,
                                                        Title = s.Title,
                                                        Content = s.Content
                                                    })
                                       .ToList();

            var detail = new BeerDetailRepresentation
                         {
                             Id = beer.Id,
                             Name = beer.Name,
                             Style = new BeerStyleRepresentation {Id = beer.Style.Id, Name = beer.Style.Name},
                             Brewery = new BreweryRepresentation {Id = beer.Brewery.Id, Name = beer.Brewery.Name}
                         };

            if (reviews.Count > 0)
            {
                detail.Reviews = new List<ReviewRepresentation>();
                foreach (var review in reviews)
                    detail.Reviews.Add(review);
            }

            return detail;
        }

        [HttpPut("{id}")]
        // PUT beerdetail/5
        public void Put(int id, BeerDetailRepresentation beer)
        {
            // this is here just to see how the deserializer is working
            // we should get the links and all the embedded objects deserialized
            // we'd be better off creating a client to test the full deserializing, but this way is cheap for now
        }
    }
}