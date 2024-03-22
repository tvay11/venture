using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stock.Model.DTO;
using stock.Model.Entity;

namespace stock.Controllers;

public partial class StockController
{
    private DateTime GetCurrentDateForUser(int userId)
    {
        var userProfile = _context.UserProfiles
            .Where(up => up.UserId == userId)
            .Select(up => up.CurrentDate)
            .FirstOrDefault();

        if (userProfile == default(DateTime))
        {
            throw new Exception($"User with ID {userId} not found or CurrentDate not set.");
        }

        return userProfile;
    }
    
                private async Task<(bool Success, string Message)> SellStockForUser(int userId, int stockId, int quantity)
            {
                var userProfile = await _context.UserProfiles.FindAsync(userId);
                var stock = await _context.Stocks
                    .Include(s => s.StockPrices)
                    .FirstOrDefaultAsync(s => s.Id == stockId);

                if (userProfile == null || stock == null)
                {
                    return (false, "User or stock not found.");
                }

                var currentPrice = await
                    GetStockPriceByDate(stockId, userProfile.CurrentDate); 
                if (currentPrice == null)
                {
                    return (false, "Current stock price is not available.");
                }

                var existingHolding = await _context.StockHoldings
                    .FirstOrDefaultAsync(sh => sh.UserId == userId && sh.StockId == stockId);

                if (existingHolding == null || existingHolding.Quantity < quantity)
                {
                    return (false, "Not enough stock to sell.");
                }

                decimal totalSaleValue = currentPrice  * quantity;
                userProfile.Cash += totalSaleValue;
                existingHolding.Quantity -= quantity;

                if (existingHolding.Quantity == 0)
                {
                    _context.StockHoldings.Remove(existingHolding);
                }

                await _context.SaveChangesAsync();
                return (true, "Stock sold successfully.");
            }
            private async Task<decimal> CalculateStockHoldingsValue(int userId)
            {
                var userProfile = await _context.UserProfiles
                    .Include(up => up.StockHoldings)
                    .ThenInclude(sh => sh.Stock)
                    .ThenInclude(s => s.StockPrices)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(up => up.UserId == userId);

                if (userProfile == null)
                {
                    throw new InvalidOperationException("User profile not found.");
                }

                decimal totalStockValue = 0;

                foreach (var holding in userProfile.StockHoldings)
                {
                    var currentPrice =await GetStockPriceByDate(holding.StockId, userProfile.CurrentDate);
                    if (currentPrice != -1)
                    {
                        totalStockValue += currentPrice * holding.Quantity;
                    }
                }

                return totalStockValue;
            }
            
            public async Task<IActionResult> CheckAndSellStocksAtLastDate(int userId)
            {
                var userProfile = await _context.UserProfiles
                    .Include(up => up.StockHoldings)
                    .ThenInclude(sh => sh.Stock)
                    .ThenInclude(s => s.StockPrices)
                    .FirstOrDefaultAsync(up => up.UserId == userId);

                if (userProfile == null)
                {
                    return NotFound("User profile not found.");
                }

                var currentDate = userProfile.CurrentDate;
                List<string> soldStockMessages = new List<string>();

                foreach (var holding in userProfile.StockHoldings)
                {
                    var lastPriceDate = holding.Stock.StockPrices.MaxBy(sp => sp.Date)?.Date;
                    if (lastPriceDate.HasValue && lastPriceDate.Value == currentDate)
                    {
                        var (success, message) = await SellStockForUser(userId, holding.StockId, holding.Quantity);
                        if (success)
                        {
                            soldStockMessages.Add(message);
                        }
                    }
                }

                await _context.SaveChangesAsync();

                if (soldStockMessages.Any())
                {
                    return Ok($"Stocks sold: {string.Join(", ", soldStockMessages)}");
                }
                else
                {
                    return Ok("No stocks were sold.");
                }
            }
            

            
            public async Task<decimal> GetStockPriceByDate(int stockId, DateTime userCurrentDate)
            {
                var stockPrice = await _context.StockPrices
                    .Where(sp => sp.StockId == stockId && sp.Date <= userCurrentDate)
                    .OrderByDescending(sp => sp.Date)
                    .FirstOrDefaultAsync();

                return stockPrice?.Price ?? -1; 
            }

}