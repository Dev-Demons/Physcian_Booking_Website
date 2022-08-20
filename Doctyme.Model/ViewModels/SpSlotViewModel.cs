using System;

namespace Doctyme.Model.ViewModels
{
    public class SpSlotViewModel : SpAllTypeViewModel
    {
        public string StSlotDate { get; set; }
        public DateTime SlotDate { get; set; }
    }

    public class appSlotViewModel
    {
        public int DoctorId { get; set; }
        public int SlotId { get; set; }
        public int PatientUserId { get; set; }
    }
}
