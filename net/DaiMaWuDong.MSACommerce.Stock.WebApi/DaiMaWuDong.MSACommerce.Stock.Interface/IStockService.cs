using DaiMaWuDong.MSACommerce.Stock.DTOModel.DTO;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaiMaWuDong.MSACommerce.Stock.Interface
{
    public interface IStockService
    {
        void IncreaseStock();
        void DecreaseStock(IDbContextTransaction trans, List<CartDto> cartDtos, long orderId);
        void ResumeStock(List<CartDto> cartDtos, long orderId);
    }
}
