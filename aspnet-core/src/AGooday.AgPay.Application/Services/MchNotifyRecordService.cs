﻿using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Application.ViewModels;
using AGooday.AgPay.Domain.Commands.SysUsers;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AGooday.AgPay.Infrastructure.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGooday.AgPay.Application.Services
{
    public class MchNotifyRecordService: IMchNotifyRecordService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IMchNotifyRecordRepository _mchNotifyRecordRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public MchNotifyRecordService(IMchNotifyRecordRepository mchNotifyRecordRepository, IMapper mapper, IMediatorHandler bus)
        {
            _mchNotifyRecordRepository = mchNotifyRecordRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Add(MchNotifyRecordVM vm)
        {
            var m = _mapper.Map<MchNotifyRecord>(vm);
            _mchNotifyRecordRepository.Add(m);
            _mchNotifyRecordRepository.SaveChanges();
        }

        public void Remove(long recordId)
        {
            _mchNotifyRecordRepository.Remove(recordId);
            _mchNotifyRecordRepository.SaveChanges();
        }

        public void Update(MchNotifyRecordVM vm)
        {
            var m = _mapper.Map<MchNotifyRecord>(vm);
            _mchNotifyRecordRepository.Update(m);
            _mchNotifyRecordRepository.SaveChanges();
        }

        public MchNotifyRecordVM GetById(long recordId)
        {
            var entity = _mchNotifyRecordRepository.GetById(recordId);
            var vm = _mapper.Map<MchNotifyRecordVM>(entity);
            return vm;
        }

        public IEnumerable<MchNotifyRecordVM> GetAll()
        {
            var mchNotifyRecords = _mchNotifyRecordRepository.GetAll();
            return _mapper.Map<IEnumerable<MchNotifyRecordVM>>(mchNotifyRecords);
        }
    }
}
