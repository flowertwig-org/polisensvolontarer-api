namespace Api.Contracts
{
    public class Assignment
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Date { get; set; }

        public string Area { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("[Assignment: Name={0}, Date={1} Category={2}, Area={3}, Description={4}, Link={5}]", Name, Date, Category, Area, Description, Link);
        }
    }
}
