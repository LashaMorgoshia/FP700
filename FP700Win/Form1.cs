﻿using FP700KasaGe;
using FP700KasaGe.Commands;
using FP700KasaGe.Responses;
using FP700Win.Events;
using FP700Win.Implementation;
using FP700Win.Interfaces;
using FP700Win.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FP700Win
{
    public partial class Form1 : Form
    {
        private readonly FP700 _fp700;
        private readonly IMessageAggregator _messenger;
        private readonly List<ItemInfo> _items;

        public Form1()
        {
            InitializeComponent();

            _fp700 = new FP700("COM3");
            _messenger = new MessageAggregator();
            _items = new List<ItemInfo>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _items.Add(new ItemInfo() { Id = 1, Code = "YAVV-(NAYY)-R 3x240+120 mm² BLACK", Price = 3.75m, Qty = 12 });
            _items.Add(new ItemInfo() { Id = 2, Code = "LIHCH 4x2.5 mm² GREY RAL7001", Price = 12.10m, Qty = 20 });
            _items.Add(new ItemInfo() { Id = 3, Code = "NHXMH-J 3x2.5 mm²", Price = 0.6m, Qty = 125 });
            _items.Add(new ItemInfo() { Id = 4, Code = "ელ.სადენი N2XH 3X2.5", Price = 1.25m, Qty = 35 });
            this.dataGridView1.DataSource = _items;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Sale();

            //PrintXReport();
        }

        public void Sale()
        {
            try
            {
                OpenFiscalReceiptResponse response = this._fp700.OpenFiscalReceipt("001", "1");
                this._messenger.Publish<EcrRespondedEvent>(new EcrRespondedEvent(response));

                foreach (var item in _items)
                {
                    RegisterSaleResponse res = this._fp700.RegisterSale(item.Code, item.Price, item.Qty, 1, TaxCode.A);
                    this._messenger.Publish<EcrRespondedEvent>(new EcrRespondedEvent(res));
                }
                CalculateTotalResponse response2 = this._fp700.Total(PaymentMode.Cash);
                this._messenger.Publish<EcrRespondedEvent>(new EcrRespondedEvent(response2));
                CloseFiscalReceiptResponse response3 = this._fp700.CloseFiscalReceipt();
                this._messenger.Publish<EcrRespondedEvent>(new EcrRespondedEvent(response3));
            }
            catch (Exception ex)
            {
                this._messenger.Publish<EcrThrewExceptionEvent>(new EcrThrewExceptionEvent(ex));
            }
        }

        

        public void PrintZReport()
        {
            try
            {
                PrintReportResponse res = _fp700.PrintReport(ReportType.Z);
                _messenger.Publish(new EcrRespondedEvent(res));
            }
            catch (Exception ex)
            {
                _messenger.Publish(new EcrThrewExceptionEvent(ex));
            }
        }

        public void PrintXReport()
        {
            try
            {
                PrintReportResponse res = _fp700.PrintReport(ReportType.X);
                _messenger.Publish(new EcrRespondedEvent(res));
            }
            catch (Exception ex)
            {
                _messenger.Publish(new EcrThrewExceptionEvent(ex));
            }
        }

        public void GetLastFiscalEntryInfo()
        {
            try
            {
                GetLastFiscalEntryInfoResponse res = _fp700.GetLastFiscalEntryInfo();
                _messenger.Publish(new EcrRespondedEvent(res));
            }
            catch (Exception ex)
            {
                _messenger.Publish(new EcrThrewExceptionEvent(ex));
            }
        }

        public void ReadStatus()
        {
            try
            {
                ReadStatusResponse res = _fp700.ReadStatus();
                _messenger.Publish(new EcrRespondedEvent(res));
            }
            catch (Exception ex)
            {
                _messenger.Publish(new EcrThrewExceptionEvent(ex));
            }
        }
    }
}
