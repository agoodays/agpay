﻿using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Infrastructure.Repositories
{
    public class MchInfoRepository : Repository<MchInfo>, IMchInfoRepository
    {
        public MchInfoRepository(AgPayDbContext context)
            : base(context)
        {
        }

        public bool IsExistMchNo(string mchNo)
        {
            return DbSet.AsNoTracking().Any(c => c.MchNo.Equals(mchNo));
        }
    }
}
