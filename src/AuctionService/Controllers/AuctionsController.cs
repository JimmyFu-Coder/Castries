
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/auctions")]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDBContext _context;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;


        public AuctionsController(AuctionDBContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;

        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
        {
            var auctions = await _context.Auctions
            .Include(x => x.Item)
            .OrderBy(x => x.Item.Make)
            .ToListAsync();

            return _mapper.Map<List<AuctionDto>>(auctions);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction = await _context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
            {
                return NotFound();
            }

            return _mapper.Map<AuctionDto>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto auctionDto)
        {
            var auction = _mapper.Map<Auction>(auctionDto);
            // TODO: add Seller
            auction.Seller = "test";
            _context.Auctions.Add(auction);
            var result = await _context.SaveChangesAsync() > 0;
            var newAuction = _mapper.Map<AuctionDto>(auction);
            await _publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));
            if (!result)
            {
                return BadRequest("Failed to create auction");
            }

            return CreatedAtAction(nameof(GetAuctionById), new { id = newAuction.Id }, newAuction);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AuctionDto>> UpdateAuction(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);
            if (auction == null)
            {
                return NotFound();
            }
            //  TODO: check Seller

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;

            var result = await _context.SaveChangesAsync() > 0;
            if (!result)
            {
                return BadRequest("Failed to update auction");
            }
            return _mapper.Map<AuctionDto>(auction);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);
            if (auction == null)
            {
                return NotFound();
            }
            _context.Auctions.Remove(auction);
            var result = await _context.SaveChangesAsync() > 0;
            if (!result)
            {
                return BadRequest("Failed to delete auction");
            }
            return Ok();
        }
    }
}