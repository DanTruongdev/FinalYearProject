namespace OnlineShopping.Hubs.Models
{
    public class JwtToken
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }

        public JwtToken(string token, DateTime expiration)
        {
            Token = token;
            Expiration = expiration;
        }
    }


}
