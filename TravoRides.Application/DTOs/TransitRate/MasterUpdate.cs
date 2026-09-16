
using TravoRides.Application.DTOs.Cabs;
using TravoRides.Application.DTOs.Transit;

namespace TravoRides.Application.DTOs.TransitRate
{
    public class MasterUpdate
    {
        // Section 1 - Transit details
        public UpdateTransitRequest UpdateTransit { get; set; } = new();

        // Section 2 - Add cab
        public TransitCabRequest TransitCabRequest { get; set; } = new();

        // Available cabs for dropdown
        public List<CabDTO> AvailableCabs { get; set; } = new();

        // Section 3 - Already added cabs
        public List<TransitCabRateDTO> TransitCabRates { get; set; } = new();

        // Used when editing an existing Transit cab
        public UpdateTransitCabRequest UpdateTransitCab { get; set; } = new();
    }

}
