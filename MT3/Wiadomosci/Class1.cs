using MassTransit;

namespace Wiadomosci
{
    public interface IMessage { }

    public record StartZamowienia(Guid OrderId, int Ilosc, Uri ClientQueue) : IMessage;
    public record PytanieoWolne(Guid OrderId, int Ilosc) : IMessage;
    public record PytanieoPotwierdzenie(Guid OrderId, int Ilosc) : IMessage;

    public record Potwierdzenie(Guid OrderId) : IMessage, CorrelatedBy<Guid>
    {
        public Guid CorrelationId => OrderId;
    }

    public record BrakPotwierdzenia(Guid OrderId) : IMessage, CorrelatedBy<Guid>
    {
        public Guid CorrelationId => OrderId;
    }

    public record OdpowiedzWolne(Guid OrderId) : IMessage;
    public record OdpowiedzWolneNegatywna(Guid OrderId) : IMessage;

    public record AkceptacjaZamowienia(Guid OrderId, int Ilosc) : IMessage;
    public record OdrzucenieZamowienia(Guid OrderId, int Ilosc) : IMessage;

    public class Timeout : CorrelatedBy<Guid>
    {
        public Guid CorrelationId { get; set; }
    }
}
