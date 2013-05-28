namespace EsterCommon.PlanObjectTypes.Data
{
    public struct BaseObjectProperty
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public int TypeId { get; set; }
        public string Path { get; set; }
        public int Units { get; set; }
        public double Multipler { get; set; }
    }
}
