using FP700KasaGe.Core;

namespace FP700Win.Events
{
    public class EcrRespondedEvent
    {
        public IFiscalResponse Response;

        public EcrRespondedEvent(IFiscalResponse response)
        {
            Response = response;
        }
    }
}
