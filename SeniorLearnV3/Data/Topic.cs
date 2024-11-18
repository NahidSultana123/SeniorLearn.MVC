namespace SeniorLearnV3.Data
{
    public class Topic
    {
        public Topic()
        {

        }
        public Topic(int id, string name)
        {
            Id = id;
            Name = name;
        }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int OrganisationId { get; set; }
        public Organisation Organisation { get; set; } = default!;

        // Sprint 2 - Done

        public List<Lesson> Lessons { get; set; } = new();

    }
}
