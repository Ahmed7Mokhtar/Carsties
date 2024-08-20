using System;
using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;

namespace SearchService.Consumers;

public class AuctionCreatedConsumer : IConsumer<AuctionCreated> // Mass Transit convension is to use Consumer at the end of the class name
{
    private readonly IMapper _mapper;

    public AuctionCreatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine("---> Consuming Auction Created: " + context.Message.Id);

        var item = _mapper.Map<Item>(context.Message);

        if(item.Model == "Foo")
            throw new ArgumentException("Can't sell Cars With Name Foo!");

        await item.SaveAsync();
    }
}
