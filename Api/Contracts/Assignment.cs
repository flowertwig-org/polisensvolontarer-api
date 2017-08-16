namespace Api.Contracts
{
    public class Assignment
    {
        public string Name { get; set; }
        public string Category { get; set; }

        public string Area { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("[Assignment: Name={0}, Category={1}, Area={2}, Description={3}]", Name, Category, Area, Description);
        }
    }
}
