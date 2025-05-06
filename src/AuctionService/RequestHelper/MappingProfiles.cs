
using AuctionService.Entities;
using AuctionService.DTOs;
using AutoMapper;

namespace AuctionService.RequestHelper
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles()
        {
            CreateMap<Auction, AuctionDto>().IncludeMembers(x => x.Item);
            CreateMap<Item, AuctionDto>();
            
        }
    }
}