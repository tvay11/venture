    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using stock.Model;
    using stock.Data;
    using stock.Model.DTO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Cors;
    using stock.Model.Entity;
    using stock.Service;

    namespace stock.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public partial class StockController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly StockService _stockService;

        public StockController(ApplicationDbContext context, StockService stockService)
        {
            _context = context;
            _stockService = stockService;
        }
        

            
            


    
        }