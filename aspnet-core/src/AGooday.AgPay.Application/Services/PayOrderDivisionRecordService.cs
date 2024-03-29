﻿using AGooday.AgPay.Application.DataTransfer;
using AGooday.AgPay.Application.Interfaces;
using AGooday.AgPay.Common.Enumerator;
using AGooday.AgPay.Domain.Core.Bus;
using AGooday.AgPay.Domain.Interfaces;
using AGooday.AgPay.Domain.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AGooday.AgPay.Application.Services
{
    public class PayOrderDivisionRecordService : IPayOrderDivisionRecordService
    {
        // 注意这里是要IoC依赖注入的，还没有实现
        private readonly IPayOrderDivisionRecordRepository _payOrderDivisionRecordRepository;
        // 用来进行DTO
        private readonly IMapper _mapper;
        // 中介者 总线
        private readonly IMediatorHandler Bus;

        public PayOrderDivisionRecordService(IPayOrderDivisionRecordRepository payOrderDivisionRecordRepository, IMapper mapper, IMediatorHandler bus)
        {
            _payOrderDivisionRecordRepository = payOrderDivisionRecordRepository;
            _mapper = mapper;
            Bus = bus;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public bool Add(PayOrderDivisionRecordDto dto)
        {
            var m = _mapper.Map<PayOrderDivisionRecord>(dto);
            _payOrderDivisionRecordRepository.Add(m);
            return _payOrderDivisionRecordRepository.SaveChanges(out int _);
        }

        public bool Remove(long recordId)
        {
            _payOrderDivisionRecordRepository.Remove(recordId);
            return _payOrderDivisionRecordRepository.SaveChanges(out int _);
        }

        public bool Update(PayOrderDivisionRecordDto dto)
        {
            var m = _mapper.Map<PayOrderDivisionRecord>(dto);
            _payOrderDivisionRecordRepository.Update(m);
            return _payOrderDivisionRecordRepository.SaveChanges(out int _);
        }

        public PayOrderDivisionRecordDto GetById(long recordId)
        {
            var entity = _payOrderDivisionRecordRepository.GetById(recordId);
            var dto = _mapper.Map<PayOrderDivisionRecordDto>(entity);
            return dto;
        }

        public PayOrderDivisionRecordDto GetById(long recordId, string mchNo)
        {
            var entity = _payOrderDivisionRecordRepository.GetAll().Where(w => w.RecordId.Equals(recordId) && w.MchNo.Equals(mchNo)).FirstOrDefault();
            return _mapper.Map<PayOrderDivisionRecordDto>(entity);
        }

        public IEnumerable<PayOrderDivisionRecordDto> GetAll()
        {
            var payOrderDivisionRecords = _payOrderDivisionRecordRepository.GetAll();
            return _mapper.Map<IEnumerable<PayOrderDivisionRecordDto>>(payOrderDivisionRecords);
        }

        public PaginatedList<PayOrderDivisionRecordDto> GetPaginatedData(PayOrderDivisionRecordQueryDto dto)
        {
            var mchInfos = _payOrderDivisionRecordRepository.GetAll()
                .Where(w => (string.IsNullOrWhiteSpace(dto.MchNo) || w.MchNo.Equals(dto.MchNo))
                && (string.IsNullOrWhiteSpace(dto.IsvNo) || w.IsvNo.Equals(dto.IsvNo))
                && (dto.ReceiverId.Equals(0) || w.ReceiverId.Equals(dto.ReceiverId))
                && (dto.ReceiverGroupId.Equals(0) || w.ReceiverGroupId.Equals(dto.ReceiverGroupId))
                && (string.IsNullOrWhiteSpace(dto.PayOrderId) || w.PayOrderId.Equals(dto.PayOrderId))
                && (string.IsNullOrWhiteSpace(dto.AccNo) || w.AccNo.Equals(dto.AccNo))
                && (string.IsNullOrWhiteSpace(dto.AppId) || w.AppId.Equals(dto.AppId))
                && (dto.State.Equals(null) || w.State.Equals(dto.State))
                && (dto.CreatedEnd == null || w.CreatedAt < dto.CreatedEnd)
                && (dto.CreatedStart == null || w.CreatedAt >= dto.CreatedStart)
                ).OrderByDescending(o => o.CreatedAt);
            var records = PaginatedList<PayOrderDivisionRecord>.Create<PayOrderDivisionRecordDto>(mchInfos.AsNoTracking(), _mapper, dto.PageNumber, dto.PageSize);
            return records;
        }

        public void UpdateRecordSuccessOrFail(List<PayOrderDivisionRecordDto> records, byte state, string channelBatchOrderId, string channelRespResult)
        {
            if (records == null || !records.Any())
            {
                return;
            }
            var recordIds = records.Select(s => s.RecordId);

            var updateRecords = _payOrderDivisionRecordRepository.GetAll().Where(w => recordIds.Contains(w.RecordId) && w.State.Equals((byte)PayOrderDivisionState.STATE_WAIT));
            foreach (var updateRecord in updateRecords)
            {
                updateRecord.State = state;
                updateRecord.ChannelBatchOrderId = channelBatchOrderId;
                updateRecord.ChannelRespResult = channelRespResult;
                _payOrderDivisionRecordRepository.Update(updateRecord);
            }
            _payOrderDivisionRecordRepository.SaveChanges(out int _);
        }
    }
}
