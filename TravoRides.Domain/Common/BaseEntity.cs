namespace TravoRides.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        protected BaseEntity()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.Now;
            CreatedBy = "System";
            ModifiedAt = DateTime.Now;
            ModifiedBy = "System";
            IsActive = true;
            IsDeleted = false;
        }
    }
}
