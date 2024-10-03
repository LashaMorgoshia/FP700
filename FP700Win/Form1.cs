using FP700KasaGe;
using FP700KasaGe.Commands;
using FP700KasaGe.Responses;
using FP700Win.Events;
using FP700Win.Implementation;
using FP700Win.Interfaces;
using System;
using System.Windows.Forms;

namespace FP700Win
{
    public partial class Form1 : Form
    {
        private readonly FP700 _fp700;
        private readonly IMessageAggregator _messenger;

        public Form1()
        {
            InitializeComponent();

            _fp700 = new FP700("COM7");
            _messenger = new MessageAggregator();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PrintXReport();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
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
