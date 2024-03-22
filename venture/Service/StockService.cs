using Microsoft.EntityFrameworkCore;
using stock.Data;
using stock.Model.DTO;
using stock.Model.Entity;
namespace stock.Service;
public class StockService
{    private readonly ApplicationDbContext _context;

    public StockService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public StockResponseDto CreateStockResponseDto(Stock stock, UserProfile userProfile)
    {
        var responseDto = new StockResponseDto
        {
            StockId = stock.Id,
            Symbol = stock.Symbol,
            Name = stock.Name,
            CurrentDate = userProfile.CurrentDate,
            Prices = stock.StockPrices.Select(sp => new StockPriceDto
            {
                Date = sp.Date,
                Price = sp.Price
            }).ToList()
        };
        

        return responseDto;
    }
    
    private decimal? GetCurrentStockPrice(Stock stock, DateTime date)
    {
        var currentPriceInfo = stock.StockPrices
            .Where(sp => sp.Date <= date).MaxBy(sp => sp.Date);
        return currentPriceInfo?.Price;
    }
    
    public async Task<bool> AddStockAsync(Stock stock)
    {
        var existingStockWithName = await _context.Stocks
            .AnyAsync(s => s.Name == stock.Name);
        if (existingStockWithName)
        {
            return false;
        }
        
        var existingStockWithSymbol = await _context.Stocks
            .AnyAsync(s => s.Symbol == stock.Symbol);
        if (existingStockWithSymbol)
        {
            return false;
        }
        
        _context.Stocks.Add(stock);
        await _context.SaveChangesAsync();

        return true;
    }
}