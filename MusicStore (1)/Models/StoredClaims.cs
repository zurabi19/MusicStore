using System.Security.Claims;
namespace MusicStore__1_.Models
{
    public static class StoredClaims
    {
        public static List<Claim> claims = new List<Claim>()
        {
            new Claim("Create Role", "Create Role"),
            new Claim("Edit Role", "Edit Role"),
        };
    }
}
