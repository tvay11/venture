using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using stock.Model.DTO;
using stock.Model.Entity;

namespace stock.Controllers;

public partial class StockController
{
                [HttpPost("addstock")]
            public async Task<ActionResult<StockResponseDto>> CreateStock([FromBody] StockCreateDto stockCreateDto)
            {
                var stock = new Stock
                {
                    Symbol = stockCreateDto.Symbol,
                    Name = stockCreateDto.Name,
                    StockPrices = stockCreateDto.Prices.Select(p => new StockPrice
                    {
                        Date = p.Date,
                        Price = p.Price
                    }).ToList()
                };

                var success = await _stockService.AddStockAsync(stock);
                if (!success)
                {
                    return BadRequest("Stock name or symbol already exists.");
                }

                var responseDto = new StockResponseDto
                {
                    StockId = stock.Id,
                    Symbol = stock.Symbol,
                    Name = stock.Name,
                    Prices = stock.StockPrices.Select(sp => new StockPriceDto
                    {
                        Date = sp.Date,
                        Price = sp.Price
                    }).ToList()
                };
                return Ok(responseDto);
            }

            [HttpPost("user/{userId}/buy/{stockId}/{quantity}")]
            public async Task<IActionResult> BuyStock(int userId, int stockId, int quantity)
            {
                var userProfile = await _context.UserProfiles.FindAsync(userId);
                var stock = await _context.Stocks
                    .Include(s => s.StockPrices)
                    .FirstOrDefaultAsync(s => s.Id == stockId);

                if (userProfile == null || stock == null)
                    return NotFound("User or stock not found.");

                var currentPrice = await GetStockPriceByDate(stockId, userProfile.CurrentDate);

                if (currentPrice <= 0) 
                    return BadRequest("Current stock price is not available.");

                decimal totalCost = currentPrice * quantity;

                if (userProfile.Cash < totalCost)
                    return BadRequest("Insufficient funds.");

                userProfile.Cash -= totalCost;

                var existingHolding = await _context.StockHoldings
                    .FirstOrDefaultAsync(sh => sh.UserId == userId && sh.StockId == stockId);

                if (existingHolding != null)
                {
                    existingHolding.Quantity += quantity;
                }
                else
                {
                    var newHolding = new StockHolding
                    {
                        UserId = userId,
                        StockId = stockId,
                        Quantity = quantity
                    };
                    _context.StockHoldings.Add(newHolding);
                }

                await _context.SaveChangesAsync();
                return Ok("Stock purchased successfully.");
            }


            [HttpPost("user/{userId}/sell/{stockId}/{quantity}")]
            public async Task<IActionResult> SellStock(int userId, int stockId, int quantity)
            {
                var (success, message) = await SellStockForUser(userId, stockId, quantity);

                if (!success)
                {
                    return NotFound(message);
                }

                return Ok(message);
            }
            
            [HttpPost("user/{userId}/nextday")]
            public async Task<IActionResult> NextDay(int userId)
            {
                var userProfile = await _context.UserProfiles
                    .FirstOrDefaultAsync(up => up.UserId == userId);

                if (userProfile == null)
                {
                    return NotFound("User profile not found.");
                }

                userProfile.CurrentDate = userProfile.CurrentDate.AddDays(1);

                decimal holding = await CalculateStockHoldingsValue(userId);
                userProfile.NetWorth = holding + userProfile.Cash;

                await _context.SaveChangesAsync();

                string message = $"Current date updated to {userProfile.CurrentDate:yyyy-MM-dd}.";
                return Ok(message);
            }

            [HttpPost("prevday")]
            public async Task<IActionResult> PrevDay()
            {
                const int userId = 1;

                var userProfile = await _context.UserProfiles
                    .Include(up => up.StockHoldings)
                    .ThenInclude(sh => sh.Stock)
                    .ThenInclude(s => s.StockPrices)
                    .FirstOrDefaultAsync(up => up.UserId == userId);

                if (userProfile == null)
                    return NotFound("User profile not found.");
                
                userProfile.CurrentDate = userProfile.CurrentDate.AddDays(-1);
        
                decimal totalStockValue = 0;
                foreach (var holding in userProfile.StockHoldings)
                {
                    var currentPrice = await  GetStockPriceByDate(holding.StockId, userProfile.CurrentDate);
                    if (currentPrice != -1)
                    {
                        totalStockValue += currentPrice * holding.Quantity;
                    }
                }
                
                var userHistory = new UserHistory
                {
                    UserId = userId,
                    Date = userProfile.CurrentDate,
                    NetWorth = userProfile.Cash + totalStockValue
                };
                _context.UserHistories.Add(userHistory);
                
                userProfile.NetWorth = userProfile.Cash + totalStockValue;

                await _context.SaveChangesAsync();
                return Ok($"Net worth updated for date {userProfile.CurrentDate:yyyy-MM-dd}.");
            }
            
            [HttpPost("setcurrentdatetotoday/{userId}")]
            public async Task<IActionResult> SetCurrentDateToToday(int userId)
            {
                var userProfile = await _context.UserProfiles.FindAsync(userId);
                if (userProfile == null)
                {
                    return NotFound($"User with ID {userId} not found.");
                }

                userProfile.CurrentDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok($"Current date set to today for user with ID {userId}.");
            }
}