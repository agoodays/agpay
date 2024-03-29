﻿using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class IsvInfoRepository : Repository<IsvInfo>, IIsvInfoRepository
    {
        public IsvInfoRepository(AgPayDbContext context)
            : base(context)
        {
        }
        public bool IsExistIsvNo(string isvNo)
        {
            return DbSet.AsNoTracking().Any(c => c.IsvNo.Equals(isvNo));
        }
    }
}
