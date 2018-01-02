namespace Api.Contracts
{
    public class Assignment
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Date { get; set; }

        public string Area { get; set; }

        public override string ToString()
        {
            return string.Format("[Assignment: Name={0}, Date={1} Category={2}, Area={3}, Id={4}]", Name, Date, Category, Area, Id);
        }
    }
}
