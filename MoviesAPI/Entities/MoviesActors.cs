namespace MoviesAPI.Entities
{
    public class MoviesActors
    {
        public int PersonId { get; set; }
        public int MovieId { get; set; }
        public Person Person { get; set; }
        public Movie Movie { get; set; }
        //NOTE: we can also add fields:
        public string Character { get; set; }
        public int Order { get; set; }
    }
}
