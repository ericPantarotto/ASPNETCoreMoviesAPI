namespace MoviesAPI.DTOs
{
    public class Link
    {
        public string Href { get; set; } //endpoint
        public string Rel { get; set; } //description
        public string Method { get; set; } //Http Method

        public Link(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}
